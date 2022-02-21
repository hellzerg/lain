using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace Lain
{
    public partial class MainForm : Form
    {
        readonly string _latestVersionLink = "https://raw.githubusercontent.com/hellzerg/lain/master/version.txt";

        readonly string _noNewVersionMessage = "You already have the latest version!";
        readonly string _betaVersionMessage = "You are using an experimental version!";

        const int ONE_MINUTE_IN_MILLISECONDS = 60000;

        bool IsDialogOpen = false;

        internal static List<LainAccount> Accounts = new List<LainAccount>();
        internal static SecureString Key;

        CryLain _cryLain = new CryLain();

        string _temp = string.Empty;
        string _temp2 = string.Empty;
        string _term = string.Empty;
        string _temp3 = string.Empty;

        public MainForm(SecureString key)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            DoubleBuffered = true;
            KeyPreview = true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Options.ApplyTheme(this);
            FixColors();

            TriggerTimer();
            helperMenu.Renderer = new MoonMenuRenderer();

            DeserializeAccounts();
            Key = key;
            this.Text = string.Format("Lain - {0} accounts", Accounts.Count);

            RestoreWindowState();
            SetFontSize();

            clearPic.Visible = false;
            LoadAccounts();
        }

        private void RestoreWindowState()
        {
            this.WindowState = Options.CurrentOptions.WindowState;
            this.Size = Options.CurrentOptions.WindowSize;

            if (Options.CurrentOptions.WindowLocation != null)
            {
                this.Location = (Point)Options.CurrentOptions.WindowLocation;
            }
            else
            {
                this.CenterToScreen();
            }
        }

        private void SaveWindowState()
        {
            Options.CurrentOptions.WindowState = this.WindowState;

            if (this.WindowState == FormWindowState.Normal)
            {
                Options.CurrentOptions.WindowLocation = this.Location;
                Options.CurrentOptions.WindowSize = this.Size;
            }
            else
            {
                Options.CurrentOptions.WindowLocation = this.RestoreBounds.Location;
                Options.CurrentOptions.WindowSize = this.RestoreBounds.Size;
            }
        }

        private string NewVersionMessage(string latest)
        {
            return string.Format("There is a new version available!\n\nLatest version: {0}\nCurrent version: {1}\n\nDo you want to download it now?", latest, Program.GetCurrentVersionToString());
        }

        private void TriggerTimer()
        {
            timerAutoLock.Interval = Options.CurrentOptions.Minutes * ONE_MINUTE_IN_MILLISECONDS;
        }

        private string NewDownloadLink(string latestVersion)
        {
            return string.Format("https://github.com/hellzerg/lain/releases/download/{0}/Lain-{0}.exe", latestVersion);
        }

        private void CheckForUpdate()
        {
            WebClient client = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            string latestVersion = string.Empty;
            try
            {
                latestVersion = client.DownloadString(_latestVersionLink);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (!string.IsNullOrEmpty(latestVersion))
            {
                if (float.Parse(latestVersion) > Program.GetCurrentVersion())
                {
                    if (MessageBox.Show(NewVersionMessage(latestVersion), "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        // PATCHING PROCESS
                        try
                        {
                            Assembly currentAssembly = Assembly.GetEntryAssembly();

                            if (currentAssembly == null)
                            {
                                currentAssembly = Assembly.GetCallingAssembly();
                            }

                            string appFolder = Path.GetDirectoryName(currentAssembly.Location);
                            string appName = Path.GetFileNameWithoutExtension(currentAssembly.Location);
                            string appExtension = Path.GetExtension(currentAssembly.Location);

                            string archiveFile = Path.Combine(appFolder, appName + "_old" + appExtension);
                            string appFile = Path.Combine(appFolder, appName + appExtension);
                            string tempFile = Path.Combine(appFolder, appName + "_tmp" + appExtension);

                            // DOWNLOAD NEW VERSION
                            client.DownloadFile(NewDownloadLink(latestVersion), tempFile);

                            // DELETE PREVIOUS BACK-UP
                            if (File.Exists(archiveFile))
                            {
                                File.Delete(archiveFile);
                            }

                            // MAKE BACK-UP
                            File.Move(appFile, archiveFile);

                            // PATCH
                            File.Move(tempFile, appFile);

                            // BYPASS SINGLE-INSTANCE MECHANISM
                            Program.RestartLain();

                            Application.Restart();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }
                else if (float.Parse(latestVersion) == Program.GetCurrentVersion())
                {
                    MessageBox.Show(_noNewVersionMessage, "No update available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(_betaVersionMessage, "No update available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                clearPic.Visible = false;
                LoadAccounts();
            }
            else
            {
                clearPic.Visible = true;
                _term = txtSearch.Text.Trim().ToLowerInvariant();
                AccountView.Rows.Clear();

                foreach (LainAccount la in Accounts)
                {
                    _temp = _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Name());
                    _temp2 = _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Email());

                    if (_temp.ToLowerInvariant().Contains(_term) || _temp2.ToLowerInvariant().Contains(_term))
                    {
                        if (!Options.CurrentOptions.HidePasswords) _temp3 = _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Password());

                        AccountView.Rows.Add(new string[] { _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Name()),
                                _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Email()),
                                _temp3,
                                _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Link())
                            });
                    }
                }
            }

            _temp = string.Empty;
            _temp2 = string.Empty;
            _temp3 = string.Empty;
        }

        internal void FixColors()
        {
            AccountView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(65, 65, 65);
            AccountView.ColumnHeadersDefaultCellStyle.SelectionForeColor = Options.ForegroundColor;
            AccountView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Options.ForegroundColor;
            AccountView.ColumnHeadersDefaultCellStyle.ForeColor = Options.ForegroundColor;
            AccountView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 20, 20);
        }

        private void CopyAccount()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1)
            {
                try
                {
                    Clipboard.SetText(rows[0].Cells[1].Value.ToString());
                }
                catch { }
            }
        }

        private void CopyPassword()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1)
            {
                try
                {
                    string i = rows[0].Cells[0].Value.ToString();
                    int y = Accounts.FindIndex(x => i == _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Name()));
                    if (y > -1)
                    {
                        Clipboard.SetText(_cryLain.Decrypt(CryLain.ToInsecureString(Key), Accounts[y].Password()));
                    }
                }
                catch { }
            }
        }

        private void OpenLink()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1)
            {
                var link = rows[0].Cells[3].Value;
                if (link == null) return;

                if (!string.IsNullOrEmpty(link.ToString().Trim()))
                {
                    if (!link.ToString().StartsWith("http://") && !link.ToString().StartsWith("https://")) link = "https://" + link;
                    System.Diagnostics.Process.Start(link.ToString());
                }
            }
        }

        private void Modify()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1)
            {
                if (Authorize(LoginType.Authorize, true))
                {
                    NewForm f = new NewForm(NewType.Modify, rows[0].Cells[0].Value.ToString());

                    IsDialogOpen = true;
                    DialogResult r = f.ShowDialog();
                    IsDialogOpen = false;

                    if (r == DialogResult.OK)
                    {
                        LoadAccounts();
                    }
                }
            }
        }

        private bool Authorize(LoginType type, bool simple = false)
        {
            bool result = false;

            if (Options.CurrentOptions.Authorize)
            {
                LoginForm f = new LoginForm(type);

                IsDialogOpen = true;
                DialogResult dr = f.ShowDialog();
                IsDialogOpen = false;

                if (dr == DialogResult.Yes)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                if (simple)
                {
                    return true;
                }

                string title = string.Empty;
                switch (type)
                {
                    case LoginType.Remove:
                        title = "Delete account?";
                        break;
                    case LoginType.RemoveAll:
                        title = "Delete all accounts?";
                        break;
                }

                IsDialogOpen = true;
                DialogResult dr = MessageBox.Show("Are you sure you want to do this?", title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                IsDialogOpen = false;

                if (dr == DialogResult.Yes)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        private void Remove()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            string name;
            int i;

            if (Authorize(LoginType.Remove))
            {
                foreach (DataGridViewRow row in rows)
                {
                    name = row.Cells[0].Value.ToString();
                    i = Accounts.FindIndex(x => x.Name() == _cryLain.Encrypt(CryLain.ToInsecureString(Key), name));
                    if (i > -1) { Accounts.RemoveAt(i); }
                }

                LoadAccounts();
            }
        }

        private void CreateNewAccount()
        {
            NewForm f = new NewForm(NewType.New);
            IsDialogOpen = true;
            DialogResult r = f.ShowDialog(this);
            IsDialogOpen = false;

            if (r == DialogResult.OK)
            {
                LoadAccounts();
            }

            Program.SaveSettings();
        }

        private void RemoveAll()
        {
            if (AccountView.Rows.Count > 0)
            {
                if (Authorize(LoginType.RemoveAll))
                {
                    Accounts.Clear();
                    LoadAccounts();
                }
            }
        }

        private void Lock()
        {
            Program.SetMainForm(new LoginForm(LoginType.Login, this.WindowState));
            IsDialogOpen = true;
            timerAutoLock.Stop();
            this.Close();
            Program.ShowMainForm();
        }

        private void DeserializeAccounts()
        {
            try
            {
                if (File.Exists(Required.LainData))
                {
                    byte[] bytes = File.ReadAllBytes(Required.LainData);

                    MemoryStream ms = new MemoryStream(bytes);
                    BinaryFormatter bf = new BinaryFormatter();

                    Accounts = (List<LainAccount>)bf.Deserialize(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        internal void LoadAccounts()
        {
            AccountView.Rows.Clear();
            AccountView.Refresh();

            AccountView.Columns[2].Visible = !Options.CurrentOptions.HidePasswords;

            foreach (LainAccount x in Accounts)
            {
                if (!Options.CurrentOptions.HidePasswords) _temp3 = _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Password());

                AccountView.Rows.Add(new string[] { _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Name()),
                    _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Email()),
                    _temp3,
                    _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Link())
                });
            }

            _temp3 = string.Empty;
            txtSearch.Clear();
            this.Text = string.Format("Lain {0} - {1} accounts", Program.GetCurrentVersionToString(), Accounts.Count);

            SetAutoSizeColumns();

            if (AccountView.Rows.Count > 0)
            {
                AccountView.ClearSelection();
                AccountView.Rows[0].Selected = true;
            }
        }

        internal void SetAutoSizeColumns()
        {
            if (Options.CurrentOptions.AutoSizeColumns)
            {
                AccountView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                AccountView.Columns[AccountView.ColumnCount - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            else
            {
                AccountView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                AccountView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }

            AccountView.Sort(AccountView.Columns[0], ListSortDirection.Ascending);
        }

        // find duplicate passwords
        private void AnalyzeAccounts()
        {
            List<LainAccount> duplicates = new HashSet<LainAccount>(Accounts.Where(c => Accounts.Count(x => x.Password() == c.Password()) > 1)).ToList();

            if (duplicates.Count() <= 0)
            {
                MessageBox.Show("There are no accounts with identical passwords.", "Duplicate Passwords Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DuplicatesForm f = new DuplicatesForm(duplicates.OrderBy(g => g.Password()).ToList());
            duplicates.Clear();
            f.ShowDialog(this);
        }

        internal void SetFontSize()
        {
            if (Options.CurrentOptions.FontSize == 0)
            {
                leftPanel.Font = new Font("Segoe UI Semibold", 9f);
            }
            if (Options.CurrentOptions.FontSize == 1)
            {
                leftPanel.Font = new Font("Segoe UI Semibold", 11f);
            }
            if (Options.CurrentOptions.FontSize == 2)
            {
                leftPanel.Font = new Font("Segoe UI Semibold", 13f);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CreateNewAccount();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyAccount();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            Lock();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            OptionsForm f = new OptionsForm(this);
            IsDialogOpen = true;
            f.ShowDialog(this);
            IsDialogOpen = false;
            TriggerTimer();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            IsDialogOpen = true;
            f.ShowDialog();
            IsDialogOpen = false;
        }

        private void clearPic_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewAccount();
        }

        private void ediToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void timerAutoLock_Tick(object sender, EventArgs e)
        {
            Lock();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            timerAutoLock.Stop();
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (Options.CurrentOptions.AutoLock && !IsDialogOpen)
            {
                timerAutoLock.Start();
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            Search();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            CheckForUpdate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveWindowState();
            Options.SaveSettings();

            try
            {
                Clipboard.Clear();
            }
            catch { }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyPassword();
        }

        private void AccountView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Modify();
        }

        private void openLinkBtn_Click(object sender, EventArgs e)
        {
            OpenLink();
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            AnalyzeAccounts();
        }

        private void AccountView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                DataGridViewRow row = AccountView.Rows[e.RowIndex];

                AccountView.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;

                AccountView.Focus();
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) txtSearch.Clear();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                copyToolStripMenuItem.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W)
            {
                toolStripMenuItem1.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E)
            {
                openLinkBtn.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
            {
                txtSearch.Focus();
                if (!string.IsNullOrEmpty(txtSearch.Text)) txtSearch.SelectAll();
            }

            if (e.KeyCode == Keys.Delete)
            {
                removeToolStripMenuItem.PerformClick();
            }

            if (e.KeyCode == Keys.Enter)
            {
                ediToolStripMenuItem.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                CreateNewAccount();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L)
            {
                Lock();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                btnOptions.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
            {
                btnAnalyze.PerformClick();
            }
        }
    }
}
