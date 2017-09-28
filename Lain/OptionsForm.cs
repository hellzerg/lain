using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;
using System.IO.Compression;

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
                RegistryKey k = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                if (!remove)
                {
                    k.SetValue("Lain", Application.ExecutablePath);
                }
                else
                {
                    k.DeleteValue("Lain", false);
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
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
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
        }

        private void magmatheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Magma;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
        }

        private void zergtheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Zerg;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
        }

        private void carameltheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Caramel;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
        }

        private void limetheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Lime;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
        }

        private void minimaltheme_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Color = Theme.Minimal;
            Options.ApplyTheme(this);
            Options.ApplyTheme(_main);
            _main.FixColor();
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
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.AutoStart = checkBox3.Checked;
            RegisterAutoStart(!Options.CurrentOptions.AutoStart);
        }

        private void txtTimer_TextChanged(object sender, EventArgs e)
        {
            Options.CurrentOptions.Minutes = Convert.ToInt32(txtTimer.Text);
        }

        private void CreateBackup()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".Lain";
            dialog.Title = "Create backup ...";
            dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            dialog.FileName = "Backup";
            dialog.Filter = "All files|*.*";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (Directory.Exists(Required.DataFolder))
                    {
                        ZipFile.CreateFromDirectory(Required.DataFolder, dialog.FileName, CompressionLevel.Optimal, false);
                    }
                }
                catch { }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateBackup();
        }
    }
}
