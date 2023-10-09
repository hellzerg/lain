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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewForm));
            this.txtName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNote = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMaxRandomChars = new System.Windows.Forms.TextBox();
            this.btnCopyPassword = new System.Windows.Forms.Button();
            this.btnGenerateRandom = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLink = new System.Windows.Forms.TextBox();
            this.chkNums = new Lain.MoonCheck();
            this.chkSpecials = new Lain.MoonCheck();
            this.chkLowers = new Lain.MoonCheck();
            this.chkUppers = new Lain.MoonCheck();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtName.ForeColor = System.Drawing.Color.White;
            this.txtName.Location = new System.Drawing.Point(13, 32);
            this.txtName.Margin = new System.Windows.Forms.Padding(2);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(476, 29);
            this.txtName.TabIndex = 0;
            this.txtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 19);
            this.label1.TabIndex = 61;
            this.label1.Tag = "themeable";
            this.label1.Text = "Title";
            // 
            // txtMail
            // 
            this.txtMail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtMail.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMail.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMail.ForeColor = System.Drawing.Color.White;
            this.txtMail.Location = new System.Drawing.Point(13, 86);
            this.txtMail.Margin = new System.Windows.Forms.Padding(2);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(476, 29);
            this.txtMail.TabIndex = 1;
            this.txtMail.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label2.Location = new System.Drawing.Point(9, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 19);
            this.label2.TabIndex = 63;
            this.label2.Tag = "themeable";
            this.label2.Text = "Account";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPassword.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.ForeColor = System.Drawing.Color.White;
            this.txtPassword.Location = new System.Drawing.Point(13, 141);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(476, 29);
            this.txtPassword.TabIndex = 2;
            this.txtPassword.UseSystemPasswordChar = true;
            this.txtPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtName_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label3.Location = new System.Drawing.Point(9, 118);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 19);
            this.label3.TabIndex = 65;
            this.label3.Tag = "themeable";
            this.label3.Text = "Password";
            // 
            // txtNote
            // 
            this.txtNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNote.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtNote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNote.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNote.ForeColor = System.Drawing.Color.White;
            this.txtNote.Location = new System.Drawing.Point(17, 365);
            this.txtNote.Margin = new System.Windows.Forms.Padding(2);
            this.txtNote.Multiline = true;
            this.txtNote.Name = "txtNote";
            this.txtNote.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtNote.Size = new System.Drawing.Size(476, 180);
            this.txtNote.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label4.Location = new System.Drawing.Point(13, 344);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 19);
            this.label4.TabIndex = 67;
            this.label4.Tag = "themeable";
            this.label4.Text = "Notes (optional)";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(318, 559);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(83, 31);
            this.btnCancel.TabIndex = 70;
            this.btnCancel.Tag = "themeable";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOk.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(406, 559);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(83, 31);
            this.btnOk.TabIndex = 69;
            this.btnOk.Tag = "themeable";
            this.btnOk.Text = "OK";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(462, 141);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(27, 28);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 71;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Silver;
            this.label5.Location = new System.Drawing.Point(13, 197);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 19);
            this.label5.TabIndex = 79;
            this.label5.Tag = "";
            this.label5.Text = "Length:";
            // 
            // txtMaxRandomChars
            // 
            this.txtMaxRandomChars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMaxRandomChars.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtMaxRandomChars.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxRandomChars.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaxRandomChars.ForeColor = System.Drawing.Color.White;
            this.txtMaxRandomChars.Location = new System.Drawing.Point(72, 195);
            this.txtMaxRandomChars.Margin = new System.Windows.Forms.Padding(2);
            this.txtMaxRandomChars.MaxLength = 888;
            this.txtMaxRandomChars.Name = "txtMaxRandomChars";
            this.txtMaxRandomChars.Size = new System.Drawing.Size(74, 25);
            this.txtMaxRandomChars.TabIndex = 78;
            this.txtMaxRandomChars.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMaxRandomChars.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMaxRandomChars_KeyPress);
            // 
            // btnCopyPassword
            // 
            this.btnCopyPassword.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnCopyPassword.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnCopyPassword.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnCopyPassword.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnCopyPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCopyPassword.ForeColor = System.Drawing.Color.White;
            this.btnCopyPassword.Image = ((System.Drawing.Image)(resources.GetObject("btnCopyPassword.Image")));
            this.btnCopyPassword.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCopyPassword.Location = new System.Drawing.Point(403, 174);
            this.btnCopyPassword.Margin = new System.Windows.Forms.Padding(2);
            this.btnCopyPassword.Name = "btnCopyPassword";
            this.btnCopyPassword.Size = new System.Drawing.Size(86, 25);
            this.btnCopyPassword.TabIndex = 77;
            this.btnCopyPassword.Tag = "themeable";
            this.btnCopyPassword.Text = "Copy";
            this.btnCopyPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCopyPassword.UseVisualStyleBackColor = false;
            this.btnCopyPassword.Click += new System.EventHandler(this.btnCopyPassword_Click);
            // 
            // btnGenerateRandom
            // 
            this.btnGenerateRandom.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnGenerateRandom.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnGenerateRandom.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnGenerateRandom.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnGenerateRandom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGenerateRandom.ForeColor = System.Drawing.Color.White;
            this.btnGenerateRandom.Image = ((System.Drawing.Image)(resources.GetObject("btnGenerateRandom.Image")));
            this.btnGenerateRandom.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGenerateRandom.Location = new System.Drawing.Point(352, 249);
            this.btnGenerateRandom.Margin = new System.Windows.Forms.Padding(2);
            this.btnGenerateRandom.Name = "btnGenerateRandom";
            this.btnGenerateRandom.Size = new System.Drawing.Size(137, 25);
            this.btnGenerateRandom.TabIndex = 76;
            this.btnGenerateRandom.Tag = "themeable";
            this.btnGenerateRandom.Text = "Generate random";
            this.btnGenerateRandom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGenerateRandom.UseVisualStyleBackColor = false;
            this.btnGenerateRandom.Click += new System.EventHandler(this.btnGenerateRandom_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label6.Location = new System.Drawing.Point(13, 292);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 19);
            this.label6.TabIndex = 80;
            this.label6.Tag = "themeable";
            this.label6.Text = "Link (optional)";
            // 
            // txtLink
            // 
            this.txtLink.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLink.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtLink.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLink.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLink.ForeColor = System.Drawing.Color.White;
            this.txtLink.Location = new System.Drawing.Point(17, 313);
            this.txtLink.Margin = new System.Windows.Forms.Padding(2);
            this.txtLink.Name = "txtLink";
            this.txtLink.Size = new System.Drawing.Size(476, 29);
            this.txtLink.TabIndex = 3;
            // 
            // chkNums
            // 
            this.chkNums.AutoSize = true;
            this.chkNums.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNums.Location = new System.Drawing.Point(126, 251);
            this.chkNums.Margin = new System.Windows.Forms.Padding(2);
            this.chkNums.Name = "chkNums";
            this.chkNums.Size = new System.Drawing.Size(117, 23);
            this.chkNums.TabIndex = 89;
            this.chkNums.Text = "With numbers";
            this.chkNums.UseVisualStyleBackColor = true;
            // 
            // chkSpecials
            // 
            this.chkSpecials.AutoSize = true;
            this.chkSpecials.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSpecials.Location = new System.Drawing.Point(126, 224);
            this.chkSpecials.Margin = new System.Windows.Forms.Padding(2);
            this.chkSpecials.Name = "chkSpecials";
            this.chkSpecials.Size = new System.Drawing.Size(112, 23);
            this.chkSpecials.TabIndex = 88;
            this.chkSpecials.Text = "With specials";
            this.chkSpecials.UseVisualStyleBackColor = true;
            // 
            // chkLowers
            // 
            this.chkLowers.AutoSize = true;
            this.chkLowers.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLowers.Location = new System.Drawing.Point(17, 251);
            this.chkLowers.Margin = new System.Windows.Forms.Padding(2);
            this.chkLowers.Name = "chkLowers";
            this.chkLowers.Size = new System.Drawing.Size(104, 23);
            this.chkLowers.TabIndex = 87;
            this.chkLowers.Text = "With lowers";
            this.chkLowers.UseVisualStyleBackColor = true;
            // 
            // chkUppers
            // 
            this.chkUppers.AutoSize = true;
            this.chkUppers.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUppers.Location = new System.Drawing.Point(17, 224);
            this.chkUppers.Margin = new System.Windows.Forms.Padding(2);
            this.chkUppers.Name = "chkUppers";
            this.chkUppers.Size = new System.Drawing.Size(105, 23);
            this.chkUppers.TabIndex = 86;
            this.chkUppers.Text = "With uppers";
            this.chkUppers.UseVisualStyleBackColor = true;
            // 
            // NewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(502, 600);
            this.Controls.Add(this.chkNums);
            this.Controls.Add(this.chkSpecials);
            this.Controls.Add(this.chkLowers);
            this.Controls.Add(this.chkUppers);
            this.Controls.Add(this.txtLink);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtMaxRandomChars);
            this.Controls.Add(this.btnCopyPassword);
            this.Controls.Add(this.btnGenerateRandom);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtNote);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtMail);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create new account...";
            this.Load += new System.EventHandler(this.NewForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMail;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNote;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtMaxRandomChars;
        private System.Windows.Forms.Button btnCopyPassword;
        private System.Windows.Forms.Button btnGenerateRandom;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLink;
        private MoonCheck chkUppers;
        private MoonCheck chkLowers;
        private MoonCheck chkSpecials;
        private MoonCheck chkNums;
    }
}