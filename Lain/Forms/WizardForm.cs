using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lain
{
    public partial class WizardForm : Form
    {
        bool _showPassword = false;

        public WizardForm()
        {
            InitializeComponent();
            Options.ApplyTheme(this);
        }

        private void PasswordVisibility()
        {
            if (_showPassword)
            {
                pictureBox1.BackColor = Options.BackgroundColor;
                pictureBox2.BackColor = Options.BackgroundColor;
                txtPassword.UseSystemPasswordChar = true;
                txtVerify.UseSystemPasswordChar = true;
                _showPassword = false;
                return;
            }
            else
            {
                pictureBox1.BackColor = Options.ForegroundColor;
                pictureBox2.BackColor = Options.ForegroundColor;
                txtPassword.UseSystemPasswordChar = false;
                txtVerify.UseSystemPasswordChar = false;
                _showPassword = true;
                return;
            }
        }

        private void Register()
        {
            if ((txtPassword.Text == txtVerify.Text) && (!string.IsNullOrEmpty(txtPassword.Text)) && (!string.IsNullOrEmpty(txtVerify.Text)))
            {
                try
                {
                    File.WriteAllText(Required.LainSerial, CryLain.HashKey(txtVerify.Text));

                    Program.SetMainForm(new MainForm(CryLain.ToSecureString(txtVerify.Text)));
                    txtPassword.Text = string.Empty;
                    txtVerify.Text = string.Empty;
                    this.Close();
                    Program.ShowMainForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR: Creating account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ValidatePassword()
        {
            if ((txtPassword.Text == txtVerify.Text) && (!string.IsNullOrEmpty(txtPassword.Text)) && (!string.IsNullOrEmpty(txtVerify.Text)))
            {
                txtPassword.ForeColor = Color.Lime;
                txtVerify.ForeColor = Color.Lime;
            }
            else
            {
                txtPassword.ForeColor = Color.Tomato;
                txtVerify.ForeColor = Color.Tomato;
            }
        }

        private void WizardForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Register();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            ValidatePassword();
        }

        private void txtVerify_TextChanged(object sender, EventArgs e)
        {
            ValidatePassword();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PasswordVisibility();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            PasswordVisibility();
        }
    }
}
