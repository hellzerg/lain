using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security;

namespace Lain
{
    public partial class MainForm : Form
    {
        const int ONE_MINUTE_IN_MILLISECONDS = 60000;
        bool IsDialogOpen = false;

        internal static List<LainAccount> Accounts = new List<LainAccount>();
        internal static SecureString Key;
        CryLain cryLain = new CryLain();

        string temp, term = string.Empty;

        public MainForm(SecureString key)
        {
            InitializeComponent();
            Options.ApplyTheme(this);
            FixColor();
            TriggerTimer();
            HelperMenu.Renderer = new ToolStripRendererMaterial();

            DeserializeAccounts();
            Key = key;
            this.Text = string.Format("Lain [{0} accounts]", Accounts.Count);
        }

        private void TriggerTimer()
        {
            timerAutoLock.Interval = Options.CurrentOptions.Minutes * ONE_MINUTE_IN_MILLISECONDS;
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                pictureBox1.Visible = false;
                LoadAccounts();
            }
            else
            {
                pictureBox1.Visible = true;
                term = txtSearch.Text.Trim().ToLowerInvariant();
                AccountView.Nodes.Clear();
                foreach (LainAccount la in Accounts)
                {
                    temp = cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Name());
                    if (temp.ToLowerInvariant().Contains(term))
                    {
                        TreeNode node = new TreeNode(cryLain.Decrypt(CryLain.ToInsecureString(Key), la.Name()));
                        node.ForeColor = Options.ForegroundColor;
                        node.Tag = Options.ThemeFlag;
                        node.Nodes.Add("Name/Email: ");
                        node.Nodes.Add("Password: ");
                        node.Nodes.Add("Note: ");
                        AccountView.Nodes.Add(node);
                    }
                }
            }
        }

        private void Reset()
        {
            try
            {
                if (Directory.Exists(Required.DataFolder))
                {
                    Directory.Delete(Required.DataFolder, true);
                }
            }
            catch { }
            finally
            {
                Application.Restart();
            }
        }

        internal void FixColor()
        {
            foreach (TreeNode node in AccountView.Nodes)
            {
                if ((string)node.Tag == Options.ThemeFlag)
                {
                    node.ForeColor = Options.ForegroundColor;
                }
            }
        }

        private void Copy()
        {
            string s = AccountView.SelectedNode.Text;
            s = s.Replace("Name/Email: ", string.Empty);
            s = s.Replace("Password: ", string.Empty);
            s = s.Replace("Note: ", string.Empty);

            try
            {
                Clipboard.SetText(s);
            }
            catch { }
        }

        private void Modify()
        {
            if (AccountView.SelectedNode != null)
            {
                if (Authorize(LoginType.Authorize))
                {
                    if (AccountView.SelectedNode.Parent == null)
                    {
                        NewForm f = new NewForm(NewType.Modify, AccountView.SelectedNode.Text);
                        IsDialogOpen = true;
                        f.ShowDialog();
                        IsDialogOpen = false;
                    }
                    else
                    {
                        NewForm f = new NewForm(NewType.Modify, AccountView.SelectedNode.Parent.Text);
                        IsDialogOpen = true;
                        f.ShowDialog();
                        IsDialogOpen = false;
                    }

                    LoadAccounts();
                }
            }
        }

        private bool Authorize(LoginType type)
        {
            if (!Options.CurrentOptions.Authorize)
            {
                return true;
            }

            bool result = false;
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

            return result;
        }

        private void Remove()
        {
            if (AccountView.SelectedNode != null)
            {
                if (Authorize(LoginType.Remove))
                {
                    string name = string.Empty;
                    if (AccountView.SelectedNode.Parent == null)
                    {
                        name = AccountView.SelectedNode.Text;
                    }
                    else
                    {
                        name = AccountView.SelectedNode.Parent.Text;
                    }
                    int i = Accounts.FindIndex(x => x.Name() == cryLain.Encrypt(CryLain.ToInsecureString(Key), name));
                    if (i > -1) { Accounts.RemoveAt(i); }
                    LoadAccounts();
                }
            }
        }

        private void RemoveAll()
        {
            if (AccountView.Nodes.Count > 0)
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
            AccountView.Nodes.Clear();

            foreach (LainAccount x in Accounts)
            {
                TreeNode node = new TreeNode(cryLain.Decrypt(CryLain.ToInsecureString(Key), x.Name()));
                node.ForeColor = Options.ForegroundColor;
                node.Tag = Options.ThemeFlag;
                node.Nodes.Add("Name/Email: ");
                node.Nodes.Add("Password: ");
                node.Nodes.Add("Note: ");
                AccountView.Nodes.Add(node);
            }

            txtSearch.Clear();
            this.Text = string.Format("Lain {0} [{1} accounts]", Program.GetCurrentVersionToString(), Accounts.Count);

            AccountView.Sort();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            pictureBox1.Visible = false;
            LoadAccounts();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            NewForm f = new NewForm(NewType.New);
            IsDialogOpen = true;
            f.ShowDialog(this);
            IsDialogOpen = false;
            LoadAccounts();

            Program.SaveSettings();
        }

        private void AccountView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (Authorize(LoginType.Authorize))
            {
                LainAccount account = Accounts.Find(x => x.Name() == cryLain.Encrypt(CryLain.ToInsecureString(Key), e.Node.Text));

                if (account != null)
                {
                    e.Node.Nodes[0].Text = "Name/Email: " + cryLain.Decrypt(CryLain.ToInsecureString(Key), account.Email());
                    e.Node.Nodes[1].Text = "Password: " + cryLain.Decrypt(CryLain.ToInsecureString(Key), account.Password());
                    e.Node.Nodes[2].Text = "Note: " + cryLain.Decrypt(CryLain.ToInsecureString(Key), account.Note());
                }

                account = null;
                this.Cursor = Cursors.Default;
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void AccountView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button.Equals(MouseButtons.Right))
            {
                AccountView.SelectedNode = e.Node;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Lock();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            OptionsForm f = new OptionsForm(this);
            IsDialogOpen = true;
            f.ShowDialog(this);
            IsDialogOpen = false;
            TriggerTimer();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IsDialogOpen = true;
            DialogResult r = MessageBox.Show("Do you really want to reset Lain?\nThis will delete everything!", "Reset Lain?", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            IsDialogOpen = false;
            if (r == DialogResult.Yes)
            {
                Reset();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            IsDialogOpen = true;
            f.ShowDialog();
            IsDialogOpen = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewForm f = new NewForm(NewType.New);
            f.ShowDialog(this);

            LoadAccounts();
        }

        private void ediToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void removeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveAll();
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
    }
}
