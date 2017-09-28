using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;

namespace Lain
{
    public partial class WizardForm : Form
    {
        bool ShowPassword = false;

        public WizardForm()
        {
            InitializeComponent();
            Options.ApplyTheme(this);
        }

        private void PasswordVisibility()
        {
            if (ShowPassword)
            {
                pictureBox1.BackColor = Options.BackgroundColor;
                pictureBox2.BackColor = Options.BackgroundColor;
                txtPassword.UseSystemPasswordChar = true;
                txtVerify.UseSystemPasswordChar = true;
                ShowPassword = false;
                return;
            }
            else
            {
                pictureBox1.BackColor = Options.ForegroundColor;
                pictureBox2.BackColor = Options.ForegroundColor;
                txtPassword.UseSystemPasswordChar = false;
                txtVerify.UseSystemPasswordChar = false;
                ShowPassword = true;
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

        private void RestoreBackup()
        {
            if (MessageBox.Show("Do you really want restore a backup?", "Restore backup?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".Lain";
                dialog.Title = "Restore backup ...";
                dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
                dialog.FileName = string.Empty;
                dialog.Filter = "Lain files|*.Lain";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (Directory.Exists(Required.DataFolder))
                        {
                            Directory.Delete(Required.DataFolder, true);
                        }
                    }
                    catch { }
                    finally
                    {
                        try
                        {
                            ZipFile.ExtractToDirectory(dialog.FileName, Required.DataFolder);
                            Application.Restart();
                        }
                        catch { }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RestoreBackup();
        }
    }
}
