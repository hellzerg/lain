namespace Lain
{
    partial class ChangePasswordForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.keyIcon = new Lain.IconLabel();
            this.lblTitle = new Lain.FluentLabel();
            this.lblIntro = new Lain.FluentLabel();
            this.label1 = new Lain.FluentLabel();
            this.currentBox = new Lain.InputBox();
            this.txtCurrent = new Lain.FluentTextBox();
            this.eyeCurrent = new Lain.IconLabel();
            this.label2 = new Lain.FluentLabel();
            this.newBox = new Lain.InputBox();
            this.txtNew = new Lain.FluentTextBox();
            this.eyeNew = new Lain.IconLabel();
            this.strengthMeter = new Lain.StrengthMeter();
            this.label3 = new Lain.FluentLabel();
            this.verifyBox = new Lain.InputBox();
            this.txtVerify = new Lain.FluentTextBox();
            this.eyeVerify = new Lain.IconLabel();
            this.lblMessage = new Lain.FluentLabel();
            this.footer = new Lain.FooterPanel();
            this.btnOk = new Lain.ModernButton();
            this.btnCancel = new Lain.ModernButton();
            this.currentBox.SuspendLayout();
            this.newBox.SuspendLayout();
            this.verifyBox.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // keyIcon
            // 
            this.keyIcon.Active = true;
            this.keyIcon.Glyph = "\uE8D7";
            this.keyIcon.IconSize = 18F;
            this.keyIcon.Location = new System.Drawing.Point(24, 18);
            this.keyIcon.Name = "keyIcon";
            this.keyIcon.Size = new System.Drawing.Size(36, 36);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = Fluent.Display(16F);
            this.lblTitle.Location = new System.Drawing.Point(62, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Change master password";
            // 
            // lblIntro
            // 
            this.lblIntro.Location = new System.Drawing.Point(30, 64);
            this.lblIntro.Name = "lblIntro";
            this.lblIntro.Size = new System.Drawing.Size(384, 42);
            this.lblIntro.Text = "Your whole vault is re-encrypted with the new password. It can\'t be recovered if you forget it.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = Fluent.UiSemibold(10F);
            this.label1.Location = new System.Drawing.Point(29, 118);
            this.label1.Name = "label1";
            this.label1.Text = "Current password";
            // 
            // currentBox
            // 
            this.currentBox.Controls.Add(this.txtCurrent);
            this.currentBox.Controls.Add(this.eyeCurrent);
            this.currentBox.Location = new System.Drawing.Point(30, 142);
            this.currentBox.Name = "currentBox";
            this.currentBox.Size = new System.Drawing.Size(384, 38);
            this.currentBox.TabIndex = 0;
            // 
            // txtCurrent
            // 
            this.txtCurrent.Font = Fluent.Ui(11.5F);
            this.txtCurrent.Name = "txtCurrent";
            this.txtCurrent.Size = new System.Drawing.Size(320, 20);
            this.txtCurrent.TabIndex = 0;
            this.txtCurrent.UseSystemPasswordChar = true;
            // 
            // eyeCurrent
            // 
            this.eyeCurrent.Clickable = true;
            this.eyeCurrent.Glyph = "\uE7B3";
            this.eyeCurrent.Name = "eyeCurrent";
            this.eyeCurrent.Size = new System.Drawing.Size(32, 32);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = Fluent.UiSemibold(10F);
            this.label2.Location = new System.Drawing.Point(29, 196);
            this.label2.Name = "label2";
            this.label2.Text = "New password";
            // 
            // newBox
            // 
            this.newBox.Controls.Add(this.txtNew);
            this.newBox.Controls.Add(this.eyeNew);
            this.newBox.Location = new System.Drawing.Point(30, 220);
            this.newBox.Name = "newBox";
            this.newBox.Size = new System.Drawing.Size(384, 38);
            this.newBox.TabIndex = 1;
            // 
            // txtNew
            // 
            this.txtNew.Font = Fluent.Ui(11.5F);
            this.txtNew.Name = "txtNew";
            this.txtNew.Size = new System.Drawing.Size(320, 20);
            this.txtNew.TabIndex = 0;
            this.txtNew.UseSystemPasswordChar = true;
            // 
            // eyeNew
            // 
            this.eyeNew.Clickable = true;
            this.eyeNew.Glyph = "\uE7B3";
            this.eyeNew.Name = "eyeNew";
            this.eyeNew.Size = new System.Drawing.Size(32, 32);
            // 
            // strengthMeter
            // 
            this.strengthMeter.Location = new System.Drawing.Point(30, 266);
            this.strengthMeter.Name = "strengthMeter";
            this.strengthMeter.Size = new System.Drawing.Size(384, 22);
            this.strengthMeter.TabStop = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = Fluent.UiSemibold(10F);
            this.label3.Location = new System.Drawing.Point(29, 300);
            this.label3.Name = "label3";
            this.label3.Text = "Confirm new password";
            // 
            // verifyBox
            // 
            this.verifyBox.Controls.Add(this.txtVerify);
            this.verifyBox.Controls.Add(this.eyeVerify);
            this.verifyBox.Location = new System.Drawing.Point(30, 324);
            this.verifyBox.Name = "verifyBox";
            this.verifyBox.Size = new System.Drawing.Size(384, 38);
            this.verifyBox.TabIndex = 2;
            // 
            // txtVerify
            // 
            this.txtVerify.Font = Fluent.Ui(11.5F);
            this.txtVerify.Name = "txtVerify";
            this.txtVerify.Size = new System.Drawing.Size(320, 20);
            this.txtVerify.TabIndex = 0;
            this.txtVerify.UseSystemPasswordChar = true;
            // 
            // eyeVerify
            // 
            this.eyeVerify.Clickable = true;
            this.eyeVerify.Glyph = "\uE7B3";
            this.eyeVerify.Name = "eyeVerify";
            this.eyeVerify.Size = new System.Drawing.Size(32, 32);
            // 
            // lblMessage
            // 
            this.lblMessage.Location = new System.Drawing.Point(30, 370);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(384, 42);
            // 
            // footer
            // 
            this.footer.Controls.Add(this.btnOk);
            this.footer.Controls.Add(this.btnCancel);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 426);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(444, 68);
            this.footer.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Enabled = false;
            this.btnOk.Location = new System.Drawing.Point(264, 16);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(150, 36);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Change password";
            this.btnOk.Variant = Lain.ButtonVariant.Primary;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(146, 16);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 36);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            // 
            // ChangePasswordForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(444, 494);
            this.Controls.Add(this.keyIcon);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblIntro);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.currentBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.newBox);
            this.Controls.Add(this.strengthMeter);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.verifyBox);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.footer);
            this.DoubleBuffered = true;
            this.Font = Fluent.Ui(10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChangePasswordForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Master password";
            this.currentBox.ResumeLayout(false);
            this.currentBox.PerformLayout();
            this.newBox.ResumeLayout(false);
            this.newBox.PerformLayout();
            this.verifyBox.ResumeLayout(false);
            this.verifyBox.PerformLayout();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Lain.IconLabel keyIcon;
        private Lain.FluentLabel lblTitle;
        private Lain.FluentLabel lblIntro;
        private Lain.FluentLabel label1;
        private Lain.InputBox currentBox;
        private Lain.FluentTextBox txtCurrent;
        private Lain.IconLabel eyeCurrent;
        private Lain.FluentLabel label2;
        private Lain.InputBox newBox;
        private Lain.FluentTextBox txtNew;
        private Lain.IconLabel eyeNew;
        private Lain.StrengthMeter strengthMeter;
        private Lain.FluentLabel label3;
        private Lain.InputBox verifyBox;
        private Lain.FluentTextBox txtVerify;
        private Lain.IconLabel eyeVerify;
        private Lain.FluentLabel lblMessage;
        private Lain.FooterPanel footer;
        private Lain.ModernButton btnOk;
        private Lain.ModernButton btnCancel;
    }
}
