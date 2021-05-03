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
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.minimaltheme = new System.Windows.Forms.RadioButton();
            this.carameltheme = new System.Windows.Forms.RadioButton();
            this.limetheme = new System.Windows.Forms.RadioButton();
            this.magmatheme = new System.Windows.Forms.RadioButton();
            this.oceantheme = new System.Windows.Forms.RadioButton();
            this.zergtheme = new System.Windows.Forms.RadioButton();
            this.label27 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.txtTimer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.sizePanel = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.rLarge = new System.Windows.Forms.RadioButton();
            this.rNormal = new System.Windows.Forms.RadioButton();
            this.rSmall = new System.Windows.Forms.RadioButton();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.sizePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.Location = new System.Drawing.Point(14, 276);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(206, 25);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Always ask for password";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // minimaltheme
            // 
            this.minimaltheme.AutoSize = true;
            this.minimaltheme.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.minimaltheme.ForeColor = System.Drawing.Color.Gray;
            this.minimaltheme.Location = new System.Drawing.Point(122, 101);
            this.minimaltheme.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.minimaltheme.Name = "minimaltheme";
            this.minimaltheme.Size = new System.Drawing.Size(86, 25);
            this.minimaltheme.TabIndex = 76;
            this.minimaltheme.Text = "Minimal";
            this.minimaltheme.UseVisualStyleBackColor = true;
            this.minimaltheme.CheckedChanged += new System.EventHandler(this.minimaltheme_CheckedChanged);
            // 
            // carameltheme
            // 
            this.carameltheme.AutoSize = true;
            this.carameltheme.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.carameltheme.ForeColor = System.Drawing.Color.DarkOrange;
            this.carameltheme.Location = new System.Drawing.Point(122, 40);
            this.carameltheme.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.carameltheme.Name = "carameltheme";
            this.carameltheme.Size = new System.Drawing.Size(87, 25);
            this.carameltheme.TabIndex = 75;
            this.carameltheme.Text = "Caramel";
            this.carameltheme.UseVisualStyleBackColor = true;
            this.carameltheme.CheckedChanged += new System.EventHandler(this.carameltheme_CheckedChanged);
            // 
            // limetheme
            // 
            this.limetheme.AutoSize = true;
            this.limetheme.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.limetheme.ForeColor = System.Drawing.Color.LimeGreen;
            this.limetheme.Location = new System.Drawing.Point(122, 70);
            this.limetheme.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.limetheme.Name = "limetheme";
            this.limetheme.Size = new System.Drawing.Size(63, 25);
            this.limetheme.TabIndex = 74;
            this.limetheme.Text = "Lime";
            this.limetheme.UseVisualStyleBackColor = true;
            this.limetheme.CheckedChanged += new System.EventHandler(this.limetheme_CheckedChanged);
            // 
            // magmatheme
            // 
            this.magmatheme.AutoSize = true;
            this.magmatheme.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.magmatheme.ForeColor = System.Drawing.Color.Tomato;
            this.magmatheme.Location = new System.Drawing.Point(30, 70);
            this.magmatheme.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.magmatheme.Name = "magmatheme";
            this.magmatheme.Size = new System.Drawing.Size(83, 25);
            this.magmatheme.TabIndex = 73;
            this.magmatheme.Text = "Magma";
            this.magmatheme.UseVisualStyleBackColor = true;
            this.magmatheme.CheckedChanged += new System.EventHandler(this.magmatheme_CheckedChanged);
            // 
            // oceantheme
            // 
            this.oceantheme.AutoSize = true;
            this.oceantheme.Checked = true;
            this.oceantheme.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.oceantheme.ForeColor = System.Drawing.Color.DodgerBlue;
            this.oceantheme.Location = new System.Drawing.Point(30, 40);
            this.oceantheme.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.oceantheme.Name = "oceantheme";
            this.oceantheme.Size = new System.Drawing.Size(74, 25);
            this.oceantheme.TabIndex = 72;
            this.oceantheme.TabStop = true;
            this.oceantheme.Text = "Ocean";
            this.oceantheme.UseVisualStyleBackColor = true;
            this.oceantheme.CheckedChanged += new System.EventHandler(this.oceantheme_CheckedChanged);
            // 
            // zergtheme
            // 
            this.zergtheme.AutoSize = true;
            this.zergtheme.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.zergtheme.ForeColor = System.Drawing.Color.MediumOrchid;
            this.zergtheme.Location = new System.Drawing.Point(30, 101);
            this.zergtheme.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.zergtheme.Name = "zergtheme";
            this.zergtheme.Size = new System.Drawing.Size(62, 25);
            this.zergtheme.TabIndex = 71;
            this.zergtheme.Text = "Zerg";
            this.zergtheme.UseVisualStyleBackColor = true;
            this.zergtheme.CheckedChanged += new System.EventHandler(this.zergtheme_CheckedChanged);
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
            this.btnOk.Location = new System.Drawing.Point(215, 417);
            this.btnOk.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(71, 31);
            this.btnOk.TabIndex = 77;
            this.btnOk.Tag = "themeable";
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = false;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.Location = new System.Drawing.Point(14, 337);
            this.checkBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(116, 25);
            this.checkBox2.TabIndex = 78;
            this.checkBox2.Text = "Auto lock in";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox3.Location = new System.Drawing.Point(14, 367);
            this.checkBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(203, 25);
            this.checkBox3.TabIndex = 79;
            this.checkBox3.Text = "Start Lain with Windows";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // txtTimer
            // 
            this.txtTimer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtTimer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTimer.Enabled = false;
            this.txtTimer.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTimer.ForeColor = System.Drawing.Color.White;
            this.txtTimer.Location = new System.Drawing.Point(134, 336);
            this.txtTimer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtTimer.Name = "txtTimer";
            this.txtTimer.ShortcutsEnabled = false;
            this.txtTimer.Size = new System.Drawing.Size(50, 29);
            this.txtTimer.TabIndex = 80;
            this.txtTimer.Text = "2";
            this.txtTimer.TextChanged += new System.EventHandler(this.txtTimer_TextChanged);
            this.txtTimer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtTimer_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(188, 338);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 21);
            this.label1.TabIndex = 81;
            this.label1.Text = "minutes";
            // 
            // sizePanel
            // 
            this.sizePanel.Controls.Add(this.label2);
            this.sizePanel.Controls.Add(this.rLarge);
            this.sizePanel.Controls.Add(this.rNormal);
            this.sizePanel.Controls.Add(this.rSmall);
            this.sizePanel.Location = new System.Drawing.Point(10, 139);
            this.sizePanel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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
            this.rLarge.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rLarge.Name = "rLarge";
            this.rLarge.Size = new System.Drawing.Size(69, 25);
            this.rLarge.TabIndex = 79;
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
            this.rNormal.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rNormal.Name = "rNormal";
            this.rNormal.Size = new System.Drawing.Size(82, 25);
            this.rNormal.TabIndex = 78;
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
            this.rSmall.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rSmall.Name = "rSmall";
            this.rSmall.Size = new System.Drawing.Size(67, 25);
            this.rSmall.TabIndex = 77;
            this.rSmall.Text = "Small";
            this.rSmall.UseVisualStyleBackColor = true;
            this.rSmall.CheckedChanged += new System.EventHandler(this.rSmall_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox4.Location = new System.Drawing.Point(14, 306);
            this.checkBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(145, 25);
            this.checkBox4.TabIndex = 84;
            this.checkBox4.Text = "Hide passwords";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox4_CheckedChanged);
            // 
            // OptionsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.CancelButton = this.btnOk;
            this.ClientSize = new System.Drawing.Size(296, 458);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.sizePanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTimer);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.minimaltheme);
            this.Controls.Add(this.carameltheme);
            this.Controls.Add(this.limetheme);
            this.Controls.Add(this.magmatheme);
            this.Controls.Add(this.oceantheme);
            this.Controls.Add(this.zergtheme);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.checkBox1);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
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

        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.RadioButton minimaltheme;
        private System.Windows.Forms.RadioButton carameltheme;
        private System.Windows.Forms.RadioButton limetheme;
        private System.Windows.Forms.RadioButton magmatheme;
        private System.Windows.Forms.RadioButton oceantheme;
        private System.Windows.Forms.RadioButton zergtheme;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.TextBox txtTimer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel sizePanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rLarge;
        private System.Windows.Forms.RadioButton rNormal;
        private System.Windows.Forms.RadioButton rSmall;
        private System.Windows.Forms.CheckBox checkBox4;
    }
}