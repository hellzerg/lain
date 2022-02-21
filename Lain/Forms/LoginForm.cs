using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Lain
{
    public partial class LoginForm : Form
    {
        LoginType _type;
        bool _showPassword = false;

        internal LoginForm(LoginType type, FormWindowState state = FormWindowState.Normal)
        {
            InitializeComponent();

            if (state == FormWindowState.Maximized)
            {
                this.WindowState = FormWindowState.Normal;
            }
            else
            {
                this.WindowState = state;
            }

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

        private void LoginForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            switch (_type)
            {
                case LoginType.Authorize:
                    btnExit.Text = "Cancel";
                    break;

                case LoginType.Remove:
                    btnExit.Text = "Cancel";
                    status.Text = "Enter your password\nto delete this account";
                    break;

                case LoginType.RemoveAll:
                    btnExit.Text = "Cancel";
                    status.Text = "Enter your password\nto delete all your accounts";
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
    }
}
