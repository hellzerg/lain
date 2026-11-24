namespace Lain
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.l1 = new Lain.FluentLabel();
            this.lblVersion = new Lain.FluentLabel();
            this.lblDescription = new Lain.FluentLabel();
            this.l2 = new System.Windows.Forms.LinkLabel();
            this.footer = new Lain.FooterPanel();
            this.button7 = new Lain.ModernButton();
            this.t1 = new System.Windows.Forms.Timer(this.components);
            this.t2 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(30, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(72, 72);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabStop = false;
            // 
            // l1
            // 
            this.l1.Font = Fluent.Display(20F);
            this.l1.Location = new System.Drawing.Point(120, 22);
            this.l1.Name = "l1";
            this.l1.Size = new System.Drawing.Size(220, 40);
            this.l1.Tag = "themeable";
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(123, 62);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Text = "Version";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(123, 84);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Text = "A simple and secure password manager.";
            // 
            // l2
            // 
            this.l2.ActiveLinkColor = System.Drawing.Color.White;
            this.l2.AutoSize = true;
            this.l2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.l2.Location = new System.Drawing.Point(123, 112);
            this.l2.Name = "l2";
            this.l2.TabIndex = 1;
            this.l2.Tag = "themeable";
            this.l2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.l2_LinkClicked);
            // 
            // footer
            // 
            this.footer.Controls.Add(this.button7);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 150);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(420, 66);
            this.footer.TabIndex = 0;
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button7.Location = new System.Drawing.Point(280, 15);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(110, 36);
            this.button7.TabIndex = 0;
            this.button7.Text = "Close";
            this.button7.Variant = Lain.ButtonVariant.Primary;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // t1
            // 
            this.t1.Interval = 50;
            this.t1.Tick += new System.EventHandler(this.t1_Tick);
            // 
            // t2
            // 
            this.t2.Interval = 50;
            this.t2.Tick += new System.EventHandler(this.t2_Tick);
            // 
            // AboutForm
            // 
            this.AcceptButton = this.button7;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.button7;
            this.ClientSize = new System.Drawing.Size(420, 216);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.l1);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.l2);
            this.Controls.Add(this.footer);
            this.DoubleBuffered = true;
            this.Font = Fluent.Ui(10F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About Lain";
            this.Load += new System.EventHandler(this.AboutForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Lain.FluentLabel l1;
        private Lain.FluentLabel lblVersion;
        private Lain.FluentLabel lblDescription;
        private System.Windows.Forms.LinkLabel l2;
        private Lain.FooterPanel footer;
        private Lain.ModernButton button7;
        private System.Windows.Forms.Timer t1;
        private System.Windows.Forms.Timer t2;
    }
}
