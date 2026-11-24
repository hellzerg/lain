using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lain
{
    public sealed partial class ChangePasswordForm : Form
    {
        bool _showPassword;
        bool _busy;
        readonly int _minimumPasswordLength;

        public ChangePasswordForm()
        {
            InitializeComponent();
            _minimumPasswordLength = CryLain.GetRequiredMasterPasswordLength();
            Options.ApplyTheme(this);

            currentBox.SetEditor(txtCurrent);
            currentBox.AddTrailing(eyeCurrent);
            newBox.SetEditor(txtNew);
            newBox.AddTrailing(eyeNew);
            verifyBox.SetEditor(txtVerify);
            verifyBox.AddTrailing(eyeVerify);
            Fluent.SetPlaceholder(txtCurrent, "The password you unlock Lain with");
            Fluent.SetPlaceholder(txtNew, "Choose a strong password");
            Fluent.SetPlaceholder(txtVerify, "Type it again");
            foreach (IconLabel eye in new[] { eyeCurrent, eyeNew, eyeVerify })
            {
                Fluent.SetEye(eye, false);
                eye.Click += (s, e) => TogglePasswordVisibility();
            }
            lblIntro.ForeColor = Fluent.TextSecondary;
            keyIcon.GlyphColor = Fluent.Accent;

            txtCurrent.TextChanged += (s, e) => ValidateInput();
            txtNew.TextChanged += (s, e) => { strengthMeter.Evaluate(txtNew.Text); ValidateInput(); };
            txtVerify.TextChanged += (s, e) => ValidateInput();
            btnOk.Click += btnOk_Click;
            FormClosing += (s, e) => { if (_busy) e.Cancel = true; };
            FormClosed += (s, e) => ClearFields();

            Fluent.StyleDialog(this);
        }

        private void TogglePasswordVisibility()
        {
            _showPassword = !_showPassword;
            foreach (FluentTextBox t in new[] { txtCurrent, txtNew, txtVerify }) t.UseSystemPasswordChar = !_showPassword;
            foreach (IconLabel eye in new[] { eyeCurrent, eyeNew, eyeVerify }) Fluent.SetEye(eye, _showPassword);
        }

        private void ShowMessage(string text, System.Drawing.Color color)
        {
            lblMessage.ForeColor = color;
            lblMessage.Text = text;
        }

        private void ValidateInput()
        {
            bool filled = txtCurrent.Text.Length > 0 && txtNew.Text.Length > 0;
            bool match = txtNew.Text == txtVerify.Text;
            bool strong = PasswordStrength.IsAcceptableMasterPassword(txtNew.Text, _minimumPasswordLength);
            bool different = txtNew.Text != txtCurrent.Text;
            btnOk.Enabled = !_busy && filled && match && strong && different;

            if (txtNew.Text.Length == 0 && txtVerify.Text.Length == 0) ShowMessage(string.Empty, Fluent.TextSecondary);
            else if (!strong) ShowMessage("Use at least " + _minimumPasswordLength + " characters and choose a Strong password.", Fluent.Caution);
            else if (txtVerify.Text.Length > 0 && !match) ShowMessage("Passwords don't match yet", Fluent.Danger);
            else if (!different) ShowMessage("The new password is the same as the current one.", Fluent.Caution);
            else if (txtVerify.Text.Length == 0) ShowMessage("\u2713  Strong password", Fluent.Success);
            else ShowMessage("\u2713  Passwords match", Fluent.Success);
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            UseWaitCursor = busy;
            currentBox.Enabled = newBox.Enabled = verifyBox.Enabled = btnCancel.Enabled = !busy;
            btnOk.Text = busy ? "Re-encrypting\u2026" : "Change password";
            ValidateInput();
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            string current = txtCurrent.Text;
            string next = txtNew.Text;
            if (_busy || next != txtVerify.Text) return;
            if (!PasswordStrength.IsAcceptableMasterPassword(next, _minimumPasswordLength))
            {
                ShowMessage("Use at least " + _minimumPasswordLength + " characters and choose a Strong password.", Fluent.Caution);
                return;
            }
            if (next == current)
            {
                ShowMessage("Choose a password that is different from the current one.", Fluent.Danger);
                return;
            }
            if (MainForm.Key == null) return;

            VaultKey oldKey = MainForm.Key;
            List<LainAccount> snapshot = new List<LainAccount>(MainForm.Accounts);
            SetBusy(true);
            ShowMessage("Checking your current password\u2026", Fluent.TextSecondary);
            try
            {
                bool verified = await Task.Run(() => CryLain.VerifyPassword(current, oldKey));
                if (!verified)
                {
                    SetBusy(false);
                    ShowMessage("The current password is incorrect.", Fluent.Danger);
                    txtCurrent.Focus();
                    txtCurrent.SelectAll();
                    return;
                }

                ShowMessage("Re-encrypting your vault\u2026", Fluent.TextSecondary);
                List<LainAccount> accounts = null;
                VaultKey newKey = await Task.Run(() => CryLain.ChangeMasterPassword(oldKey, snapshot, next, out accounts));

                MainForm.ReplaceVault(oldKey, newKey, accounts);

                SetBusy(false);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                SetBusy(false);
                ShowMessage(ex.Message, Fluent.Danger);
            }
        }

        private void ClearFields()
        {
            txtCurrent.Text = string.Empty;
            txtNew.Text = string.Empty;
            txtVerify.Text = string.Empty;
        }
    }
}
