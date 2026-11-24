namespace Lain
{
    partial class NewForm
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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new Lain.FluentLabel();
            this.label1 = new Lain.FluentLabel();
            this.nameBox = new Lain.InputBox();
            this.txtName = new Lain.FluentTextBox();
            this.label2 = new Lain.FluentLabel();
            this.mailBox = new Lain.InputBox();
            this.txtMail = new Lain.FluentTextBox();
            this.label3 = new Lain.FluentLabel();
            this.passwordBox = new Lain.InputBox();
            this.txtPassword = new Lain.FluentTextBox();
            this.pictureBox1 = new Lain.IconLabel();
            this.btnCopyPassword = new Lain.IconLabel();
            this.generator = new Lain.Card();
            this.btnGenerateRandom = new Lain.ModernButton();
            this.label5 = new Lain.FluentLabel();
            this.lengthBox = new Lain.InputBox();
            this.txtMaxRandomChars = new Lain.FluentTextBox();
            this.chkUppers = new Lain.MoonCheck();
            this.chkLowers = new Lain.MoonCheck();
            this.chkNums = new Lain.MoonCheck();
            this.chkSpecials = new Lain.MoonCheck();
            this.label6 = new Lain.FluentLabel();
            this.linkBox = new Lain.InputBox();
            this.txtLink = new Lain.FluentTextBox();
            this.label4 = new Lain.FluentLabel();
            this.noteBox = new Lain.InputBox();
            this.txtNote = new Lain.FluentTextBox();
            this.lblValidation = new Lain.FluentLabel();
            this.lblDates = new Lain.FluentLabel();
            this.strengthMeter = new Lain.StrengthMeter();
            this.label7 = new Lain.FluentLabel();
            this.tagsBox = new Lain.InputBox();
            this.txtTags = new Lain.FluentTextBox();
            this.chkFavorite = new Lain.MoonCheck();
            this.footer = new Lain.FooterPanel();
            this.btnOk = new Lain.ModernButton();
            this.btnCancel = new Lain.ModernButton();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.nameBox.SuspendLayout();
            this.mailBox.SuspendLayout();
            this.passwordBox.SuspendLayout();
            this.generator.SuspendLayout();
            this.lengthBox.SuspendLayout();
            this.linkBox.SuspendLayout();
            this.noteBox.SuspendLayout();
            this.tagsBox.SuspendLayout();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = Fluent.Display(16F);
            this.lblTitle.Location = new System.Drawing.Point(26, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "New account";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = Fluent.UiSemibold(10F);
            this.label1.Location = new System.Drawing.Point(29, 66);
            this.label1.Name = "label1";
            this.label1.Text = "Title";
            // 
            // nameBox
            // 
            this.nameBox.Controls.Add(this.txtName);
            this.nameBox.Location = new System.Drawing.Point(30, 88);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(460, 36);
            this.nameBox.TabIndex = 0;
            // 
            // txtName
            // 
            this.txtName.Font = Fluent.Ui(11F);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(420, 19);
            this.txtName.TabIndex = 0;
            this.txtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = Fluent.UiSemibold(10F);
            this.label2.Location = new System.Drawing.Point(29, 136);
            this.label2.Name = "label2";
            this.label2.Text = "Username or email";
            // 
            // mailBox
            // 
            this.mailBox.Controls.Add(this.txtMail);
            this.mailBox.Location = new System.Drawing.Point(30, 158);
            this.mailBox.Name = "mailBox";
            this.mailBox.Size = new System.Drawing.Size(460, 36);
            this.mailBox.TabIndex = 1;
            // 
            // txtMail
            // 
            this.txtMail.Font = Fluent.Ui(11F);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(420, 19);
            this.txtMail.TabIndex = 0;
            this.txtMail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = Fluent.UiSemibold(10F);
            this.label3.Location = new System.Drawing.Point(29, 206);
            this.label3.Name = "label3";
            this.label3.Text = "Password";
            // 
            // passwordBox
            // 
            this.passwordBox.Controls.Add(this.txtPassword);
            this.passwordBox.Controls.Add(this.pictureBox1);
            this.passwordBox.Controls.Add(this.btnCopyPassword);
            this.passwordBox.Location = new System.Drawing.Point(30, 228);
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.Size = new System.Drawing.Size(460, 36);
            this.passwordBox.TabIndex = 2;
            // 
            // txtPassword
            // 
            this.txtPassword.Font = Fluent.Mono(11F);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(380, 19);
            this.txtPassword.TabIndex = 0;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Clickable = true;
            this.pictureBox1.Glyph = "\uE7B3";
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(30, 30);
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // btnCopyPassword
            // 
            this.btnCopyPassword.Clickable = true;
            this.btnCopyPassword.Fallback = "copy";
            this.btnCopyPassword.Glyph = "\uE8C8";
            this.btnCopyPassword.Name = "btnCopyPassword";
            this.btnCopyPassword.Size = new System.Drawing.Size(30, 30);
            this.btnCopyPassword.Click += new System.EventHandler(this.btnCopyPassword_Click);
            // 
            // strengthMeter
            // 
            this.strengthMeter.Location = new System.Drawing.Point(30, 268);
            this.strengthMeter.Name = "strengthMeter";
            this.strengthMeter.Size = new System.Drawing.Size(460, 22);
            // 
            // generator
            // 
            this.generator.Controls.Add(this.btnGenerateRandom);
            this.generator.Controls.Add(this.label5);
            this.generator.Controls.Add(this.lengthBox);
            this.generator.Controls.Add(this.chkUppers);
            this.generator.Controls.Add(this.chkLowers);
            this.generator.Controls.Add(this.chkNums);
            this.generator.Controls.Add(this.chkSpecials);
            this.generator.Location = new System.Drawing.Point(30, 296);
            this.generator.Name = "generator";
            this.generator.Size = new System.Drawing.Size(460, 98);
            this.generator.TabIndex = 3;
            // 
            // btnGenerateRandom
            // 
            this.btnGenerateRandom.Glyph = "\uE72C";
            this.btnGenerateRandom.Location = new System.Drawing.Point(12, 12);
            this.btnGenerateRandom.Name = "btnGenerateRandom";
            this.btnGenerateRandom.Size = new System.Drawing.Size(178, 34);
            this.btnGenerateRandom.TabIndex = 0;
            this.btnGenerateRandom.Text = "Generate password";
            this.btnGenerateRandom.Click += new System.EventHandler(this.btnGenerateRandom_Click);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(300, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 34);
            this.label5.Text = "Length";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lengthBox
            // 
            this.lengthBox.Controls.Add(this.txtMaxRandomChars);
            this.lengthBox.Location = new System.Drawing.Point(378, 12);
            this.lengthBox.Name = "lengthBox";
            this.lengthBox.Size = new System.Drawing.Size(70, 34);
            this.lengthBox.TabIndex = 1;
            // 
            // txtMaxRandomChars
            // 
            this.txtMaxRandomChars.Font = Fluent.Ui(11F);
            this.txtMaxRandomChars.MaxLength = 888;
            this.txtMaxRandomChars.Name = "txtMaxRandomChars";
            this.txtMaxRandomChars.Size = new System.Drawing.Size(50, 19);
            this.txtMaxRandomChars.TabIndex = 0;
            this.txtMaxRandomChars.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMaxRandomChars.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxRandomChars_KeyPress);
            // 
            // chkUppers
            // 
            this.chkUppers.Location = new System.Drawing.Point(14, 58);
            this.chkUppers.Name = "chkUppers";
            this.chkUppers.Size = new System.Drawing.Size(106, 28);
            this.chkUppers.TabIndex = 2;
            this.chkUppers.Text = "Uppercase";
            // 
            // chkLowers
            // 
            this.chkLowers.Location = new System.Drawing.Point(124, 58);
            this.chkLowers.Name = "chkLowers";
            this.chkLowers.Size = new System.Drawing.Size(106, 28);
            this.chkLowers.TabIndex = 3;
            this.chkLowers.Text = "Lowercase";
            // 
            // chkNums
            // 
            this.chkNums.Location = new System.Drawing.Point(234, 58);
            this.chkNums.Name = "chkNums";
            this.chkNums.Size = new System.Drawing.Size(100, 28);
            this.chkNums.TabIndex = 4;
            this.chkNums.Text = "Numbers";
            // 
            // chkSpecials
            // 
            this.chkSpecials.Location = new System.Drawing.Point(338, 58);
            this.chkSpecials.Name = "chkSpecials";
            this.chkSpecials.Size = new System.Drawing.Size(110, 28);
            this.chkSpecials.TabIndex = 5;
            this.chkSpecials.Text = "Symbols";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = Fluent.UiSemibold(10F);
            this.label6.Location = new System.Drawing.Point(29, 406);
            this.label6.Name = "label6";
            this.label6.Text = "Website";
            // 
            // linkBox
            // 
            this.linkBox.Controls.Add(this.txtLink);
            this.linkBox.Location = new System.Drawing.Point(30, 428);
            this.linkBox.Name = "linkBox";
            this.linkBox.Size = new System.Drawing.Size(460, 36);
            this.linkBox.TabIndex = 4;
            // 
            // txtLink
            // 
            this.txtLink.Font = Fluent.Ui(11F);
            this.txtLink.Name = "txtLink";
            this.txtLink.Size = new System.Drawing.Size(420, 19);
            this.txtLink.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = Fluent.UiSemibold(10F);
            this.label7.Location = new System.Drawing.Point(29, 476);
            this.label7.Name = "label7";
            this.label7.Text = "Tags";
            // 
            // tagsBox
            // 
            this.tagsBox.Controls.Add(this.txtTags);
            this.tagsBox.Location = new System.Drawing.Point(30, 498);
            this.tagsBox.Name = "tagsBox";
            this.tagsBox.Size = new System.Drawing.Size(330, 36);
            this.tagsBox.TabIndex = 5;
            // 
            // txtTags
            // 
            this.txtTags.Font = Fluent.Ui(11F);
            this.txtTags.MaxLength = 700;
            this.txtTags.Name = "txtTags";
            this.txtTags.Size = new System.Drawing.Size(300, 19);
            this.txtTags.TabIndex = 0;
            // 
            // chkFavorite
            // 
            this.chkFavorite.Location = new System.Drawing.Point(376, 502);
            this.chkFavorite.Name = "chkFavorite";
            this.chkFavorite.Size = new System.Drawing.Size(114, 28);
            this.chkFavorite.TabIndex = 6;
            this.chkFavorite.Text = "Favorite";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = Fluent.UiSemibold(10F);
            this.label4.Location = new System.Drawing.Point(29, 546);
            this.label4.Name = "label4";
            this.label4.Text = "Notes";
            // 
            // noteBox
            // 
            this.noteBox.Controls.Add(this.txtNote);
            this.noteBox.Location = new System.Drawing.Point(30, 568);
            this.noteBox.Name = "noteBox";
            this.noteBox.Size = new System.Drawing.Size(460, 76);
            this.noteBox.TabIndex = 7;
            // 
            // txtNote
            // 
            this.txtNote.Font = Fluent.Ui(11F);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNote.Size = new System.Drawing.Size(420, 60);
            this.txtNote.TabIndex = 0;
            // 
            // lblValidation
            // 
            this.lblValidation.Location = new System.Drawing.Point(30, 650);
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Size = new System.Drawing.Size(460, 22);
            this.lblValidation.Text = "Title, username and password are required.";
            this.lblValidation.Visible = false;
            // 
            // lblDates
            // 
            this.lblDates.Location = new System.Drawing.Point(30, 650);
            this.lblDates.Name = "lblDates";
            this.lblDates.Size = new System.Drawing.Size(460, 22);
            this.lblDates.Text = "";
            this.lblDates.Visible = false;
            // 
            // footer
            // 
            this.footer.Controls.Add(this.btnOk);
            this.footer.Controls.Add(this.btnCancel);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 680);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(520, 66);
            this.footer.TabIndex = 8;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(366, 15);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(124, 36);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Add account";
            this.btnOk.Variant = Lain.ButtonVariant.Primary;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(248, 15);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 36);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // NewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(520, 746);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nameBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mailBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.passwordBox);
            this.Controls.Add(this.strengthMeter);
            this.Controls.Add(this.generator);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.linkBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tagsBox);
            this.Controls.Add(this.chkFavorite);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.noteBox);
            this.Controls.Add(this.lblValidation);
            this.Controls.Add(this.lblDates);
            this.Controls.Add(this.footer);
            this.DoubleBuffered = true;
            this.Font = Fluent.Ui(10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New account";
            this.Load += new System.EventHandler(this.NewForm_Load);
            this.nameBox.ResumeLayout(false);
            this.nameBox.PerformLayout();
            this.mailBox.ResumeLayout(false);
            this.mailBox.PerformLayout();
            this.passwordBox.ResumeLayout(false);
            this.passwordBox.PerformLayout();
            this.generator.ResumeLayout(false);
            this.lengthBox.ResumeLayout(false);
            this.lengthBox.PerformLayout();
            this.linkBox.ResumeLayout(false);
            this.linkBox.PerformLayout();
            this.noteBox.ResumeLayout(false);
            this.noteBox.PerformLayout();
            this.tagsBox.ResumeLayout(false);
            this.tagsBox.PerformLayout();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Lain.FluentLabel lblTitle;
        private Lain.FluentLabel label1;
        private Lain.InputBox nameBox;
        private Lain.FluentTextBox txtName;
        private Lain.FluentLabel label2;
        private Lain.InputBox mailBox;
        private Lain.FluentTextBox txtMail;
        private Lain.FluentLabel label3;
        private Lain.InputBox passwordBox;
        private Lain.FluentTextBox txtPassword;
        private Lain.IconLabel pictureBox1;
        private Lain.IconLabel btnCopyPassword;
        private Lain.Card generator;
        private Lain.ModernButton btnGenerateRandom;
        private Lain.FluentLabel label5;
        private Lain.InputBox lengthBox;
        private Lain.FluentTextBox txtMaxRandomChars;
        private Lain.MoonCheck chkUppers;
        private Lain.MoonCheck chkLowers;
        private Lain.MoonCheck chkNums;
        private Lain.MoonCheck chkSpecials;
        private Lain.FluentLabel label6;
        private Lain.InputBox linkBox;
        private Lain.FluentTextBox txtLink;
        private Lain.FluentLabel label4;
        private Lain.InputBox noteBox;
        private Lain.FluentTextBox txtNote;
        private Lain.FluentLabel lblValidation;
        private Lain.FluentLabel lblDates;
        private Lain.StrengthMeter strengthMeter;
        private Lain.FluentLabel label7;
        private Lain.InputBox tagsBox;
        private Lain.FluentTextBox txtTags;
        private Lain.MoonCheck chkFavorite;
        private Lain.FooterPanel footer;
        private Lain.ModernButton btnOk;
        private Lain.ModernButton btnCancel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
