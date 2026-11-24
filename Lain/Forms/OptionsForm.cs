using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lain
{
    public sealed partial class OptionsForm : Form
    {
        MainForm _main;
        bool _loading;
        ContextMenuStrip _presetMenu;

        const string RunRegistryPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        static readonly string[][] SymbolPresets =
        {
            new[] { "Lain default", RandomPassword.DefaultSymbols },
            new[] { "Common", "!@#$%^&*" },
            new[] { "Safe for most sites", "!#$%&*+-=?@_" },
            new[] { "All ASCII symbols", "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~" },
        };

        public OptionsForm(MainForm main)
        {
            InitializeComponent();

            _main = main;

            timerBox.SetEditor(txtTimer);
            clipBox.SetEditor(txtClipSeconds);
            minLengthBox.SetEditor(txtMinLength);
            minPerTypeBox.SetEditor(txtMinPerType);
            symbolsBox.SetEditor(txtSymbols);
            Fluent.SetPlaceholder(txtSymbols, "None \u2014 generated passwords won't contain symbols");

            LoadSettings();
            Options.ApplyTheme(this);

            txtTimer.ShortcutsEnabled = false;
            txtClipSeconds.ShortcutsEnabled = false;
            txtMinLength.ShortcutsEnabled = false;
            txtMinPerType.ShortcutsEnabled = false;
            UpdateTimerState();
            UpdateClipState();
            UpdateGeneratorState();
            Fluent.StyleDialog(this);
            Shown += OptionsForm_Shown;
        }

        private void UpdateGeneratorState()
        {
            txtMinLength.Enabled = chkMinLength.Checked;
            lblMinLength.ForeColor = chkMinLength.Checked ? Fluent.Text : Fluent.TextDisabled;
            minLengthBox.Invalidate();
            txtMinPerType.Enabled = chkMinPerType.Checked;
            lblMinPerType.ForeColor = chkMinPerType.Checked ? Fluent.Text : Fluent.TextDisabled;
            minPerTypeBox.Invalidate();
        }

        private void UpdateTimerState()
        {
            txtTimer.Enabled = chkAutoLock.Checked;
            lblMins.ForeColor = chkAutoLock.Checked ? Fluent.Text : Fluent.TextDisabled;
            timerBox.Invalidate();
        }

        private void UpdateClipState()
        {
            txtClipSeconds.Enabled = chkClearClip.Checked;
            lblSecs.ForeColor = chkClearClip.Checked ? Fluent.Text : Fluent.TextDisabled;
            clipBox.Invalidate();
        }

        private static bool TrySetAutoStart(bool enabled, out string error)
        {
            try
            {
                if (enabled)
                {
                    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(RunRegistryPath))
                    {
                        if (key == null) throw new InvalidOperationException("The Windows startup registry key could not be opened.");

                        string command = "\"" + Application.ExecutablePath + "\"";
                        string current = key.GetValue("Lain", null) as string;
                        if (!string.Equals(current, command, StringComparison.OrdinalIgnoreCase))
                            key.SetValue("Lain", command, RegistryValueKind.String);
                    }
                }
                else
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunRegistryPath, true))
                    {
                        if (key != null) key.DeleteValue("Lain", false);
                    }
                }

                error = null;
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private void OptionsForm_Shown(object sender, EventArgs e)
        {
            string error;
            if (!TrySetAutoStart(Options.CurrentOptions.AutoStart, out error)) ShowAutoStartError(error);
        }

        private void ShowAutoStartError(string error)
        {
            FluentMessageBox.Show(this,
                "Windows couldn't update Lain's startup registration. The saved preference was not changed.\n\n" + error,
                "Lain", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void LoadSettings()
        {
            // values are applied silently; handlers only react to user changes
            _loading = true;
            try
            {
                LoadValues();
            }
            finally
            {
                _loading = false;
            }
        }

        private void LoadValues()
        {
            chkAlwaysPass.Checked = Options.CurrentOptions.Authorize;
            chkAutoLock.Checked = Options.CurrentOptions.AutoLock;
            txtTimer.Text = Options.CurrentOptions.Minutes.ToString();
            chkAutoStart.Checked = Options.CurrentOptions.AutoStart;
            chkHidePass.Checked = Options.CurrentOptions.HidePasswords;
            chkAutoSizeColumns.Checked = Options.CurrentOptions.AutoSizeColumns;
            chkShowCreated.Checked = Options.CurrentOptions.ShowCreatedColumn;
            chkShowModified.Checked = Options.CurrentOptions.ShowModifiedColumn;
            if (Options.CurrentOptions.CompactRows) rCompact.Checked = true; else rComfortable.Checked = true;
            chkClearClip.Checked = Options.CurrentOptions.ClearClipboard;
            txtClipSeconds.Text = Options.CurrentOptions.ClipboardSeconds.ToString();
            chkMinLength.Checked = Options.CurrentOptions.EnforceMinLength;
            txtMinLength.Text = Options.CurrentOptions.GeneratorMinLength.ToString();
            chkMinPerType.Checked = Options.CurrentOptions.EnforceMinPerType;
            txtMinPerType.Text = Options.CurrentOptions.GeneratorMinPerType.ToString();
            txtSymbols.Text = Options.CurrentOptions.GeneratorSymbols;

            rSystem.Checked = Options.CurrentOptions.ThemeMode == ThemeMode.System;
            rLight.Checked = Options.CurrentOptions.ThemeMode == ThemeMode.Light;
            rDark.Checked = Options.CurrentOptions.ThemeMode == ThemeMode.Dark;

            switch (Options.CurrentOptions.Color)
            {
                case Theme.Amber:
                    rAmber.Checked = true;
                    break;
                case Theme.Jade:
                    rJade.Checked = true;
                    break;
                case Theme.Ruby:
                    rRuby.Checked = true;
                    break;
                case Theme.Silver:
                    rSilver.Checked = true;
                    break;
                case Theme.Azurite:
                    rAzurite.Checked = true;
                    break;
                case Theme.Amethyst:
                    rAmethyst.Checked = true;
                    break;
            }

            switch (Options.CurrentOptions.FontSize)
            {
                case 0:
                    rSmall.Checked = true;
                    break;
                case 1:
                    rNormal.Checked = true;
                    break;
                case 2:
                    rLarge.Checked = true;
                    break;
            }
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Options.SaveSettings();
            this.Close();
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            using (ChangePasswordForm f = new ChangePasswordForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK) _main.ShowToast("Master password changed");
            }
        }

        private void btnSecurityKeys_Click(object sender, EventArgs e)
        {
            using (SecurityKeysForm f = new SecurityKeysForm()) f.ShowDialog(this);
        }

        private void FixColors()
        {
            _main.FixColors();
        }

        private void appearance_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;

            Options.CurrentOptions.ThemeMode = rSystem.Checked ? ThemeMode.System
                : rLight.Checked ? ThemeMode.Light
                : ThemeMode.Dark;

            Fluent.RefreshPalette();
            try
            {
                Options.ApplyTheme(this);
                _main.ApplyCurrentTheme();
            }
            finally
            {
                Fluent.CompletePaletteChange();
            }
        }

        private void oceantheme_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.Color = Theme.Azurite;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void magmatheme_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.Color = Theme.Ruby;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void zergtheme_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.Color = Theme.Amethyst;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void carameltheme_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.Color = Theme.Amber;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void limetheme_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.Color = Theme.Jade;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void minimaltheme_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.Color = Theme.Silver;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Options.CurrentOptions.Authorize = chkAlwaysPass.Checked;
        }

        private void txtTimer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Options.CurrentOptions.AutoLock = chkAutoLock.Checked;
            UpdateTimerState();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;

            bool previous = Options.CurrentOptions.AutoStart;
            bool requested = chkAutoStart.Checked;
            string error;
            if (TrySetAutoStart(requested, out error))
            {
                Options.CurrentOptions.AutoStart = requested;
                return;
            }

            _loading = true;
            try { chkAutoStart.Checked = previous; }
            finally { _loading = false; }
            ShowAutoStartError(error);
        }

        private void txtTimer_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            int minutes;
            if (!int.TryParse(txtTimer.Text.Trim(), out minutes) || minutes <= 0) Options.CurrentOptions.Minutes = 1;
            else Options.CurrentOptions.Minutes = minutes;
        }

        private void rSmall_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.FontSize = 0;
            _main.SetFontSize();
        }

        private void rNormal_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.FontSize = 1;
            _main.SetFontSize();
        }

        private void rLarge_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.FontSize = 2;
            _main.SetFontSize();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Options.CurrentOptions.HidePasswords = chkHidePass.Checked;
            _main.AccountView.Columns[2].Visible = !chkHidePass.Checked;
            _main.LoadAccounts();
        }

        private void chkClearClip_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Options.CurrentOptions.ClearClipboard = chkClearClip.Checked;
            UpdateClipState();
        }

        private void txtClipSeconds_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            int seconds;
            if (int.TryParse(txtClipSeconds.Text.Trim(), out seconds))
                Options.CurrentOptions.ClipboardSeconds = SecureClipboard.ClampSeconds(seconds);
        }

        private void txtClipSeconds_Leave(object sender, EventArgs e)
        {
            // show the value actually in effect (e.g. "2" becomes the 5 second minimum)
            txtClipSeconds.Text = Options.CurrentOptions.ClipboardSeconds.ToString();
        }

        private void chkAutoSizeColumns_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Options.CurrentOptions.AutoSizeColumns = chkAutoSizeColumns.Checked;
            _main.SetAutoSizeColumns();
        }

        private void chkShowDates_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Options.CurrentOptions.ShowCreatedColumn = chkShowCreated.Checked;
            Options.CurrentOptions.ShowModifiedColumn = chkShowModified.Checked;
            _main.ApplyDateColumns();
        }

        private void rSpacing_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading || !((RadioButton)sender).Checked) return;
            Options.CurrentOptions.CompactRows = rCompact.Checked;
            _main.SetFontSize();
        }

        private void chkGenerator_CheckedChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            Options.CurrentOptions.EnforceMinLength = chkMinLength.Checked;
            Options.CurrentOptions.EnforceMinPerType = chkMinPerType.Checked;
            UpdateGeneratorState();
        }

        private void txtGenerator_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            int value;
            if (int.TryParse(txtMinLength.Text.Trim(), out value))
                Options.CurrentOptions.GeneratorMinLength = RandomPassword.ClampMinLength(value);
            if (int.TryParse(txtMinPerType.Text.Trim(), out value))
                Options.CurrentOptions.GeneratorMinPerType = RandomPassword.ClampMinPerType(value);
        }

        private void txtGenerator_Leave(object sender, EventArgs e)
        {
            txtMinLength.Text = Options.CurrentOptions.GeneratorMinLength.ToString();
            txtMinPerType.Text = Options.CurrentOptions.GeneratorMinPerType.ToString();
        }

        private void SetSymbols(string value)
        {
            string symbols = RandomPassword.SanitizeSymbols(value);
            Options.CurrentOptions.GeneratorSymbols = symbols;
            RandomPassword.Symbols = symbols;
        }

        private void txtSymbols_TextChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            SetSymbols(txtSymbols.Text);
        }

        // Shows what will actually be used (letters, digits, spaces and duplicates are dropped)
        private void txtSymbols_Leave(object sender, EventArgs e)
        {
            if (txtSymbols.Text != Options.CurrentOptions.GeneratorSymbols) txtSymbols.Text = Options.CurrentOptions.GeneratorSymbols;
        }

        private void btnSymbolPresets_Click(object sender, EventArgs e)
        {
            if (_presetMenu == null)
            {
                _presetMenu = new ContextMenuStrip();
                FormClosed += (s, a) => _presetMenu.Dispose();
            }
            Fluent.StyleMenu(_presetMenu);
            _presetMenu.Items.Clear();

            string current = Options.CurrentOptions.GeneratorSymbols;
            foreach (string[] preset in SymbolPresets)
            {
                string symbols = preset[1];
                bool active = current == symbols;
                ToolStripMenuItem item = new ToolStripMenuItem(preset[0]);
                item.ShortcutKeyDisplayString = symbols;
                Fluent.SetMenuGlyph(item, active ? Fluent.Glyphs.Checkmark : string.Empty, DeviceDpi);
                if (active) item.ForeColor = Fluent.Accent;
                item.Click += (s, a) =>
                {
                    txtSymbols.Text = symbols;
                    txtSymbols.SelectionStart = txtSymbols.TextLength;
                };
                _presetMenu.Items.Add(item);
            }

            Fluent.StyleMenuItems(_presetMenu);
            _presetMenu.Show(btnSymbolPresets, new Point(0, btnSymbolPresets.Height + 4));
        }
    }
}
