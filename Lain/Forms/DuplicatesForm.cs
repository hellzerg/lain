using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Lain
{
    public partial class DuplicatesForm : Form
    {
        CryLain _cryLain = new CryLain();

        public DuplicatesForm(List<LainAccount> duplicates)
        {
            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;
            Options.ApplyTheme(this);

            for (int i = 0; i < duplicates.Count(); i++)
            {
                if (i >= 2)
                {
                    if (duplicates[i - 1].Password() != duplicates[i].Password())
                    {
                        txtReport.AppendText(Environment.NewLine);
                    }
                }

                txtReport.AppendText(string.Format("{0} : {1}",
                    _cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), duplicates[i].Name()),
                    _cryLain.Decrypt(CryLain.ToInsecureString(MainForm.Key), duplicates[i].Password())));

                txtReport.AppendText(Environment.NewLine);
            }
        }

        private void btnOk_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DuplicatesForm_Load(object sender, EventArgs e)
        {

        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtReport.Text);
            }
            catch { }
        }

        private void DuplicatesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            txtReport.Text = string.Empty;
        }
    }
}
