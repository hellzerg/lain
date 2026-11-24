using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lain
{
    public sealed partial class LoginForm : Form
    {
        LoginType _type;
        KeyMode _mode;
        bool _showPassword = false;
        bool _busy;

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

            passwordBox.SetEditor(txtPassword);
            passwordBox.AddTrailing(pictureBox1);
            Fluent.SetPlaceholder(txtPassword, "Master password");
            Fluent.SetEye(pictureBox1, false);
            label1.ForeColor = Fluent.TextSecondary;
            lblError.ForeColor = Fluent.Danger;
            recoveryBox.SetEditor(txtRecovery);
            Fluent.SetPlaceholder(txtRecovery, "Recovery code");
            FormClosed += (s, e) => ClearFields();
            Fluent.StyleDialog(this);
        }

        private void ClearFields()
        {
            txtPassword.Text = string.Empty;
            txtRecovery.Text = string.Empty;
        }

        private static void ClearBytes(byte[] value)
        {
            if (value != null) Array.Clear(value, 0, value.Length);
        }

        bool IsLogin { get { return _type == LoginType.Login; } }

        private void SetBusy(bool busy)
        {
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
            btnOk.Enabled = btnKey.Enabled = btnRecovery.Enabled = !busy;
            txtPassword.ReadOnly = txtRecovery.ReadOnly = busy;
            _busy = busy;
        }

        private static WebAuthn.Assertion PromptKey(IntPtr hwnd)
        {
            List<SecurityKeyInfo> keys = CryLain.GetSecurityKeys();
            if (keys.Count == 0) throw new InvalidOperationException("No security keys are registered for this vault.");

            List<WebAuthn.Credential> credentials = new List<WebAuthn.Credential>(keys.Count);
            try
            {
                foreach (SecurityKeyInfo k in keys)
                {
                    credentials.Add(new WebAuthn.Credential
                    {
                        CredentialId = k.CredentialId,
                        Salt = k.Salt,
                        Algorithm = k.Algorithm,
                        KeyType = k.KeyType,
                        Curve = k.Curve,
                        PublicKeyX = k.PublicKeyX,
                        PublicKeyY = k.PublicKeyY
                    });
                }
                return WebAuthn.GetSecret(hwnd, credentials);
            }
            finally
            {
                // The credential list is a temporary copy of Lain.keys metadata.
                foreach (WebAuthn.Credential credential in credentials)
                {
                    ClearBytes(credential.CredentialId);
                    ClearBytes(credential.Salt);
                    ClearBytes(credential.PublicKeyX);
                    ClearBytes(credential.PublicKeyY);
                }
            }
        }

        // withKey: unlock with the security key alone ("password or key" mode).
        private async void Login(bool withKey = false)
        {
            if (_busy || !CryLain.VaultExists()) return;

            bool recovery = recoveryBox.Visible;
            string password = withKey ? null : txtPassword.Text;
            string code = recovery ? txtRecovery.Text : null;

            if (!withKey && string.IsNullOrEmpty(password))
            {
                txtPassword.Focus();
                return;
            }
            if (recovery && string.IsNullOrWhiteSpace(code))
            {
                txtRecovery.Focus();
                return;
            }

            // in "password and key" mode the login also needs the key (or the recovery code)
            bool needKey = withKey || (IsLogin && _mode == KeyMode.Both && !recovery);
            IntPtr hwnd = Handle;
            VaultKey session = IsLogin ? null : MainForm.Key;

            VaultKey key = null;
            List<LainAccount> accounts = null;
            bool ok;

            SetBusy(true);
            try
            {
                Tuple<VaultKey, List<LainAccount>, bool> r = await Task.Run(() =>
                {
                    UnlockFactor factor = new UnlockFactor { Password = password, RecoveryCode = code };
                    try
                    {
                        if (needKey) factor.Key = PromptKey(hwnd);

                        if (IsLogin)
                        {
                            List<LainAccount> list;
                            VaultKey k = CryLain.Unlock(factor, out list);
                            return Tuple.Create(k, list, k != null);
                        }
                        if (factor.Key != null) return Tuple.Create<VaultKey, List<LainAccount>, bool>(null, null, CryLain.Verify(factor, session));
                        return Tuple.Create<VaultKey, List<LainAccount>, bool>(null, null, CryLain.VerifyPassword(password, session));
                    }
                    finally
                    {
                        if (factor.Key != null)
                        {
                            ClearBytes(factor.Key.Secret);
                            ClearBytes(factor.Key.CredentialId);
                        }
                    }
                });
                key = r.Item1;
                accounts = r.Item2;
                ok = r.Item3;
            }
            catch (WebAuthn.WebAuthnException ex)
            {
                SetBusy(false);
                if (!ex.Cancelled) FluentMessageBox.Show(ex.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FocusInput();
                return;
            }
            catch (Exception ex)
            {
                SetBusy(false);
                FluentMessageBox.Show(ex.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            SetBusy(false);

            if (ok)
            {
                txtPassword.Text = string.Empty;
                txtRecovery.Text = string.Empty;

                switch (_type)
                {
                    case LoginType.Login:
                        Program.SetMainForm(new MainForm(key, accounts));
                        this.Close();
                        Program.ShowMainForm();
                        break;

                    default:
                        this.DialogResult = DialogResult.Yes;
                        this.Close();
                        break;
                }
            }
            else
            {
                lblError.Text = withKey ? "This security key doesn't unlock this vault."
                    : recovery ? "The password or recovery code is incorrect."
                    : needKey ? "The password or security key is incorrect."
                    : "That password is incorrect. Please try again.";
                lblError.Visible = true;
                FocusInput();
            }
        }

        private void FocusInput()
        {
            TextBox tb = recoveryBox.Visible && txtPassword.TextLength > 0 ? (TextBox)txtRecovery : txtPassword;
            tb.Focus();
            tb.SelectAll();
        }

        private void btnKey_Click(object sender, EventArgs e)
        {
            Login(true);
        }

        private void btnRecovery_Click(object sender, EventArgs e)
        {
            btnRecovery.Visible = false;
            recoveryBox.Visible = true;
            lblError.Visible = false;
            label1.Text = "Enter your master password and the recovery code you saved.";
            (txtPassword.TextLength > 0 ? (Control)txtRecovery : txtPassword).Focus();
        }

        private void LayoutKeyOptions()
        {
            bool keysUsable = WebAuthn.IsAvailable;
            bool showKey = _mode == KeyMode.Either && keysUsable;
            bool showRecovery = IsLogin && _mode == KeyMode.Both;

            btnKey.Visible = showKey;
            btnRecovery.Visible = showRecovery && keysUsable;
            // without WebAuthn the recovery code is the only second factor left
            recoveryBox.Visible = showRecovery && !keysUsable;

            if (IsLogin && _mode == KeyMode.Both)
            {
                label1.Text = keysUsable
                    ? "Enter your password, then touch your security key."
                    : "Enter your master password and your recovery code.";
            }

            if (!showKey && !showRecovery)
            {
                this.ClientSize = new System.Drawing.Size(this.ClientSize.Width, 294);
            }
        }
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            lblError.Visible = false;
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
            switch (_type)
            {
                case LoginType.Authorize:
                    btnExit.Text = "Cancel";
                    btnOk.Text = "Confirm";
                    status.Text = "Confirm it's you";
                    label1.Text = "Enter your master password to continue.";
                    break;

                case LoginType.Remove:
                    btnExit.Text = "Cancel";
                    btnOk.Text = "Delete";
                    status.Text = "Delete this account?";
                    label1.Text = "Enter your master password to confirm.";
                    break;

                case LoginType.RemoveAll:
                    btnExit.Text = "Cancel";
                    btnOk.Text = "Delete all";
                    status.Text = "Delete all accounts?";
                    label1.Text = "This can't be undone. Enter your master password to confirm.";
                    break;
            }

            if (_type == LoginType.Remove || _type == LoginType.RemoveAll)
            {
                lockIcon.Glyph = Fluent.Glyphs.Warning;
                lockIcon.GlyphColor = Fluent.Danger;
                lockIcon.UpdateColor();
            }

            try
            {
                _mode = CryLain.GetKeyMode();
                LayoutKeyOptions();
            }
            catch (Exception ex) when (ex is InvalidDataException || ex is IOException || ex is UnauthorizedAccessException)
            {
                _mode = KeyMode.Off;
                LayoutKeyOptions();
                lblError.Text = ex.Message;
                lblError.Visible = true;
                btnKey.Enabled = btnRecovery.Enabled = false;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            _showPassword = !_showPassword;
            txtPassword.UseSystemPasswordChar = !_showPassword;
            Fluent.SetEye(pictureBox1, _showPassword);
            txtPassword.Focus();
            txtPassword.SelectionStart = txtPassword.TextLength;
        }
    }
}
