namespace Lain
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.lockIcon = new Lain.IconLabel();
            this.status = new Lain.FluentLabel();
            this.label1 = new Lain.FluentLabel();
            this.passwordBox = new Lain.InputBox();
            this.txtPassword = new Lain.FluentTextBox();
            this.pictureBox1 = new Lain.IconLabel();
            this.lblError = new Lain.FluentLabel();
            this.btnKey = new Lain.ModernButton();
            this.btnRecovery = new Lain.ModernButton();
            this.recoveryBox = new Lain.InputBox();
            this.txtRecovery = new Lain.FluentTextBox();
            this.footer = new Lain.FooterPanel();
            this.btnOk = new Lain.ModernButton();
            this.btnExit = new Lain.ModernButton();
            this.passwordBox.SuspendLayout();
            this.recoveryBox.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lockIcon
            // 
            this.lockIcon.Active = true;
            this.lockIcon.Fallback = "";
            this.lockIcon.Glyph = "\uE72E";
            this.lockIcon.IconSize = 22F;
            this.lockIcon.Location = new System.Drawing.Point(28, 26);
            this.lockIcon.Name = "lockIcon";
            this.lockIcon.Size = new System.Drawing.Size(44, 44);
            this.lockIcon.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Font = Fluent.Display(16F);
            this.status.Location = new System.Drawing.Point(28, 78);
            this.status.Name = "status";
            this.status.Text = "Unlock your vault";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(30, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 22);
            this.label1.Text = "Enter your master password to continue.";
            // 
            // passwordBox
            // 
            this.passwordBox.Controls.Add(this.txtPassword);
            this.passwordBox.Controls.Add(this.pictureBox1);
            this.passwordBox.Location = new System.Drawing.Point(30, 148);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(344, 38);
            this.passwordBox.TabIndex = 0;
            // 
            // txtPassword
            // 
            this.txtPassword.Font = Fluent.Ui(11.5F);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(290, 20);
            this.txtPassword.TabIndex = 0;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Clickable = true;
            this.pictureBox1.Glyph = "\uE7B3";
            this.pictureBox1.IconSize = 11F;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(32, 32);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // lblError
            // 
            this.lblError.Location = new System.Drawing.Point(30, 192);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(344, 22);
            this.lblError.Text = "That password is incorrect. Please try again.";
            this.lblError.Visible = false;
            // 
            // btnKey
            // 
            this.btnKey.Glyph = "\uE8D7";
            this.btnKey.Location = new System.Drawing.Point(24, 221);
            this.btnKey.Name = "btnKey";
            this.btnKey.Size = new System.Drawing.Size(170, 32);
            this.btnKey.TabIndex = 2;
            this.btnKey.Text = "Use security key";
            this.btnKey.Variant = Lain.ButtonVariant.Subtle;
            this.btnKey.Visible = false;
            this.btnKey.Click += new System.EventHandler(this.btnKey_Click);
            // 
            // btnRecovery
            // 
            this.btnRecovery.Location = new System.Drawing.Point(24, 221);
            this.btnRecovery.Name = "btnRecovery";
            this.btnRecovery.Size = new System.Drawing.Size(210, 32);
            this.btnRecovery.TabIndex = 3;
            this.btnRecovery.Text = "Use recovery code instead";
            this.btnRecovery.Variant = Lain.ButtonVariant.Subtle;
            this.btnRecovery.Visible = false;
            this.btnRecovery.Click += new System.EventHandler(this.btnRecovery_Click);
            // 
            // recoveryBox
            // 
            this.recoveryBox.Controls.Add(this.txtRecovery);
            this.recoveryBox.Location = new System.Drawing.Point(30, 218);
            this.recoveryBox.Name = "recoveryBox";
            this.recoveryBox.Size = new System.Drawing.Size(344, 38);
            this.recoveryBox.TabIndex = 4;
            this.recoveryBox.Visible = false;
            // 
            // txtRecovery
            // 
            this.txtRecovery.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtRecovery.Font = Fluent.Ui(11.5F);
            this.txtRecovery.MaxLength = 60;
            this.txtRecovery.Name = "txtRecovery";
            this.txtRecovery.Size = new System.Drawing.Size(320, 20);
            this.txtRecovery.TabIndex = 0;
            this.txtRecovery.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // footer
            // 
            this.footer.Controls.Add(this.btnOk);
            this.footer.Controls.Add(this.btnExit);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 268);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(404, 68);
            this.footer.TabIndex = 1;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(264, 16);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(110, 36);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Unlock";
            this.btnOk.Variant = Lain.ButtonVariant.Primary;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(146, 16);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(110, 36);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // LoginForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(404, 336);
            this.Controls.Add(this.lockIcon);
            this.Controls.Add(this.status);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.lblError);
            this.Controls.Add(this.btnKey);
            this.Controls.Add(this.btnRecovery);
            this.Controls.Add(this.recoveryBox);
            this.Controls.Add(this.footer);
            this.DoubleBuffered = true;
            this.Font = Fluent.Ui(10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lain";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.passwordBox.ResumeLayout(false);
            this.passwordBox.PerformLayout();
            this.recoveryBox.ResumeLayout(false);
            this.recoveryBox.PerformLayout();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Lain.IconLabel lockIcon;
        private Lain.FluentLabel status;
        private Lain.FluentLabel label1;
        private Lain.InputBox passwordBox;
        private Lain.FluentTextBox txtPassword;
        private Lain.IconLabel pictureBox1;
        private Lain.FluentLabel lblError;
        private Lain.ModernButton btnKey;
        private Lain.ModernButton btnRecovery;
        private Lain.InputBox recoveryBox;
        private Lain.FluentTextBox txtRecovery;
        private Lain.FooterPanel footer;
        private Lain.ModernButton btnOk;
        private Lain.ModernButton btnExit;
    }
}
