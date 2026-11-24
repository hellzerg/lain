using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Lain
{
    public sealed partial class WizardForm : Form
    {
        bool _showPassword = false;

        public WizardForm()
        {
            InitializeComponent();
            Options.ApplyTheme(this);

            passwordBox.SetEditor(txtPassword);
            passwordBox.AddTrailing(pictureBox1);
            verifyBox.SetEditor(txtVerify);
            verifyBox.AddTrailing(pictureBox2);
            Fluent.SetPlaceholder(txtPassword, "Choose a strong password");
            Fluent.SetPlaceholder(txtVerify, "Type it again");
            Fluent.SetEye(pictureBox1, false);
            Fluent.SetEye(pictureBox2, false);
            lblIntro.ForeColor = Fluent.TextSecondary;
            FormClosed += (s, e) => ClearFields();
            Fluent.StyleDialog(this);
        }

        private void PasswordVisibility()
        {
            _showPassword = !_showPassword;
            txtPassword.UseSystemPasswordChar = !_showPassword;
            txtVerify.UseSystemPasswordChar = !_showPassword;
            Fluent.SetEye(pictureBox1, _showPassword);
            Fluent.SetEye(pictureBox2, _showPassword);
        }

        private void Register()
        {
            if ((txtPassword.Text == txtVerify.Text) && (!string.IsNullOrEmpty(txtPassword.Text)) && (!string.IsNullOrEmpty(txtVerify.Text)))
            {
                try
                {
                    Cursor = Cursors.WaitCursor;
                    VaultKey key = CryLain.CreateVault(txtVerify.Text);
                    Cursor = Cursors.Default;

                    Program.SetMainForm(new MainForm(key, new System.Collections.Generic.List<LainAccount>()));
                    txtPassword.Text = string.Empty;
                    txtVerify.Text = string.Empty;
                    this.Close();
                    Program.ShowMainForm();
                }
                catch (Exception ex)
                {
                    Cursor = Cursors.Default;
                    FluentMessageBox.Show(ex.Message, "ERROR: Creating account", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void ClearFields()
        {
            txtPassword.Text = string.Empty;
            txtVerify.Text = string.Empty;
        }

        private void ValidatePassword()
        {
            bool strong = PasswordStrength.IsAcceptableMasterPassword(txtPassword.Text);
            bool match = txtPassword.Text == txtVerify.Text && txtPassword.Text.Length > 0;
            btnOk.Enabled = strong && match;

            if (txtPassword.Text.Length == 0 && txtVerify.Text.Length == 0)
            {
                lblMatch.Text = string.Empty;
            }
            else if (!strong)
            {
                lblMatch.ForeColor = Fluent.Caution;
                lblMatch.Text = "Use at least 15 characters and choose a Strong password";
            }
            else if (txtVerify.Text.Length == 0)
            {
                lblMatch.ForeColor = Fluent.Success;
                lblMatch.Text = "\u2713  Strong password";
            }
            else if (match)
            {
                lblMatch.ForeColor = Fluent.Success;
                lblMatch.Text = "\u2713  Passwords match";
            }
            else
            {
                lblMatch.ForeColor = Fluent.Danger;
                lblMatch.Text = "Passwords don't match yet";
            }
        }

        private void WizardForm_Load(object sender, EventArgs e)
        {
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
