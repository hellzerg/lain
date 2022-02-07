using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lain
{
    public partial class OptionsForm : Form
    {
        MainForm _main;

        public OptionsForm(MainForm main)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            _main = main;

            LoadSettings();
            Options.ApplyTheme(this);

            txtTimer.ShortcutsEnabled = false;
        }

        private void RegisterAutoStart(bool remove)
        {
            try
            {
                using (RegistryKey k = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (!remove)
                    {
                        k.SetValue("Lain", Application.ExecutablePath);
                    }
                    else
                    {
                        k.DeleteValue("Lain", false);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Something went wrong. Please try running Lain as administrator!", "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void LoadSettings()
        {
            chkAlwaysPass.Checked = Options.CurrentOptions.Authorize;
            chkAutoLock.Checked = Options.CurrentOptions.AutoLock;
            txtTimer.Text = Options.CurrentOptions.Minutes.ToString();
            chkAutoStart.Checked = Options.CurrentOptions.AutoStart;
            chkHidePass.Checked = Options.CurrentOptions.HidePasswords;
            chkAutoSizeColumns.Checked = Options.CurrentOptions.AutoSizeColumns;

            RegisterAutoStart(!Options.CurrentOptions.AutoStart);

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

        private void FixColors()
        {
            _main.FixColors();
            if (chkAutoLock.Checked)
            {
                lblMins.ForeColor = Options.ForegroundColor;
                txtTimer.ForeColor = Options.ForegroundColor;
            }
            else
            {
                lblMins.ForeColor = Color.White;
                txtTimer.ForeColor = Color.White;
            }
        }

        private void oceantheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Azurite;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void magmatheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Ruby;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void zergtheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Amethyst;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void carameltheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Amber;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void limetheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Jade;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void minimaltheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Silver;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            FixColors();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Authorize = chkAlwaysPass.Checked;
        }

        private void txtTimer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            txtTimer.Enabled = chkAutoLock.Checked;
            Options.CurrentOptions.AutoLock = chkAutoLock.Checked;

            if (chkAutoLock.Checked)
            {
                lblMins.ForeColor = Options.ForegroundColor;
                lblMins.Font = new Font(lblMins.Font, FontStyle.Underline);
                txtTimer.ForeColor = Options.ForegroundColor;
            }

            if (!chkAutoLock.Checked)
            {
                lblMins.ForeColor = Color.White;
                lblMins.Font = new Font(lblMins.Font, FontStyle.Regular);
                txtTimer.ForeColor = Color.White;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.AutoStart = chkAutoStart.Checked;
            RegisterAutoStart(!Options.CurrentOptions.AutoStart);
        }

        private void txtTimer_TextChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtTimer.Text.Trim()) <= 0) Options.CurrentOptions.Minutes = 1;
            else Options.CurrentOptions.Minutes = Convert.ToInt32(txtTimer.Text);
        }

        private void rSmall_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.FontSize = 0;
            _main.SetFontSize();
        }

        private void rNormal_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.FontSize = 1;
            _main.SetFontSize();
        }

        private void rLarge_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.FontSize = 2;
            _main.SetFontSize();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.HidePasswords = chkHidePass.Checked;
            _main.AccountView.Columns[2].Visible = !chkHidePass.Checked;
            _main.LoadAccounts();
        }

        private void chkAutoSizeColumns_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.AutoSizeColumns = chkAutoSizeColumns.Checked;
            _main.SetAutoSizeColumns();
        }
    }
}
