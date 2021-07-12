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
            _main = main;

            LoadSettings();
            Options.ApplyTheme(this);
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
            checkBox1.Checked = Options.CurrentOptions.Authorize;
            checkBox2.Checked = Options.CurrentOptions.AutoLock;
            txtTimer.Text = Options.CurrentOptions.Minutes.ToString();
            checkBox3.Checked = Options.CurrentOptions.AutoStart;
            checkBox4.Checked = Options.CurrentOptions.HidePasswords;

            RegisterAutoStart(!Options.CurrentOptions.AutoStart);

            switch (Options.CurrentOptions.Color)
            {
                case Theme.Caramel:
                    carameltheme.Checked = true;
                    break;
                case Theme.Lime:
                    limetheme.Checked = true;
                    break;
                case Theme.Magma:
                    magmatheme.Checked = true;
                    break;
                case Theme.Minimal:
                    minimaltheme.Checked = true;
                    break;
                case Theme.Ocean:
                    oceantheme.Checked = true;
                    break;
                case Theme.Zerg:
                    zergtheme.Checked = true;
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
            CheckForIllegalCrossThreadCalls = false;
            txtTimer.ShortcutsEnabled = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void oceantheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Ocean;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
            label1.ForeColor = Options.ForegroundColor;
        }

        private void magmatheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Magma;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
            label1.ForeColor = Options.ForegroundColor;
        }

        private void zergtheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Zerg;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
            label1.ForeColor = Options.ForegroundColor;
        }

        private void carameltheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Caramel;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
            label1.ForeColor = Options.ForegroundColor;
        }

        private void limetheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Lime;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
            label1.ForeColor = Options.ForegroundColor;
        }

        private void minimaltheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Minimal;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
            label1.ForeColor = Options.ForegroundColor;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Authorize = checkBox1.Checked;
        }

        private void txtTimer_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            txtTimer.Enabled = checkBox2.Checked;
            Options.CurrentOptions.AutoLock = checkBox2.Checked;

            if (checkBox2.Checked)
            {
                label1.ForeColor = Options.ForegroundColor;
                label1.Font = new Font(label1.Font, FontStyle.Underline);
            }

            if (!checkBox2.Checked)
            {
                label1.ForeColor = Color.White;
                label1.Font = new Font(label1.Font, FontStyle.Regular);
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.AutoStart = checkBox3.Checked;
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
            Options.CurrentOptions.HidePasswords = checkBox4.Checked;
            _main.AccountView.Columns[2].Visible = !checkBox4.Checked;
        }
    }
}
