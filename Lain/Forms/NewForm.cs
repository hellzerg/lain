using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lain
{
    public sealed partial class NewForm : Form
    {
        NewType _type;
        readonly LainAccount _existing;

        // The account that was saved, so the list can select it.
        internal LainAccount SavedAccount { get; private set; }

        bool _showPassword = false;

        readonly int DEFAULT_RANDOM_CHARACTERS = 32;

        internal NewForm(NewType type, LainAccount existing = null)
        {
            InitializeComponent();
            Options.ApplyTheme(this);

            nameBox.SetEditor(txtName);
            mailBox.SetEditor(txtMail);
            passwordBox.SetEditor(txtPassword);
            passwordBox.AddTrailing(pictureBox1);
            passwordBox.AddTrailing(btnCopyPassword);
            lengthBox.SetEditor(txtMaxRandomChars);
            linkBox.SetEditor(txtLink);
            noteBox.SetEditor(txtNote);
            tagsBox.SetEditor(txtTags);

            Fluent.SetPlaceholder(txtName, "e.g. GitHub");
            Fluent.SetPlaceholder(txtMail, "name@example.com");
            Fluent.SetPlaceholder(txtPassword, "Type or generate a password");
            Fluent.SetPlaceholder(txtLink, "https://");
            Fluent.SetPlaceholder(txtTags, "Comma-separated, e.g. work, email");
            Fluent.SetEye(pictureBox1, false);

            chkUppers.Style = CheckStyle.Checkbox;
            chkLowers.Style = CheckStyle.Checkbox;
            chkNums.Style = CheckStyle.Checkbox;
            chkSpecials.Style = CheckStyle.Checkbox;
            chkFavorite.Style = CheckStyle.Checkbox;

            txtPassword.TextChanged += (s, e) => UpdateStrength();
            txtName.TextChanged += (s, e) => UpdateStrength();
            txtMail.TextChanged += (s, e) => UpdateStrength();
            txtTags.KeyDown += txtName_KeyDown;

            label5.ForeColor = Fluent.TextSecondary;
            lblValidation.ForeColor = Fluent.Danger;
            toolTip.SetToolTip(pictureBox1, "Show or hide password");
            toolTip.SetToolTip(btnCopyPassword, "Copy password");
            toolTip.SetToolTip(btnGenerateRandom, "Fill in a random password using the options below");
            Fluent.StyleToolTip(toolTip);

            _type = type;
            _existing = existing;

            chkLowers.Checked = true;
            chkUppers.Checked = true;
            chkSpecials.Checked = true;
            chkNums.Checked = true;

            lblDates.ForeColor = Fluent.TextSecondary;

            switch (_type)
            {
                case NewType.Modify:
                    btnOk.Text = "Save";
                    lblTitle.Text = "Edit account";

                    if (_existing != null)
                    {
                        using (VaultKey.Cipher c = MainForm.Key.Begin())
                        {
                            txtName.Text = c.Decrypt(_existing.Name());
                            txtMail.Text = c.Decrypt(_existing.Email());
                            txtPassword.Text = c.Decrypt(_existing.Password());
                            txtLink.Text = c.Decrypt(_existing.Link());
                            txtNote.Text = c.Decrypt(_existing.Note());
                        }
                        this.Text = string.Format("Edit account - {0}", txtName.Text);

                        AccountMeta meta = AccountMeta.Read(MainForm.Key, _existing);
                        txtTags.Text = meta.TagText();
                        chkFavorite.Checked = meta.Favorite;

                        lblDates.Text = DatesLine(meta);
                        lblDates.Visible = lblDates.Text.Length > 0;
                    }
                    else
                    {
                        this.Text = "Edit account";
                    }

                    break;
            }

            FormClosed += (s, e) => ClearFields();
            Fluent.StyleDialog(this);
        }

        private void ClearFields()
        {
            txtName.Text = string.Empty;
            txtMail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtLink.Text = string.Empty;
            txtNote.Text = string.Empty;
            txtTags.Text = string.Empty;
            this.Text = string.Empty;
        }

        static string DatesLine(AccountMeta meta)
        {
            List<string> parts = new List<string>(3);
            if (meta.Created.HasValue) parts.Add("Created " + MainForm.FormatDate(meta.Created));
            if (meta.Modified.HasValue) parts.Add("Modified " + MainForm.FormatDate(meta.Modified));
            if (meta.LastUsed.HasValue) parts.Add("Last used " + MainForm.FormatDate(meta.LastUsed));
            return string.Join("  \u00B7  ", parts);
        }

        void UpdateStrength()
        {
            strengthMeter.Evaluate(txtPassword.Text, txtName.Text, txtMail.Text);
        }

        private void Save()
        {
            if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtMail.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                AccountMeta meta = AccountMeta.NewNow();
                int i = -1;
                switch (_type)
                {
                    case NewType.Modify:
                        i = _existing == null ? -1 : MainForm.Accounts.IndexOf(_existing);
                        if (i > -1)
                        {
                            meta = AccountMeta.Read(MainForm.Key, _existing).Clone();
                            meta.Modified = DateTime.UtcNow;
                            if (!string.Equals(MainForm.Key.Decrypt(_existing.Password()), txtPassword.Text, StringComparison.Ordinal)) meta.PasswordChanged = DateTime.UtcNow;
                            _existing.Forget();
                            MainForm.Accounts.RemoveAt(i);
                        }
                        break;
                }

                meta.Favorite = chkFavorite.Checked;
                meta.Tags = AccountMeta.ParseTags(txtTags.Text);

                LainAccount account;
                using (VaultKey.Cipher c = MainForm.Key.Begin())
                {
                    account = new LainAccount(
                        c.Encrypt(txtName.Text),
                        c.Encrypt(txtMail.Text),
                        c.Encrypt(txtPassword.Text),
                        c.Encrypt(txtLink.Text),
                        c.Encrypt(txtNote.Text)
                    );
                }
                AccountMeta.Write(MainForm.Key, account, meta);

                // keep the edited account at its old position so the list order stays stable
                if (i > -1) MainForm.Accounts.Insert(i, account);
                else MainForm.Accounts.Add(account);
                SavedAccount = account;

                txtName.Text = string.Empty;
                txtMail.Text = string.Empty;
                txtPassword.Text = string.Empty;
                txtLink.Text = string.Empty;
                txtNote.Text = string.Empty;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblDates.Visible = false;
                lblValidation.Visible = true;
                if (string.IsNullOrEmpty(txtName.Text)) txtName.Focus();
                else if (string.IsNullOrEmpty(txtMail.Text)) txtMail.Focus();
                else txtPassword.Focus();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void NewForm_Load(object sender, EventArgs e)
        {

            txtMaxRandomChars.ShortcutsEnabled = false;
            txtMaxRandomChars.Text = Math.Max(DEFAULT_RANDOM_CHARACTERS,
                Options.CurrentOptions.EnforceMinLength ? Options.CurrentOptions.GeneratorMinLength : 0).ToString();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _showPassword = !_showPassword;
            txtPassword.UseSystemPasswordChar = !_showPassword;
            Fluent.SetEye(pictureBox1, _showPassword);
        }

        private void btnGenerateRandom_Click(object sender, EventArgs e)
        {
            int length;
            if (!int.TryParse(txtMaxRandomChars.Text, out length) || length <= 0) length = DEFAULT_RANDOM_CHARACTERS;

            SettingsJson o = Options.CurrentOptions;
            if (o.EnforceMinLength && length < o.GeneratorMinLength)
            {
                length = o.GeneratorMinLength;
                toolTip.Show("Raised to the minimum of " + length + " characters (see Options)", lengthBox, 0, -Fluent.Scale(this, 30), 2000);
            }
            txtMaxRandomChars.Text = length.ToString();

            RandomPassword.WithLowerChars = chkLowers.Checked;
            RandomPassword.WithUpperChars = chkUppers.Checked;
            RandomPassword.WithSpecialChars = chkSpecials.Checked;
            RandomPassword.WithNumbers = chkNums.Checked;
            txtPassword.Text = RandomPassword.GenerateWithMinimums(length, o.EnforceMinPerType ? o.GeneratorMinPerType : 0);
            lblValidation.Visible = false;
        }

        private void txtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Save();
            }
        }

        private void txtMaxRandomChars_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void btnCopyPassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtPassword.Text))
                {
                    SecureClipboard.CopySecret(txtPassword.Text);
                    toolTip.Show(SecureClipboard.AutoClear ? "Copied, clears in " + SecureClipboard.Seconds + "s" : "Copied",
                        btnCopyPassword, 0, -Fluent.Scale(this, 30), 1200);
                }
            }
            catch { }
        }
    }
}
