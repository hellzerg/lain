namespace Lain
{
    partial class OptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsForm));
            this.rSilver = new System.Windows.Forms.RadioButton();
            this.rAmber = new System.Windows.Forms.RadioButton();
            this.rJade = new System.Windows.Forms.RadioButton();
            this.rRuby = new System.Windows.Forms.RadioButton();
            this.rAzurite = new System.Windows.Forms.RadioButton();
            this.rAmethyst = new System.Windows.Forms.RadioButton();
            this.label27 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtTimer = new System.Windows.Forms.TextBox();
            this.lblMins = new System.Windows.Forms.Label();
            this.sizePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.rLarge = new Lain.MoonRadio();
            this.rNormal = new Lain.MoonRadio();
            this.rSmall = new Lain.MoonRadio();
            this.chkAutoSizeColumns = new Lain.MoonCheck();
            this.chkHidePass = new Lain.MoonCheck();
            this.chkAutoStart = new Lain.MoonCheck();
            this.chkAutoLock = new Lain.MoonCheck();
            this.chkAlwaysPass = new Lain.MoonCheck();
            this.sizePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // rSilver
            // 
            this.rSilver.AutoSize = true;
            this.rSilver.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rSilver.ForeColor = System.Drawing.Color.Gray;
            this.rSilver.Location = new System.Drawing.Point(152, 99);
            this.rSilver.Margin = new System.Windows.Forms.Padding(2);
            this.rSilver.Name = "rSilver";
            this.rSilver.Size = new System.Drawing.Size(68, 25);
            this.rSilver.TabIndex = 76;
            this.rSilver.Text = "Silver";
            this.rSilver.UseVisualStyleBackColor = true;
            this.rSilver.CheckedChanged += new System.EventHandler(this.minimaltheme_CheckedChanged);
            // 
            // rAmber
            // 
            this.rAmber.AutoSize = true;
            this.rAmber.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rAmber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(195)))), ((int)(((byte)(146)))), ((int)(((byte)(0)))));
            this.rAmber.Location = new System.Drawing.Point(152, 41);
            this.rAmber.Margin = new System.Windows.Forms.Padding(2);
            this.rAmber.Name = "rAmber";
            this.rAmber.Size = new System.Drawing.Size(78, 25);
            this.rAmber.TabIndex = 75;
            this.rAmber.Text = "Amber";
            this.rAmber.UseVisualStyleBackColor = true;
            this.rAmber.CheckedChanged += new System.EventHandler(this.carameltheme_CheckedChanged);
            // 
            // rJade
            // 
            this.rJade.AutoSize = true;
            this.rJade.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rJade.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(175)))), ((int)(((byte)(105)))));
            this.rJade.Location = new System.Drawing.Point(153, 70);
            this.rJade.Margin = new System.Windows.Forms.Padding(2);
            this.rJade.Name = "rJade";
            this.rJade.Size = new System.Drawing.Size(61, 25);
            this.rJade.TabIndex = 74;
            this.rJade.Text = "Jade";
            this.rJade.UseVisualStyleBackColor = true;
            this.rJade.CheckedChanged += new System.EventHandler(this.limetheme_CheckedChanged);
            // 
            // rRuby
            // 
            this.rRuby.AutoSize = true;
            this.rRuby.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rRuby.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(22)))), ((int)(((byte)(39)))));
            this.rRuby.Location = new System.Drawing.Point(30, 70);
            this.rRuby.Margin = new System.Windows.Forms.Padding(2);
            this.rRuby.Name = "rRuby";
            this.rRuby.Size = new System.Drawing.Size(65, 25);
            this.rRuby.TabIndex = 73;
            this.rRuby.Text = "Ruby";
            this.rRuby.UseVisualStyleBackColor = true;
            this.rRuby.CheckedChanged += new System.EventHandler(this.magmatheme_CheckedChanged);
            // 
            // rAzurite
            // 
            this.rAzurite.AutoSize = true;
            this.rAzurite.Checked = true;
            this.rAzurite.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rAzurite.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(127)))), ((int)(((byte)(255)))));
            this.rAzurite.Location = new System.Drawing.Point(30, 99);
            this.rAzurite.Margin = new System.Windows.Forms.Padding(2);
            this.rAzurite.Name = "rAzurite";
            this.rAzurite.Size = new System.Drawing.Size(80, 25);
            this.rAzurite.TabIndex = 72;
            this.rAzurite.TabStop = true;
            this.rAzurite.Text = "Azurite";
            this.rAzurite.UseVisualStyleBackColor = true;
            this.rAzurite.CheckedChanged += new System.EventHandler(this.oceantheme_CheckedChanged);
            // 
            // rAmethyst
            // 
            this.rAmethyst.AutoSize = true;
            this.rAmethyst.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rAmethyst.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.rAmethyst.Location = new System.Drawing.Point(30, 41);
            this.rAmethyst.Margin = new System.Windows.Forms.Padding(2);
            this.rAmethyst.Name = "rAmethyst";
            this.rAmethyst.Size = new System.Drawing.Size(98, 25);
            this.rAmethyst.TabIndex = 71;
            this.rAmethyst.Text = "Amethyst";
            this.rAmethyst.UseVisualStyleBackColor = true;
            this.rAmethyst.CheckedChanged += new System.EventHandler(this.zergtheme_CheckedChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label27.Location = new System.Drawing.Point(10, 7);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(153, 21);
            this.label27.TabIndex = 70;
            this.label27.Tag = "themeable";
            this.label27.Text = "Choose your theme";
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnOk.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnOk.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.Image = ((System.Drawing.Image)(resources.GetObject("btnOk.Image")));
            this.btnOk.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOk.Location = new System.Drawing.Point(215, 448);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(71, 31);
            this.btnOk.TabIndex = 77;
            this.btnOk.Tag = "themeable";
            this.btnOk.Text = "OK";
            this.btnOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtTimer
            // 
            this.txtTimer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtTimer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTimer.Enabled = false;
            this.txtTimer.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimer.ForeColor = System.Drawing.Color.White;
            this.txtTimer.Location = new System.Drawing.Point(135, 374);
            this.txtTimer.Margin = new System.Windows.Forms.Padding(2);
            this.txtTimer.Name = "txtTimer";
            this.txtTimer.ShortcutsEnabled = false;
            this.txtTimer.Size = new System.Drawing.Size(50, 29);
            this.txtTimer.TabIndex = 80;
            this.txtTimer.Text = "2";
            this.txtTimer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtTimer.TextChanged += new System.EventHandler(this.txtTimer_TextChanged);
            this.txtTimer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTimer_KeyPress);
            // 
            // lblMins
            // 
            this.lblMins.AutoSize = true;
            this.lblMins.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMins.Location = new System.Drawing.Point(190, 377);
            this.lblMins.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMins.Name = "lblMins";
            this.lblMins.Size = new System.Drawing.Size(68, 21);
            this.lblMins.TabIndex = 81;
            this.lblMins.Text = "minutes";
            // 
            // sizePanel
            // 
            this.sizePanel.Controls.Add(this.label2);
            this.sizePanel.Controls.Add(this.rLarge);
            this.sizePanel.Controls.Add(this.rNormal);
            this.sizePanel.Controls.Add(this.rSmall);
            this.sizePanel.Location = new System.Drawing.Point(10, 139);
            this.sizePanel.Margin = new System.Windows.Forms.Padding(2);
            this.sizePanel.Name = "sizePanel";
            this.sizePanel.Size = new System.Drawing.Size(221, 132);
            this.sizePanel.TabIndex = 83;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 21);
            this.label2.TabIndex = 84;
            this.label2.Tag = "themeable";
            this.label2.Text = "Choose your font size";
            // 
            // rLarge
            // 
            this.rLarge.AutoSize = true;
            this.rLarge.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rLarge.ForeColor = System.Drawing.Color.White;
            this.rLarge.Location = new System.Drawing.Point(20, 93);
            this.rLarge.Margin = new System.Windows.Forms.Padding(2);
            this.rLarge.Name = "rLarge";
            this.rLarge.Size = new System.Drawing.Size(69, 25);
            this.rLarge.TabIndex = 79;
            this.rLarge.Tag = "";
            this.rLarge.Text = "Large";
            this.rLarge.UseVisualStyleBackColor = true;
            this.rLarge.CheckedChanged += new System.EventHandler(this.rLarge_CheckedChanged);
            // 
            // rNormal
            // 
            this.rNormal.AutoSize = true;
            this.rNormal.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rNormal.ForeColor = System.Drawing.Color.White;
            this.rNormal.Location = new System.Drawing.Point(20, 62);
            this.rNormal.Margin = new System.Windows.Forms.Padding(2);
            this.rNormal.Name = "rNormal";
            this.rNormal.Size = new System.Drawing.Size(82, 25);
            this.rNormal.TabIndex = 78;
            this.rNormal.Tag = "";
            this.rNormal.Text = "Normal";
            this.rNormal.UseVisualStyleBackColor = true;
            this.rNormal.CheckedChanged += new System.EventHandler(this.rNormal_CheckedChanged);
            // 
            // rSmall
            // 
            this.rSmall.AutoSize = true;
            this.rSmall.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rSmall.ForeColor = System.Drawing.Color.White;
            this.rSmall.Location = new System.Drawing.Point(20, 32);
            this.rSmall.Margin = new System.Windows.Forms.Padding(2);
            this.rSmall.Name = "rSmall";
            this.rSmall.Size = new System.Drawing.Size(67, 25);
            this.rSmall.TabIndex = 77;
            this.rSmall.Tag = "";
            this.rSmall.Text = "Small";
            this.rSmall.UseVisualStyleBackColor = true;
            this.rSmall.CheckedChanged += new System.EventHandler(this.rSmall_CheckedChanged);
            // 
            // chkAutoSizeColumns
            // 
            this.chkAutoSizeColumns.AutoSize = true;
            this.chkAutoSizeColumns.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoSizeColumns.Location = new System.Drawing.Point(16, 285);
            this.chkAutoSizeColumns.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoSizeColumns.Name = "chkAutoSizeColumns";
            this.chkAutoSizeColumns.Size = new System.Drawing.Size(163, 25);
            this.chkAutoSizeColumns.TabIndex = 85;
            this.chkAutoSizeColumns.Text = "Auto-size columns";
            this.chkAutoSizeColumns.UseVisualStyleBackColor = true;
            this.chkAutoSizeColumns.CheckedChanged += new System.EventHandler(this.chkAutoSizeColumns_CheckedChanged);
            // 
            // chkHidePass
            // 
            this.chkHidePass.AutoSize = true;
            this.chkHidePass.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkHidePass.Location = new System.Drawing.Point(16, 344);
            this.chkHidePass.Margin = new System.Windows.Forms.Padding(2);
            this.chkHidePass.Name = "chkHidePass";
            this.chkHidePass.Size = new System.Drawing.Size(145, 25);
            this.chkHidePass.TabIndex = 84;
            this.chkHidePass.Text = "Hide passwords";
            this.chkHidePass.UseVisualStyleBackColor = true;
            this.chkHidePass.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // chkAutoStart
            // 
            this.chkAutoStart.AutoSize = true;
            this.chkAutoStart.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoStart.Location = new System.Drawing.Point(16, 405);
            this.chkAutoStart.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoStart.Name = "chkAutoStart";
            this.chkAutoStart.Size = new System.Drawing.Size(203, 25);
            this.chkAutoStart.TabIndex = 79;
            this.chkAutoStart.Text = "Start Lain with Windows";
            this.chkAutoStart.UseVisualStyleBackColor = true;
            this.chkAutoStart.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // chkAutoLock
            // 
            this.chkAutoLock.AutoSize = true;
            this.chkAutoLock.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoLock.Location = new System.Drawing.Point(16, 375);
            this.chkAutoLock.Margin = new System.Windows.Forms.Padding(2);
            this.chkAutoLock.Name = "chkAutoLock";
            this.chkAutoLock.Size = new System.Drawing.Size(116, 25);
            this.chkAutoLock.TabIndex = 78;
            this.chkAutoLock.Text = "Auto lock in";
            this.chkAutoLock.UseVisualStyleBackColor = true;
            this.chkAutoLock.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // chkAlwaysPass
            // 
            this.chkAlwaysPass.AutoSize = true;
            this.chkAlwaysPass.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAlwaysPass.Location = new System.Drawing.Point(16, 314);
            this.chkAlwaysPass.Margin = new System.Windows.Forms.Padding(2);
            this.chkAlwaysPass.Name = "chkAlwaysPass";
            this.chkAlwaysPass.Size = new System.Drawing.Size(206, 25);
            this.chkAlwaysPass.TabIndex = 0;
            this.chkAlwaysPass.Text = "Always ask for password";
            this.chkAlwaysPass.UseVisualStyleBackColor = true;
            this.chkAlwaysPass.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(296, 489);
            this.Controls.Add(this.chkAutoSizeColumns);
            this.Controls.Add(this.chkHidePass);
            this.Controls.Add(this.sizePanel);
            this.Controls.Add(this.lblMins);
            this.Controls.Add(this.txtTimer);
            this.Controls.Add(this.chkAutoStart);
            this.Controls.Add(this.chkAutoLock);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.rSilver);
            this.Controls.Add(this.rAmber);
            this.Controls.Add(this.rJade);
            this.Controls.Add(this.rRuby);
            this.Controls.Add(this.rAzurite);
            this.Controls.Add(this.rAmethyst);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.chkAlwaysPass);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.OptionsForm_Load);
            this.sizePanel.ResumeLayout(false);
            this.sizePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MoonCheck chkAlwaysPass;
        private System.Windows.Forms.RadioButton rSilver;
        private System.Windows.Forms.RadioButton rAmber;
        private System.Windows.Forms.RadioButton rJade;
        private System.Windows.Forms.RadioButton rRuby;
        private System.Windows.Forms.RadioButton rAzurite;
        private System.Windows.Forms.RadioButton rAmethyst;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button btnOk;
        private MoonCheck chkAutoLock;
        private MoonCheck chkAutoStart;
        private System.Windows.Forms.TextBox txtTimer;
        private System.Windows.Forms.Label lblMins;
        private System.Windows.Forms.Panel sizePanel;
        private System.Windows.Forms.Label label2;
        private MoonRadio rLarge;
        private MoonRadio rNormal;
        private MoonRadio rSmall;
        private MoonCheck chkHidePass;
        private MoonCheck chkAutoSizeColumns;
    }
}