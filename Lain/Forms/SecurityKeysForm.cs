using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lain
{
    // Manages FIDO2 security keys: unlock mode, registered keys and the recovery code.
    [DesignerCategory("Code")]
    public sealed class SecurityKeysForm : Form
    {
        const int Width96 = 484;
        const string DefaultKeyName = "Security key";

        readonly ModernButton[] _modes = new ModernButton[3];
        readonly FluentLabel _hint;
        readonly ModernButton _add;
        readonly Card _list;
        readonly FluentLabel _recovery;
        readonly ModernButton _regenerate;
        readonly InputBox _passwordBox;
        readonly FluentTextBox _password;
        readonly IconLabel _eye;
        readonly FluentLabel _message;
        readonly ModernButton _done;

        KeyMode _mode;
        List<SecurityKeyInfo> _keys = new List<SecurityKeyInfo>();
        bool _busy;
        bool _showPassword;
        bool _keyConfigurationError;

        public SecurityKeysForm()
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = Fluent.Ui(10F);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Security keys";

            Controls.Add(new IconLabel { Glyph = Fluent.Glyphs.Key, IconSize = 18F, Active = true, Location = new Point(24, 18), Size = new Size(36, 36) });
            Controls.Add(new FluentLabel { AutoSize = true, Font = Fluent.Display(16F), Text = "Security keys", Location = new Point(62, 20) });
            Controls.Add(new FluentLabel
            {
                ForeColor = Fluent.TextSecondary,
                Text = "New keys must support ES256/P-256, user verification (PIN or biometric), and the hmac-secret extension.",
                Location = new Point(30, 64),
                Size = new Size(424, 42)
            });

            Controls.Add(new FluentLabel { AutoSize = true, Font = Fluent.UiSemibold(10F), Text = "Unlock with", Location = new Point(29, 116) });
            string[] modeNames = { "Password only", "Password or key", "Password and key" };
            for (int i = 0; i < 3; i++)
            {
                KeyMode target = (KeyMode)i;
                _modes[i] = new ModernButton { Text = modeNames[i], Location = new Point(30 + i * 144, 140), Size = new Size(136, 34), TabIndex = i };
                _modes[i].Click += (s, e) => ChangeMode(target);
                Controls.Add(_modes[i]);
            }
            _hint = new FluentLabel { ForeColor = Fluent.TextSecondary, Location = new Point(30, 182), Size = new Size(424, 42) };
            Controls.Add(_hint);

            Controls.Add(new FluentLabel { AutoSize = true, Font = Fluent.UiSemibold(10F), Text = "Your keys", Location = new Point(29, 238) });
            _add = new ModernButton { Text = "Add key", Glyph = Fluent.Glyphs.Add, Variant = ButtonVariant.Subtle, Location = new Point(334, 231), Size = new Size(120, 32), TabIndex = 3 };
            _add.Click += (s, e) => AddKey();
            Controls.Add(_add);

            _list = new Card { Location = new Point(30, 268), Size = new Size(424, 150), AutoScroll = true, TabIndex = 4 };
            Controls.Add(_list);

            _recovery = new FluentLabel { ForeColor = Fluent.TextSecondary, Text = "Lost all your keys? Use the recovery code.", Location = new Point(30, 432), Size = new Size(280, 22) };
            Controls.Add(_recovery);
            _regenerate = new ModernButton { Text = "New recovery code", Glyph = Fluent.Glyphs.Refresh, Variant = ButtonVariant.Subtle, Location = new Point(294, 426), Size = new Size(160, 32), TabIndex = 5 };
            _regenerate.Click += (s, e) => RegenerateRecovery();
            Controls.Add(_regenerate);

            Controls.Add(new FluentLabel { AutoSize = true, Font = Fluent.UiSemibold(10F), Text = "Master password", Location = new Point(29, 472) });
            _passwordBox = new InputBox { Location = new Point(30, 496), Size = new Size(424, 38), TabIndex = 6 };
            _password = new FluentTextBox { Font = Fluent.Ui(11.5F), Size = new Size(360, 20), UseSystemPasswordChar = true };
            _eye = new IconLabel { Clickable = true, Glyph = Fluent.Glyphs.View, Size = new Size(32, 32) };
            _passwordBox.SetEditor(_password);
            _passwordBox.AddTrailing(_eye);
            Fluent.SetPlaceholder(_password, "Needed to confirm changes");
            Fluent.SetEye(_eye, false);
            _eye.Click += (s, e) =>
            {
                _showPassword = !_showPassword;
                _password.UseSystemPasswordChar = !_showPassword;
                Fluent.SetEye(_eye, _showPassword);
            };
            _password.TextChanged += (s, e) => { if (!_busy) ShowMessage(string.Empty, Fluent.TextSecondary); };
            Controls.Add(_passwordBox);

            _message = new FluentLabel { Location = new Point(30, 542), Size = new Size(424, 42) };
            Controls.Add(_message);

            FooterPanel footer = new FooterPanel { Dock = DockStyle.Bottom, Height = 68 };
            _done = new ModernButton { Text = "Done", Variant = ButtonVariant.Primary, DialogResult = DialogResult.OK, Location = new Point(344, 16), Size = new Size(110, 36), TabIndex = 0 };
            footer.Controls.Add(_done);
            Controls.Add(footer);

            ClientSize = new Size(Width96, 592 + 68);
            CancelButton = _done;
            ResumeLayout(false);
            PerformLayout();

            Options.ApplyTheme(this);
            Fluent.StyleDialog(this);

            FormClosing += (s, e) => { if (_busy && e.CloseReason == CloseReason.UserClosing) e.Cancel = true; };
            FormClosed += (s, e) => _password.Text = string.Empty;
            Load += (s, e) => RefreshState();
        }

        #region State

        private void RefreshState()
        {
            try
            {
                _mode = CryLain.GetKeyMode();
                _keys = CryLain.GetSecurityKeys();
                _keyConfigurationError = false;
            }
            catch (Exception ex) when (ex is InvalidDataException || ex is IOException || ex is UnauthorizedAccessException)
            {
                _keyConfigurationError = true;
                _mode = KeyMode.Off;
                _keys = new List<SecurityKeyInfo>();
                for (int i = 0; i < _modes.Length; i++) _modes[i].Enabled = false;
                _add.Enabled = false;
                _recovery.Visible = _regenerate.Visible = false;
                _regenerate.Enabled = false;
                _passwordBox.Enabled = false;
                _hint.Text = "Security-key configuration cannot be loaded safely.";
                ShowMessage(ex.Message, Fluent.Danger);
                BuildList();
                return;
            }

            bool available = WebAuthn.IsAvailable;

            for (int i = 0; i < 3; i++)
            {
                _modes[i].Variant = (int)_mode == i ? ButtonVariant.Primary : ButtonVariant.Secondary;
                // turning keys on needs a key; switching between the on modes doesn't
                _modes[i].Enabled = !_keyConfigurationError && !_busy && ((int)_mode == i || i == 0 || _mode != KeyMode.Off || available);
                _modes[i].Invalidate();
            }

            switch (_mode)
            {
                case KeyMode.Either:
                    _hint.Text = "Your master password or any of your security keys unlocks the vault.";
                    break;
                case KeyMode.Both:
                    _hint.Text = "Unlocking needs your master password and a security key. The recovery code can stand in for a lost key.";
                    break;
                default:
                    _hint.Text = available
                        ? "Only your master password unlocks the vault. Choose a mode above to add a security key."
                        : WebAuthn.UnavailableReason;
                    break;
            }

            _add.Enabled = !_keyConfigurationError && !_busy && available && _mode != KeyMode.Off && _keys.Count < CryLain.MaxSecurityKeys;
            _recovery.Visible = _regenerate.Visible = _mode == KeyMode.Both;
            _regenerate.Enabled = !_keyConfigurationError && !_busy;
            _passwordBox.Enabled = !_keyConfigurationError && !_busy;
            _done.Enabled = !_busy;
            BuildList();
        }

        private void BuildList()
        {
            _list.SuspendLayout();
            while (_list.Controls.Count > 0) _list.Controls[0].Dispose();

            int pad = Fluent.Scale(this, 8);
            int rowHeight = Fluent.Scale(this, 46);
            int width = _list.ClientSize.Width - pad * 2 - (_keys.Count * rowHeight + pad * 2 > _list.ClientSize.Height ? SystemInformation.VerticalScrollBarWidth : 0);

            if (_keys.Count == 0)
            {
                _list.Controls.Add(new FluentLabel
                {
                    AutoSize = false,
                    BackColor = Fluent.Surface,
                    ForeColor = Fluent.TextSecondary,
                    Text = _keyConfigurationError ? "The security-key configuration is invalid or unsupported."
                        : (_mode == KeyMode.Off ? "Security keys are off." : "No security keys found."),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Bounds = new Rectangle(pad, pad, _list.ClientSize.Width - pad * 2, _list.ClientSize.Height - pad * 2)
                });
            }

            int y = pad;
            int icon = Fluent.Scale(this, 32);
            foreach (SecurityKeyInfo key in _keys)
            {
                SecurityKeyInfo k = key;
                Panel row = new Panel { BackColor = Fluent.Surface, Bounds = new Rectangle(pad, y, width, rowHeight) };

                row.Controls.Add(new IconLabel { Glyph = Fluent.Glyphs.Key, IconSize = 14F, BackColor = Fluent.Surface, Bounds = new Rectangle(0, (rowHeight - icon) / 2, icon, icon) });
                int textLeft = icon + Fluent.Scale(this, 6);
                int textWidth = width - textLeft - icon * 2 - Fluent.Scale(this, 4);
                row.Controls.Add(new FluentLabel
                {
                    AutoSize = false,
                    AutoEllipsis = true,
                    UseMnemonic = false,
                    BackColor = Fluent.Surface,
                    Font = Fluent.UiSemibold(10F),
                    Text = k.Name,
                    Bounds = new Rectangle(textLeft, Fluent.Scale(this, 3), textWidth, Fluent.Scale(this, 21))
                });
                row.Controls.Add(new FluentLabel
                {
                    AutoSize = false,
                    AutoEllipsis = true,
                    BackColor = Fluent.Surface,
                    ForeColor = Fluent.TextSecondary,
                    Font = Fluent.Ui(9F),
                    Text = "Added " + k.Added.ToLocalTime().ToString("d MMM yyyy")
                        + " · ES256",
                    Bounds = new Rectangle(textLeft, Fluent.Scale(this, 24), textWidth, Fluent.Scale(this, 18))
                });

                IconLabel rename = new IconLabel { Glyph = Fluent.Glyphs.Edit, IconSize = 12F, Clickable = true, BackColor = Fluent.Surface, Enabled = !_busy, Bounds = new Rectangle(width - icon * 2, (rowHeight - icon) / 2, icon, icon) };
                rename.Click += (s, e) => RenameKey(k);
                row.Controls.Add(rename);

                IconLabel remove = new IconLabel { Glyph = Fluent.Glyphs.Delete, IconSize = 12F, Clickable = true, BackColor = Fluent.Surface, Enabled = !_busy && _keys.Count > 1, Bounds = new Rectangle(width - icon, (rowHeight - icon) / 2, icon, icon) };
                remove.Click += (s, e) => RemoveKey(k);
                row.Controls.Add(remove);

                ToolTip tips = new ToolTip();
                tips.SetToolTip(rename, "Rename");
                tips.SetToolTip(remove, _keys.Count > 1 ? "Remove" : "The last key can't be removed");
                row.Disposed += (s, e) => tips.Dispose();

                _list.Controls.Add(row);
                y += rowHeight;
            }
            _list.ResumeLayout(true);
        }

        private void ShowMessage(string text, Color color)
        {
            _message.ForeColor = color;
            _message.Text = text;
        }

        private void SetBusy(bool busy, string status = null)
        {
            _busy = busy;
            UseWaitCursor = busy;
            if (status != null) ShowMessage(status, Fluent.TextSecondary);
            RefreshState();
        }

        // The current password, or null (with a prompt) when it is empty.
        private string RequirePassword()
        {
            if (_password.TextLength > 0) return _password.Text;
            ShowMessage("Enter your master password to confirm this change.", Fluent.Caution);
            _password.Focus();
            return null;
        }

        private static List<byte[]> CredentialIds(List<SecurityKeyInfo> keys)
        {
            List<byte[]> ids = new List<byte[]>(keys.Count);
            foreach (SecurityKeyInfo k in keys) ids.Add(k.CredentialId);
            return ids;
        }

        #endregion

        #region Operations

        // Runs an operation off the UI thread with busy state; returns false (after reporting) when it failed.
        private async Task<bool> Run(string status, Action work)
        {
            SetBusy(true, status);
            try
            {
                await Task.Run(work);
                SetBusy(false, string.Empty);
                return true;
            }
            catch (WebAuthn.WebAuthnException ex)
            {
                SetBusy(false, string.Empty);
                ShowMessage(ex.Cancelled ? "Cancelled." : ex.Message, ex.Cancelled ? Fluent.TextSecondary : Fluent.Danger);
            }
            catch (Exception ex)
            {
                SetBusy(false, string.Empty);
                ShowMessage(ex.Message, Fluent.Danger);
                if (ex.Message.IndexOf("password", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _password.Focus();
                    _password.SelectAll();
                }
            }
            return false;
        }

        private bool CheckSession(out VaultKey session)
        {
            session = MainForm.Key;
            if (session != null) return true;
            Close();
            return false;
        }

        // Registers a credential and reads its PRF secret: one touch when Windows and the key can evaluate the PRF during
        // registration, otherwise a second touch to read it. Fails early on a wrong password.
        private static void Enroll(IntPtr hwnd, VaultKey session, string password, string name, List<byte[]> exclude,
            out WebAuthn.Credential credential, out byte[] prf, out byte[] salt)
        {
            if (!CryLain.VerifyPassword(password, session)) throw new InvalidOperationException("The master password is incorrect.");
            salt = CryLain.NewPrfSalt();
            credential = WebAuthn.Register(hwnd, name, exclude, salt, out prf);
            if (prf == null) prf = WebAuthn.GetSecret(hwnd, new[] { credential }).Secret;
        }

        private static string EnrollPrompt
        {
            get
            {
                return WebAuthn.CanReadSecretOnRegister
                    ? "Follow the Windows prompts and touch your key to register it (some keys ask for a second touch)\u2026"
                    : "Follow the Windows prompts: touch your key to register it, then once more to confirm\u2026";
            }
        }

        private async void ChangeMode(KeyMode target)
        {
            if (_busy || target == _mode) return;
            VaultKey session;
            if (!CheckSession(out session)) return;
            string password = RequirePassword();
            if (password == null) return;

            IntPtr hwnd = Handle;
            KeyMode from = _mode;

            if (from == KeyMode.Off)
            {
                string name = FluentPrompt.AskText(this, "Name your key", "A name helps you tell your keys apart, e.g. \"YubiKey on keychain\".", DefaultKeyName, "Key name", 60, "Continue");
                if (name == null) return;

                List<LainAccount> snapshot = new List<LainAccount>(MainForm.Accounts);
                string code = null;
                VaultKey newKey = null;
                List<LainAccount> accounts = null;
                bool ok = await Run(EnrollPrompt, () =>
                {
                    WebAuthn.Credential credential;
                    byte[] prf, salt;
                    Enroll(hwnd, session, password, name, new List<byte[]>(), out credential, out prf, out salt);
                    try
                    {
                        newKey = CryLain.EnableSecurityKeys(session, snapshot, password, target, name, credential, prf, salt, out code, out accounts);
                    }
                    finally
                    {
                        Array.Clear(prf, 0, prf.Length);
                    }
                });
                if (!ok) return;
                MainForm.ReplaceVault(session, newKey, accounts);
                if (code != null)
                {
                    try { FluentPrompt.ShowRecoveryCode(this, code); }
                    finally { code = null; }
                }
                ShowMessage("\u2713  Security keys are on.", Fluent.Success);
            }
            else if (target == KeyMode.Off)
            {
                if (FluentMessageBox.Show(this, "Your vault will be re-encrypted with the master password only, and your registered security keys will be forgotten.",
                    "Turn off security keys?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, "Turn off", "Cancel") != DialogResult.Yes) return;

                List<LainAccount> snapshot = new List<LainAccount>(MainForm.Accounts);
                VaultKey newKey = null;
                List<LainAccount> accounts = null;
                bool ok = await Run("Re-encrypting your vault\u2026", () => newKey = CryLain.DisableSecurityKeys(session, snapshot, password, out accounts));
                if (!ok) return;
                MainForm.ReplaceVault(session, newKey, accounts);
                ShowMessage("\u2713  Security keys are off. Only your master password unlocks the vault.", Fluent.Success);
            }
            else
            {
                string code = null;
                bool ok = await Run("Updating\u2026", () => code = CryLain.SetKeyMode(session, password, target));
                if (!ok) return;
                if (code != null)
                {
                    try { FluentPrompt.ShowRecoveryCode(this, code); }
                    finally { code = null; }
                }
                ShowMessage(target == KeyMode.Both ? "\u2713  Your password and a security key are now both needed." : "\u2713  Your password or a security key now unlocks the vault.", Fluent.Success);
            }
        }

        private async void AddKey()
        {
            if (_busy) return;
            VaultKey session;
            if (!CheckSession(out session)) return;
            string password = RequirePassword();
            if (password == null) return;

            string name = FluentPrompt.AskText(this, "Name your key", "A name helps you tell your keys apart, e.g. \"Backup YubiKey\".", DefaultKeyName + " " + (_keys.Count + 1), "Key name", 60, "Continue");
            if (name == null) return;

            IntPtr hwnd = Handle;
            List<byte[]> exclude = CredentialIds(_keys);
            bool ok = await Run(EnrollPrompt, () =>
            {
                WebAuthn.Credential credential;
                byte[] prf, salt;
                Enroll(hwnd, session, password, name, exclude, out credential, out prf, out salt);
                try
                {
                    CryLain.AddSecurityKey(session, password, name, credential, prf, salt);
                }
                finally
                {
                    Array.Clear(prf, 0, prf.Length);
                }
            });
            if (ok) ShowMessage("\u2713  \u201C" + name + "\u201D was added.", Fluent.Success);
        }

        private async void RemoveKey(SecurityKeyInfo key)
        {
            if (_busy) return;
            VaultKey session;
            if (!CheckSession(out session)) return;
            string password = RequirePassword();
            if (password == null) return;

            if (FluentMessageBox.Show(this, "\u201C" + key.Name + "\u201D will no longer unlock this vault.", "Remove this key?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, "Remove", "Cancel", true) != DialogResult.Yes) return;

            bool ok = await Run("Removing\u2026", () => CryLain.RemoveSecurityKey(session, password, key.CredentialId));
            if (ok) ShowMessage("\u2713  \u201C" + key.Name + "\u201D was removed.", Fluent.Success);
        }

        private async void RenameKey(SecurityKeyInfo key)
        {
            if (_busy) return;
            VaultKey session;
            if (!CheckSession(out session)) return;

            string name = FluentPrompt.AskText(this, "Rename key", "Choose a new name for this security key.", key.Name, "Key name", 60);
            if (name == null || name == key.Name) return;

            await Run("Saving\u2026", () => CryLain.RenameSecurityKey(session, key.CredentialId, name));
        }

        private async void RegenerateRecovery()
        {
            if (_busy) return;
            VaultKey session;
            if (!CheckSession(out session)) return;
            string password = RequirePassword();
            if (password == null) return;

            if (FluentMessageBox.Show(this, "Your current recovery code will stop working.", "Make a new recovery code?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Continue", "Cancel") != DialogResult.Yes) return;

            string code = null;
            bool ok = await Run("Updating\u2026", () => code = CryLain.RegenerateRecoveryCode(session, password));
            if (!ok) return;
            try { FluentPrompt.ShowRecoveryCode(this, code); }
            finally { code = null; }
            ShowMessage("\u2713  Your new recovery code is active.", Fluent.Success);
        }

        #endregion
    }
}
