using System;
using System.Windows.Forms;

namespace Lain
{
    public partial class NewForm : Form
    {
        CryLain _cryLain = new CryLain();

        NewType _type;
        string _name;

        bool _showPassword = false;

        readonly int DEFAULT_RANDOM_CHARACTERS = 32;

        internal NewForm(NewType type, string info = null)
        {
            InitializeComponent();
            Options.ApplyTheme(this);

            _type = type;
            _name = info;

            chkLowers.Checked = true;
            chkUppers.Checked = true;
            chkSpecials.Checked = true;
            chkNums.Checked = true;

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
                        txtLink.Text = _cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), MainForm.Accounts[i].Link());
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

                LainAccount account = new LainAccount(
                    _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtName.Text),
                    _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtMail.Text),
                    _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtPassword.Text),
                    _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtLink.Text),
                    _cryLain.Encrypt(CryLain.ToInsecureString(MainForm.Key), txtNote.Text)
                );

                MainForm.Accounts.Add(account);

                txtName.Text = string.Empty;
                txtMail.Text = string.Empty;
                txtPassword.Text = string.Empty;
                txtLink.Text = string.Empty;
                txtNote.Text = string.Empty;
                account = null;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void NewForm_Load(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;

            txtMaxRandomChars.ShortcutsEnabled = false;
            txtMaxRandomChars.Text = DEFAULT_RANDOM_CHARACTERS.ToString();
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

        private void btnGenerateRandom_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaxRandomChars.Text) ||
                Convert.ToInt32(txtMaxRandomChars.Text) <= 0)
            {
                txtMaxRandomChars.Text = DEFAULT_RANDOM_CHARACTERS.ToString();
            }

            RandomPassword.WithLowerChars = chkLowers.Checked;
            RandomPassword.WithUpperChars = chkUppers.Checked;
            RandomPassword.WithSpecialChars = chkSpecials.Checked;
            RandomPassword.WithNumbers = chkNums.Checked;
            txtPassword.Text = RandomPassword.Generate(Convert.ToInt32(txtMaxRandomChars.Text));
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
                Clipboard.SetText(txtPassword.Text);
            }
            catch { }
        }
    }
}
