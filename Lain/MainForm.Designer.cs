namespace Lain
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.AccountView = new System.Windows.Forms.TreeView();
            this.helperMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ediToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnModify = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnLock = new System.Windows.Forms.Button();
            this.btnAbout = new System.Windows.Forms.Button();
            this.modifyRemoveAll = new System.Windows.Forms.Button();
            this.btnOptions = new System.Windows.Forms.Button();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.topPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.timerAutoLock = new System.Windows.Forms.Timer(this.components);
            this.helperPanel = new System.Windows.Forms.Panel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.helperMenu.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.helperPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // AccountView
            // 
            this.AccountView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.AccountView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AccountView.ContextMenuStrip = this.helperMenu;
            this.AccountView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AccountView.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AccountView.ForeColor = System.Drawing.Color.White;
            this.AccountView.Location = new System.Drawing.Point(0, 0);
            this.AccountView.Margin = new System.Windows.Forms.Padding(2);
            this.AccountView.Name = "AccountView";
            this.AccountView.Size = new System.Drawing.Size(719, 569);
            this.AccountView.TabIndex = 22;
            this.AccountView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.AccountView_BeforeExpand);
            this.AccountView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.AccountView_NodeMouseClick);
            // 
            // helperMenu
            // 
            this.helperMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.helperMenu.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.helperMenu.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.helperMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.helperMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.newToolStripMenuItem,
            this.ediToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.helperMenu.Name = "helperMenu";
            this.helperMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.helperMenu.ShowImageMargin = false;
            this.helperMenu.Size = new System.Drawing.Size(113, 132);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(112, 32);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.newToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(112, 32);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // ediToolStripMenuItem
            // 
            this.ediToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ediToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.ediToolStripMenuItem.Name = "ediToolStripMenuItem";
            this.ediToolStripMenuItem.Size = new System.Drawing.Size(112, 32);
            this.ediToolStripMenuItem.Text = "Edit";
            this.ediToolStripMenuItem.Click += new System.EventHandler(this.ediToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(112, 32);
            this.removeToolStripMenuItem.Text = "Delete";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnAdd.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnAdd.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnAdd.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Location = new System.Drawing.Point(10, 10);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(110, 39);
            this.btnAdd.TabIndex = 63;
            this.btnAdd.Tag = "themeable";
            this.btnAdd.Text = "New";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnRemove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnRemove.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnRemove.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnRemove.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.ForeColor = System.Drawing.Color.White;
            this.btnRemove.Location = new System.Drawing.Point(10, 99);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(2);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(110, 42);
            this.btnRemove.TabIndex = 64;
            this.btnRemove.Tag = "themeable";
            this.btnRemove.Text = "Delete";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnModify
            // 
            this.btnModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnModify.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnModify.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnModify.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnModify.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnModify.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnModify.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnModify.ForeColor = System.Drawing.Color.White;
            this.btnModify.Location = new System.Drawing.Point(10, 53);
            this.btnModify.Margin = new System.Windows.Forms.Padding(2);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(110, 42);
            this.btnModify.TabIndex = 65;
            this.btnModify.Tag = "themeable";
            this.btnModify.Text = "Edit";
            this.btnModify.UseVisualStyleBackColor = false;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnReset.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnReset.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.Location = new System.Drawing.Point(10, 468);
            this.btnReset.Margin = new System.Windows.Forms.Padding(2);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(110, 42);
            this.btnReset.TabIndex = 66;
            this.btnReset.Tag = "themeable";
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnLock
            // 
            this.btnLock.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLock.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLock.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnLock.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnLock.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLock.ForeColor = System.Drawing.Color.White;
            this.btnLock.Location = new System.Drawing.Point(10, 145);
            this.btnLock.Margin = new System.Windows.Forms.Padding(2);
            this.btnLock.Name = "btnLock";
            this.btnLock.Size = new System.Drawing.Size(110, 42);
            this.btnLock.TabIndex = 67;
            this.btnLock.Tag = "themeable";
            this.btnLock.Text = "Lock";
            this.btnLock.UseVisualStyleBackColor = false;
            this.btnLock.Click += new System.EventHandler(this.btnLock_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbout.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnAbout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAbout.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnAbout.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnAbout.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnAbout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAbout.ForeColor = System.Drawing.Color.White;
            this.btnAbout.Location = new System.Drawing.Point(10, 560);
            this.btnAbout.Margin = new System.Windows.Forms.Padding(2);
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(110, 42);
            this.btnAbout.TabIndex = 70;
            this.btnAbout.Tag = "themeable";
            this.btnAbout.Text = "About";
            this.btnAbout.UseVisualStyleBackColor = false;
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // modifyRemoveAll
            // 
            this.modifyRemoveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.modifyRemoveAll.BackColor = System.Drawing.Color.DodgerBlue;
            this.modifyRemoveAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.modifyRemoveAll.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.modifyRemoveAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.modifyRemoveAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.modifyRemoveAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.modifyRemoveAll.ForeColor = System.Drawing.Color.White;
            this.modifyRemoveAll.Location = new System.Drawing.Point(10, 422);
            this.modifyRemoveAll.Margin = new System.Windows.Forms.Padding(2);
            this.modifyRemoveAll.Name = "modifyRemoveAll";
            this.modifyRemoveAll.Size = new System.Drawing.Size(110, 42);
            this.modifyRemoveAll.TabIndex = 69;
            this.modifyRemoveAll.Tag = "themeable";
            this.modifyRemoveAll.Text = "Delete all";
            this.modifyRemoveAll.UseVisualStyleBackColor = false;
            this.modifyRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOptions.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOptions.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnOptions.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnOptions.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnOptions.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnOptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOptions.ForeColor = System.Drawing.Color.White;
            this.btnOptions.Location = new System.Drawing.Point(10, 191);
            this.btnOptions.Margin = new System.Windows.Forms.Padding(2);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(110, 42);
            this.btnOptions.TabIndex = 68;
            this.btnOptions.Tag = "themeable";
            this.btnOptions.Text = "Options";
            this.btnOptions.UseVisualStyleBackColor = false;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // leftPanel
            // 
            this.leftPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftPanel.Controls.Add(this.AccountView);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(2);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(721, 571);
            this.leftPanel.TabIndex = 71;
            // 
            // rightPanel
            // 
            this.rightPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rightPanel.Controls.Add(this.btnUpdate);
            this.rightPanel.Controls.Add(this.btnOptions);
            this.rightPanel.Controls.Add(this.btnAdd);
            this.rightPanel.Controls.Add(this.btnAbout);
            this.rightPanel.Controls.Add(this.btnRemove);
            this.rightPanel.Controls.Add(this.modifyRemoveAll);
            this.rightPanel.Controls.Add(this.btnModify);
            this.rightPanel.Controls.Add(this.btnReset);
            this.rightPanel.Controls.Add(this.btnLock);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(721, 0);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(2);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(132, 614);
            this.rightPanel.TabIndex = 72;
            // 
            // topPanel
            // 
            this.topPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.topPanel.Controls.Add(this.pictureBox1);
            this.topPanel.Controls.Add(this.txtSearch);
            this.topPanel.Controls.Add(this.label27);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(2);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(721, 43);
            this.topPanel.TabIndex = 73;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(680, 2);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(2);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(35, 34);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 71;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.ForeColor = System.Drawing.Color.White;
            this.txtSearch.Location = new System.Drawing.Point(79, 4);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(2);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(636, 27);
            this.txtSearch.TabIndex = 74;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label27.Location = new System.Drawing.Point(2, 4);
            this.label27.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(77, 28);
            this.label27.TabIndex = 71;
            this.label27.Tag = "themeable";
            this.label27.Text = "Search:";
            // 
            // timerAutoLock
            // 
            this.timerAutoLock.Interval = 120000;
            this.timerAutoLock.Tick += new System.EventHandler(this.timerAutoLock_Tick);
            // 
            // helperPanel
            // 
            this.helperPanel.Controls.Add(this.leftPanel);
            this.helperPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helperPanel.Location = new System.Drawing.Point(0, 43);
            this.helperPanel.Name = "helperPanel";
            this.helperPanel.Size = new System.Drawing.Size(721, 571);
            this.helperPanel.TabIndex = 74;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnUpdate.FlatAppearance.BorderColor = System.Drawing.Color.DodgerBlue;
            this.btnUpdate.FlatAppearance.MouseDownBackColor = System.Drawing.Color.RoyalBlue;
            this.btnUpdate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.RoyalBlue;
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Location = new System.Drawing.Point(10, 514);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(2);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(110, 42);
            this.btnUpdate.TabIndex = 71;
            this.btnUpdate.Tag = "themeable";
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(853, 614);
            this.Controls.Add(this.helperPanel);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.rightPanel);
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lain ";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.helperMenu.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.helperPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView AccountView;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnLock;
        private System.Windows.Forms.Button btnAbout;
        private System.Windows.Forms.Button modifyRemoveAll;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.ContextMenuStrip helperMenu;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ediToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.Timer timerAutoLock;
        private System.Windows.Forms.Panel helperPanel;
        private System.Windows.Forms.Button btnUpdate;
    }
}