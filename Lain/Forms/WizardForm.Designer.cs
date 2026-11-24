namespace Lain
{
    partial class WizardForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardForm));
            this.shieldIcon = new Lain.IconLabel();
            this.status = new Lain.FluentLabel();
            this.lblIntro = new Lain.FluentLabel();
            this.label1 = new Lain.FluentLabel();
            this.passwordBox = new Lain.InputBox();
            this.txtPassword = new Lain.FluentTextBox();
            this.pictureBox1 = new Lain.IconLabel();
            this.label2 = new Lain.FluentLabel();
            this.verifyBox = new Lain.InputBox();
            this.txtVerify = new Lain.FluentTextBox();
            this.pictureBox2 = new Lain.IconLabel();
            this.lblMatch = new Lain.FluentLabel();
            this.footer = new Lain.FooterPanel();
            this.btnOk = new Lain.ModernButton();
            this.btnExit = new Lain.ModernButton();
            this.passwordBox.SuspendLayout();
            this.verifyBox.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // shieldIcon
            // 
            this.shieldIcon.Active = true;
            this.shieldIcon.Glyph = "\uEA18";
            this.shieldIcon.IconSize = 22F;
            this.shieldIcon.Location = new System.Drawing.Point(28, 26);
            this.shieldIcon.Name = "shieldIcon";
            this.shieldIcon.Size = new System.Drawing.Size(44, 44);
            this.shieldIcon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Font = Fluent.Display(16F);
            this.status.Location = new System.Drawing.Point(28, 78);
            this.status.Name = "status";
            this.status.Text = "Welcome to Lain";
            // 
            // lblIntro
            // 
            this.lblIntro.Location = new System.Drawing.Point(30, 112);
            this.lblIntro.Name = "lblIntro";
            this.lblIntro.Size = new System.Drawing.Size(364, 42);
            this.lblIntro.Text = "Create a master password to encrypt your vault. It can\'t be recovered, so make it strong and memorable.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = Fluent.UiSemibold(10F);
            this.label1.Location = new System.Drawing.Point(29, 166);
            this.label1.Name = "label1";
            this.label1.Text = "Master password";
            // 
            // passwordBox
            // 
            this.passwordBox.Controls.Add(this.txtPassword);
            this.passwordBox.Controls.Add(this.pictureBox1);
            this.passwordBox.Location = new System.Drawing.Point(30, 190);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(364, 38);
            this.passwordBox.TabIndex = 0;
            // 
            // txtPassword
            // 
            this.txtPassword.Font = Fluent.Ui(11.5F);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(300, 20);
            this.txtPassword.TabIndex = 0;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Clickable = true;
            this.pictureBox1.Glyph = "\uE7B3";
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = Fluent.UiSemibold(10F);
            this.label2.Location = new System.Drawing.Point(29, 240);
            this.label2.Name = "label2";
            this.label2.Text = "Confirm password";
            // 
            // verifyBox
            // 
            this.verifyBox.Controls.Add(this.txtVerify);
            this.verifyBox.Controls.Add(this.pictureBox2);
            this.verifyBox.Location = new System.Drawing.Point(30, 264);
            this.verifyBox.Name = "verifyBox";
            this.verifyBox.Size = new System.Drawing.Size(364, 38);
            this.verifyBox.TabIndex = 1;
            // 
            // txtVerify
            // 
            this.txtVerify.Font = Fluent.Ui(11.5F);
            this.txtVerify.Name = "txtVerify";
            this.txtVerify.Size = new System.Drawing.Size(300, 20);
            this.txtVerify.TabIndex = 0;
            this.txtVerify.UseSystemPasswordChar = true;
            this.txtVerify.TextChanged += new System.EventHandler(this.txtVerify_TextChanged);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Clickable = true;
            this.pictureBox2.Glyph = "\uE7B3";
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // lblMatch
            // 
            this.lblMatch.Location = new System.Drawing.Point(30, 308);
            this.lblMatch.Name = "lblMatch";
            this.lblMatch.Size = new System.Drawing.Size(364, 22);
            // 
            // footer
            // 
            this.footer.Controls.Add(this.btnOk);
            this.footer.Controls.Add(this.btnExit);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 342);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(424, 68);
            this.footer.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(274, 16);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(120, 36);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Get started";
            this.btnOk.Variant = Lain.ButtonVariant.Primary;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(156, 16);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(110, 36);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // WizardForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(424, 410);
            this.Controls.Add(this.shieldIcon);
            this.Controls.Add(this.status);
            this.Controls.Add(this.lblIntro);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.verifyBox);
            this.Controls.Add(this.lblMatch);
            this.Controls.Add(this.footer);
            this.DoubleBuffered = true;
            this.Font = Fluent.Ui(10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "WizardForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lain";
            this.Load += new System.EventHandler(this.WizardForm_Load);
            this.passwordBox.ResumeLayout(false);
            this.passwordBox.PerformLayout();
            this.verifyBox.ResumeLayout(false);
            this.verifyBox.PerformLayout();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Lain.IconLabel shieldIcon;
        private Lain.FluentLabel status;
        private Lain.FluentLabel lblIntro;
        private Lain.FluentLabel label1;
        private Lain.InputBox passwordBox;
        private Lain.FluentTextBox txtPassword;
        private Lain.IconLabel pictureBox1;
        private Lain.FluentLabel label2;
        private Lain.InputBox verifyBox;
        private Lain.FluentTextBox txtVerify;
        private Lain.IconLabel pictureBox2;
        private Lain.FluentLabel lblMatch;
        private Lain.FooterPanel footer;
        private Lain.ModernButton btnOk;
        private Lain.ModernButton btnExit;
    }
}
