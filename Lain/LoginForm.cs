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
using System.IO.Compression;
using System.Security;

namespace Lain
{
    public partial class LoginForm : Form
    {
        LoginType _type;
        bool _showPassword = false;

        public LoginForm(LoginType type, FormWindowState state = FormWindowState.Normal)
        {
            InitializeComponent();
            this.WindowState = state;
            Options.ApplyTheme(this);
            _type = type;
        }

        private void Login()
        {
            if ((!string.IsNullOrEmpty(txtPassword.Text)) && (File.Exists(Required.LainSerial)))
            {
                string hashed = CryLain.HashKey(txtPassword.Text);
                string stored = File.ReadAllText(Required.LainSerial);

                if (hashed == stored)
                {
                    switch (_type)
                    {
                        case LoginType.Login:
                            Program.SetMainForm(new MainForm(CryLain.ToSecureString(txtPassword.Text)));
                            this.Close();
                            Program.ShowMainForm();
                            break;

                        case LoginType.Authorize:
                            this.DialogResult = DialogResult.Yes;
                            this.Close();
                            break;

                        case LoginType.Remove:
                            this.DialogResult = DialogResult.Yes;
                            this.Close();
                            break;

                        case LoginType.RemoveAll:
                            this.DialogResult = DialogResult.Yes;
                            this.Close();
                            break;
                    }

                    txtPassword.Text = string.Empty;
                    hashed = string.Empty;
                    stored = string.Empty;
                }
                else
                {
                    MessageBox.Show("The password you provided is incorrect!", "ERROR: Logging in", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            switch (_type)
            {
                case LoginType.Login:
                    Application.Exit();
                    break;

                case LoginType.Authorize:
                    this.DialogResult = DialogResult.No;
                    this.Close();
                    break;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Login();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to reset Lain?\nThis will delete everything!", "Reset Lain?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Reset();
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            switch (_type)
            {
                case LoginType.Authorize:
                    btnExit.Text = "Cancel";
                    btnReset.Visible = false;
                    btnRestore.Visible = false;
                    break;

                case LoginType.Remove:
                    btnExit.Text = "Cancel";
                    btnReset.Visible = false;
                    btnRestore.Visible = false;
                    status.Text = "Enter your password\nto remove this account";
                    break;

                case LoginType.RemoveAll:
                    btnExit.Text = "Cancel";
                    btnReset.Visible = false;
                    btnRestore.Visible = false;
                    status.Text = "Enter your password\nto remove all accounts";
                    break;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (_showPassword)
            {
                pictureBox1.BackColor = Options.BackgroundColor;
                txtPassword.UseSystemPasswordChar = true;
                _showPassword = false;
                return;
            }
            else
            {
                pictureBox1.BackColor = Options.ForegroundColor;
                txtPassword.UseSystemPasswordChar = false;
                _showPassword = true;
                return;
            }
        }

        private void RestoreBackup()
        {
            if (MessageBox.Show("Do you really want restore a backup?\nThis will delete your current accounts!", "Restore backup?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".Lain";
                dialog.Title = "Restore backup ...";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                dialog.FileName = string.Empty;
                dialog.Filter = "Lain files|*.Lain";

                if (dialog.ShowDialog() == DialogResult.OK)
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
                        try
                        {
                            ZipFile.ExtractToDirectory(dialog.FileName, Required.DataFolder);
                            Application.Restart();
                        }
                        catch { }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RestoreBackup();
        }
    }
}
