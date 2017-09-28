using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lain
{
    public partial class NewForm : Form
    {
        CryLain cryLain = new CryLain();
        NewType _type;
        string _Name;

        bool ShowPassword = false;

        public NewForm(NewType type, string info = null)
        {
            InitializeComponent();
            Options.ApplyTheme(this);
            _type = type;
            _Name = info;

            switch (_type)
            {
                case NewType.Modify:
                    btnOk.Text = "Save";
                    if (!string.IsNullOrEmpty(info))
                    {
                        this.Text = string.Format("Modify account - {0}", info);
                    }
                    else
                    {
                        this.Text = "Create new account...";
                    }
                    
                    int i = MainForm.Accounts.FindIndex(x => x.Name() == cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), _Name));
                    
                    if (i > -1)
                    {
                        txtName.Text = cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Name());
                        txtMail.Text = cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Email());
                        txtPassword.Text = cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Password());
                        txtNote.Text = cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Note());
                    }

                    break;
            }
        }

        private void Save()
        {
            if (!string.IsNullOrEmpty(txtName.Text) && !string.IsNullOrEmpty(txtMail.Text) && !string.IsNullOrEmpty(txtPassword.Text))
            {
                switch (_type)
                {
                    case NewType.Modify:
                        int i = MainForm.Accounts.FindIndex(x => x.Name() == cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), _Name));
                        if (i > -1) { MainForm.Accounts.RemoveAt(i); }
                        break;
                }

                LainAccount account = new LainAccount(cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtName.Text), cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtMail.Text), cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtPassword.Text), cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtNote.Text));
                MainForm.Accounts.Add(account);

                txtName.Text = string.Empty;
                txtMail.Text = string.Empty;
                txtPassword.Text = string.Empty;
                txtNote.Text = string.Empty;
                account = null;

                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NewForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (ShowPassword)
            {
                pictureBox1.BackColor = Options.BackgroundColor;
                txtPassword.UseSystemPasswordChar = true;
                ShowPassword = false;
                return;
            }
            else
            {
                pictureBox1.BackColor = Options.ForegroundColor;
                txtPassword.UseSystemPasswordChar = false;
                ShowPassword = true;
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtPassword.Text = CryLain.GenerateRandomPassword(25);
        }
    }
}
