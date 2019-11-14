using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
        string _term = string.Empty;

        public MainForm(SecureString key)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            DoubleBuffered = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Options.ApplyTheme(this);
            FixColor();
            TriggerTimer();
            helperMenu.Renderer = new ToolStripRendererMaterial();

            DeserializeAccounts();
            Key = key;
            this.Text = string.Format("Lain [{0} accounts]", Accounts.Count);
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
                    if (_temp.ToLowerInvariant().Contains(_term))
                    {
                        AccountView.Rows.Add(new string[] { _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Name()),
                                _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Email()),
                                _cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Password())
                            });
                    }
                }
            }
        }

        //private void Reset()
        //{
        //    try
        //    {
        //        if (Directory.Exists(Required.DataFolder))
        //        {
        //            Directory.Delete(Required.DataFolder, true);
        //        }
        //    }
        //    catch { }
        //    finally
        //    {
        //        Application.Restart();
        //    }
        //}

        internal void FixColor()
        {
            AccountView.DefaultCellStyle.SelectionBackColor = Options.ForegroundColor;
            AccountView.ColumnHeadersDefaultCellStyle.SelectionBackColor = Options.ForegroundColor;
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
                    Clipboard.SetText(rows[0].Cells[2].Value.ToString());
                }
                catch { }
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

        private void LoadAccounts()
        {
            AccountView.Rows.Clear();
            AccountView.Refresh();

            foreach (LainAccount x in Accounts)
            {
                AccountView.Rows.Add(new string[] { _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Name()),
                    _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Email()),
                    _cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Password())
                        });
            }

            txtSearch.Clear();
            this.Text = string.Format("Lain {0} [{1} accounts]", Program.GetCurrentVersionToString(), Accounts.Count);

            AccountView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            AccountView.Sort(AccountView.Columns[0], ListSortDirection.Ascending);
            
            if (AccountView.Rows.Count > 0)
            {
                AccountView.ClearSelection();
                AccountView.Rows[0].Selected = true;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            clearPic.Visible = false;
            LoadAccounts();
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
    }
}
