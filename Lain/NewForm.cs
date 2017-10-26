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
        CryLain _cryLain = new CryLain();

        NewType _type;
        string _name;

        bool _showPassword = false;

        internal NewForm(NewType type, string info = null)
        {
            InitializeComponent();
            Options.ApplyTheme(this);
            _type = type;
            _name = info;

            switch (_type)
            {
                case NewType.Modify:
                    btnOk.Text = "Save";
                    if (!string.IsNullOrEmpty(info))
                    {
                        this.Text = string.Format("Edit account - {0}", info);
                    }
                    else
                    {
                        this.Text = "Add new account...";
                    }
                    
                    int i = MainForm.Accounts.FindIndex(x => x.Name() == _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), _name));
                    
                    if (i > -1)
                    {
                        txtName.Text = _cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Name());
                        txtMail.Text = _cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Email());
                        txtPassword.Text = _cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Password());
                        txtNote.Text = _cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Note());
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
                        int i = MainForm.Accounts.FindIndex(x => x.Name() == _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), _name));
                        if (i > -1) { MainForm.Accounts.RemoveAt(i); }
                        break;
                }

                LainAccount account = new LainAccount(_cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtName.Text), _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtMail.Text), _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtPassword.Text), _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtNote.Text));
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
            if (_showPassword)
            {
                pictureBox1.BackColor = Options.BackgroundColor;
                txtPassword.UseSystemPasswordChar = true;
                _showPassword = false;
                return;
            }
            else
            {
                pictureBox1.BackColor = Options.ForegroundColor;
                txtPassword.UseSystemPasswordChar = false;
                _showPassword = true;
                return;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            txtPassword.Text = CryLain.GenerateRandomPassword(32);
        }
    }
}
