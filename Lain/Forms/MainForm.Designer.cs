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
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.helperMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openLinkBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ediToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.favoriteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnView = new Lain.ModernButton();
            this.btnFavorite = new Lain.ModernButton();
            this.Column5 = new Lain.TagColumn();
            this.moreMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnAnalyze = new System.Windows.Forms.ToolStripMenuItem();
            this.modifyRemoveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnUpdate = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.topPanel = new System.Windows.Forms.Panel();
            this.headerLayout = new System.Windows.Forms.TableLayoutPanel();
            this.label27 = new Lain.FluentLabel();
            this.searchBox = new Lain.InputBox();
            this.searchIcon = new Lain.IconLabel();
            this.txtSearch = new Lain.FluentTextBox();
            this.clearPic = new Lain.IconLabel();
            this.btnAdd = new Lain.ModernButton();
            this.btnLock = new Lain.ModernButton();
            this.btnOptions = new Lain.ModernButton();
            this.btnMore = new Lain.ModernButton();
            this.actionBar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCopyUser = new Lain.ModernButton();
            this.btnCopyPassword = new Lain.ModernButton();
            this.btnOpenLink = new Lain.ModernButton();
            this.actionDivider = new System.Windows.Forms.Panel();
            this.btnModify = new Lain.ModernButton();
            this.btnRemove = new Lain.ModernButton();
            this.helperPanel = new System.Windows.Forms.Panel();
            this.leftPanel = new Lain.Card();
            this.AccountView = new Lain.FluentGrid();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblEmpty = new Lain.FluentLabel();
            this.statusBar = new System.Windows.Forms.Panel();
            this.lblStatus = new Lain.FluentLabel();
            this.lblHint = new Lain.FluentLabel();
            this.timerAutoLock = new System.Windows.Forms.Timer(this.components);
            this.toastTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.helperMenu.SuspendLayout();
            this.moreMenu.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.headerLayout.SuspendLayout();
            this.searchBox.SuspendLayout();
            this.actionBar.SuspendLayout();
            this.helperPanel.SuspendLayout();
            this.leftPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AccountView)).BeginInit();
            this.statusBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // helperMenu
            // 
            this.helperMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.toolStripMenuItem1,
            this.openLinkBtn,
            this.favoriteMenuItem,
            this.toolStripSeparator1,
            this.newToolStripMenuItem,
            this.ediToolStripMenuItem,
            this.removeToolStripMenuItem});
            this.helperMenu.Name = "helperMenu";
            this.helperMenu.ShowImageMargin = true;
            this.helperMenu.Size = new System.Drawing.Size(240, 200);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Q";
            this.copyToolStripMenuItem.Text = "Copy username";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeyDisplayString = "Ctrl+W";
            this.toolStripMenuItem1.Text = "Copy password";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // openLinkBtn
            // 
            this.openLinkBtn.Name = "openLinkBtn";
            this.openLinkBtn.ShortcutKeyDisplayString = "Ctrl+E";
            this.openLinkBtn.Text = "Open link";
            this.openLinkBtn.Click += new System.EventHandler(this.openLinkBtn_Click);
            // 
            // favoriteMenuItem
            // 
            this.favoriteMenuItem.Name = "favoriteMenuItem";
            this.favoriteMenuItem.ShortcutKeyDisplayString = "Ctrl+D";
            this.favoriteMenuItem.Text = "Add to favorites";
            this.favoriteMenuItem.Click += new System.EventHandler(this.favorite_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+N";
            this.newToolStripMenuItem.Text = "New account";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // ediToolStripMenuItem
            // 
            this.ediToolStripMenuItem.Name = "ediToolStripMenuItem";
            this.ediToolStripMenuItem.ShortcutKeyDisplayString = "Enter";
            this.ediToolStripMenuItem.Text = "Edit";
            this.ediToolStripMenuItem.Click += new System.EventHandler(this.ediToolStripMenuItem_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.ShortcutKeyDisplayString = "Del";
            this.removeToolStripMenuItem.Text = "Delete";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // moreMenu
            // 
            this.moreMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAnalyze,
            this.modifyRemoveAll,
            this.toolStripSeparator2,
            this.btnUpdate,
            this.btnAbout});
            this.moreMenu.Name = "moreMenu";
            this.moreMenu.Size = new System.Drawing.Size(260, 140);
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.ShortcutKeyDisplayString = "Ctrl+R";
            this.btnAnalyze.Text = "Security report";
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // modifyRemoveAll
            // 
            this.modifyRemoveAll.Name = "modifyRemoveAll";
            this.modifyRemoveAll.Text = "Delete all accounts...";
            this.modifyRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Text = "Check for updates";
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Text = "About Lain";
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.headerLayout);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(20, 16, 16, 8);
            this.topPanel.Size = new System.Drawing.Size(1000, 68);
            this.topPanel.TabIndex = 0;
            // 
            // headerLayout
            // 
            this.headerLayout.ColumnCount = 7;
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.headerLayout.Controls.Add(this.label27, 0, 0);
            this.headerLayout.Controls.Add(this.searchBox, 1, 0);
            this.headerLayout.Controls.Add(this.btnView, 2, 0);
            this.headerLayout.Controls.Add(this.btnAdd, 3, 0);
            this.headerLayout.Controls.Add(this.btnLock, 4, 0);
            this.headerLayout.Controls.Add(this.btnOptions, 5, 0);
            this.headerLayout.Controls.Add(this.btnMore, 6, 0);
            this.headerLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headerLayout.Location = new System.Drawing.Point(20, 16);
            this.headerLayout.Margin = new System.Windows.Forms.Padding(0);
            this.headerLayout.Name = "headerLayout";
            this.headerLayout.RowCount = 1;
            this.headerLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerLayout.Size = new System.Drawing.Size(964, 44);
            this.headerLayout.TabIndex = 0;
            // 
            // label27
            // 
            this.label27.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label27.AutoSize = true;
            this.label27.Font = Fluent.Display(16F);
            this.label27.Margin = new System.Windows.Forms.Padding(0, 0, 24, 2);
            this.label27.Name = "label27";
            this.label27.Tag = "themeable";
            this.label27.Text = "Lain";
            // 
            // searchBox
            // 
            this.searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBox.Controls.Add(this.searchIcon);
            this.searchBox.Controls.Add(this.txtSearch);
            this.searchBox.Controls.Add(this.clearPic);
            this.searchBox.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(500, 36);
            this.searchBox.TabIndex = 0;
            // 
            // searchIcon
            // 
            this.searchIcon.Fallback = "";
            this.searchIcon.Glyph = "\uE721";
            this.searchIcon.IconSize = 10F;
            this.searchIcon.Name = "searchIcon";
            this.searchIcon.Size = new System.Drawing.Size(30, 36);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSearch.Font = Fluent.Ui(11F);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(400, 19);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtPassword_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // clearPic
            // 
            this.clearPic.Clickable = true;
            this.clearPic.Fallback = "x";
            this.clearPic.Glyph = "\uE711";
            this.clearPic.IconSize = 8F;
            this.clearPic.Name = "clearPic";
            this.clearPic.Size = new System.Drawing.Size(30, 30);
            this.clearPic.Visible = false;
            this.clearPic.Click += new System.EventHandler(this.clearPic_Click);
            // 
            // btnView
            // 
            this.btnView.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnView.Glyph = "\uE71C";
            this.btnView.Margin = new System.Windows.Forms.Padding(0, 0, 16, 0);
            this.btnView.Name = "btnView";
            this.btnView.Padding = new System.Windows.Forms.Padding(10, 0, 12, 0);
            this.btnView.Size = new System.Drawing.Size(36, 36);
            this.btnView.TabIndex = 1;
            this.btnView.Variant = Lain.ButtonVariant.Subtle;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnAdd.Glyph = "\uE710";
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0, 0, 8, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(138, 36);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "New account";
            this.btnAdd.Variant = Lain.ButtonVariant.Primary;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnLock
            // 
            this.btnLock.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnLock.Glyph = "\uE72E";
            this.btnLock.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnLock.Name = "btnLock";
            this.btnLock.Size = new System.Drawing.Size(36, 36);
            this.btnLock.TabIndex = 3;
            this.btnLock.Variant = Lain.ButtonVariant.Subtle;
            this.btnLock.Click += new System.EventHandler(this.btnLock_Click);
            // 
            // btnOptions
            // 
            this.btnOptions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnOptions.Glyph = "\uE713";
            this.btnOptions.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(36, 36);
            this.btnOptions.TabIndex = 4;
            this.btnOptions.Variant = Lain.ButtonVariant.Subtle;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // btnMore
            // 
            this.btnMore.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnMore.Glyph = "\uE712";
            this.btnMore.Margin = new System.Windows.Forms.Padding(0);
            this.btnMore.Name = "btnMore";
            this.btnMore.Size = new System.Drawing.Size(36, 36);
            this.btnMore.TabIndex = 5;
            this.btnMore.Variant = Lain.ButtonVariant.Subtle;
            this.btnMore.Click += new System.EventHandler(this.btnMore_Click);
            // 
            // actionBar
            // 
            this.actionBar.Controls.Add(this.btnCopyUser);
            this.actionBar.Controls.Add(this.btnCopyPassword);
            this.actionBar.Controls.Add(this.btnOpenLink);
            this.actionBar.Controls.Add(this.btnFavorite);
            this.actionBar.Controls.Add(this.actionDivider);
            this.actionBar.Controls.Add(this.btnModify);
            this.actionBar.Controls.Add(this.btnRemove);
            this.actionBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.actionBar.Name = "actionBar";
            this.actionBar.Padding = new System.Windows.Forms.Padding(12, 2, 16, 6);
            this.actionBar.Size = new System.Drawing.Size(1000, 44);
            this.actionBar.TabIndex = 1;
            this.actionBar.WrapContents = false;
            // 
            // btnCopyUser
            // 
            this.btnCopyUser.AutoSize = true;
            this.btnCopyUser.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCopyUser.Glyph = "\uE77B";
            this.btnCopyUser.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnCopyUser.Name = "btnCopyUser";
            this.btnCopyUser.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnCopyUser.TabIndex = 0;
            this.btnCopyUser.Text = "Copy username";
            this.btnCopyUser.Variant = Lain.ButtonVariant.Subtle;
            this.btnCopyUser.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // btnCopyPassword
            // 
            this.btnCopyPassword.AutoSize = true;
            this.btnCopyPassword.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnCopyPassword.Glyph = "\uE8D7";
            this.btnCopyPassword.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnCopyPassword.Name = "btnCopyPassword";
            this.btnCopyPassword.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnCopyPassword.TabIndex = 1;
            this.btnCopyPassword.Text = "Copy password";
            this.btnCopyPassword.Variant = Lain.ButtonVariant.Subtle;
            this.btnCopyPassword.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // btnOpenLink
            // 
            this.btnOpenLink.AutoSize = true;
            this.btnOpenLink.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnOpenLink.Glyph = "\uE71B";
            this.btnOpenLink.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnOpenLink.Name = "btnOpenLink";
            this.btnOpenLink.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnOpenLink.TabIndex = 2;
            this.btnOpenLink.Text = "Open link";
            this.btnOpenLink.Variant = Lain.ButtonVariant.Subtle;
            this.btnOpenLink.Click += new System.EventHandler(this.openLinkBtn_Click);
            // 
            // btnFavorite
            // 
            this.btnFavorite.AutoSize = true;
            this.btnFavorite.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFavorite.Glyph = "\uE734";
            this.btnFavorite.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnFavorite.Name = "btnFavorite";
            this.btnFavorite.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnFavorite.TabIndex = 3;
            this.btnFavorite.Text = "Favorite";
            this.btnFavorite.Variant = Lain.ButtonVariant.Subtle;
            this.btnFavorite.Click += new System.EventHandler(this.favorite_Click);
            // 
            // actionDivider
            // 
            this.actionDivider.Margin = new System.Windows.Forms.Padding(8, 7, 10, 0);
            this.actionDivider.Name = "actionDivider";
            this.actionDivider.Size = new System.Drawing.Size(1, 20);
            // 
            // btnModify
            // 
            this.btnModify.AutoSize = true;
            this.btnModify.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnModify.Glyph = "\uE70F";
            this.btnModify.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.btnModify.Name = "btnModify";
            this.btnModify.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnModify.TabIndex = 4;
            this.btnModify.Text = "Edit";
            this.btnModify.Variant = Lain.ButtonVariant.Subtle;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.AutoSize = true;
            this.btnRemove.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemove.Glyph = "\uE74D";
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnRemove.TabIndex = 5;
            this.btnRemove.Text = "Delete";
            this.btnRemove.Variant = Lain.ButtonVariant.Danger;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // helperPanel
            // 
            this.helperPanel.Controls.Add(this.leftPanel);
            this.helperPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helperPanel.Name = "helperPanel";
            this.helperPanel.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.helperPanel.TabIndex = 2;
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.AccountView);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Padding = new System.Windows.Forms.Padding(4, 2, 4, 4);
            this.leftPanel.TabIndex = 0;
            // 
            // AccountView
            // 
            this.AccountView.AllowUserToAddRows = false;
            this.AccountView.AllowUserToDeleteRows = false;
            this.AccountView.AllowUserToOrderColumns = true;
            this.AccountView.AllowUserToResizeRows = false;
            this.AccountView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.AccountView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AccountView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.AccountView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle.Font = Fluent.UiSemibold(9.5F);
            headerStyle.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            headerStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.AccountView.ColumnHeadersDefaultCellStyle = headerStyle;
            this.AccountView.ColumnHeadersHeight = 40;
            this.AccountView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.AccountView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.AccountView.ContextMenuStrip = this.helperMenu;
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Font = Fluent.Ui(11F);
            cellStyle.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            cellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.AccountView.DefaultCellStyle = cellStyle;
            this.AccountView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AccountView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.AccountView.EnableHeadersVisualStyles = false;
            this.AccountView.MultiSelect = true;
            this.AccountView.Name = "AccountView";
            this.AccountView.ReadOnly = true;
            this.AccountView.RowHeadersVisible = false;
            this.AccountView.RowTemplate.Height = 38;
            this.AccountView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AccountView.ShowCellErrors = false;
            this.AccountView.ShowCellToolTips = false;
            this.AccountView.ShowEditingIcon = false;
            this.AccountView.StandardTab = true;
            this.AccountView.TabIndex = 0;
            this.AccountView.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.AccountView_CellMouseDoubleClick);
            this.AccountView.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.AccountView_CellMouseDown);
            this.AccountView.SelectionChanged += new System.EventHandler(this.AccountView_SelectionChanged);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Title";
            this.Column1.MinimumWidth = 80;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Username";
            this.Column2.MinimumWidth = 80;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Password";
            this.Column3.MinimumWidth = 80;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Link";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Visible = false;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Tags";
            this.Column5.MinimumWidth = 60;
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Visible = false;
            // 
            // lblEmpty
            // 
            this.lblEmpty.Font = Fluent.Ui(11F);
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmpty.Visible = false;
            // 
            // statusBar
            // 
            this.statusBar.Controls.Add(this.lblStatus);
            this.statusBar.Controls.Add(this.lblHint);
            this.statusBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusBar.Name = "statusBar";
            this.statusBar.Padding = new System.Windows.Forms.Padding(30, 6, 30, 8);
            this.statusBar.Size = new System.Drawing.Size(1000, 42);
            this.statusBar.TabIndex = 3;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblStatus.Font = Fluent.Ui(9.5F);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHint
            // 
            this.lblHint.AutoSize = true;
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblHint.Font = Fluent.Ui(9.5F);
            this.lblHint.Name = "lblHint";
            this.lblHint.Text = "Ctrl+N new  ·  Ctrl+F search  ·  Ctrl+Q / Ctrl+W copy  ·  Ctrl+L lock";
            this.lblHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // timerAutoLock
            // 
            this.timerAutoLock.Tick += new System.EventHandler(this.timerAutoLock_Tick);
            // 
            // toastTimer
            // 
            this.toastTimer.Interval = 2200;
            this.toastTimer.Tick += new System.EventHandler(this.toastTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1000, 620);
            this.Controls.Add(this.helperPanel);
            this.Controls.Add(this.actionBar);
            this.Controls.Add(this.topPanel);
            this.Controls.Add(this.statusBar);
            this.Font = Fluent.Ui(10F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(780, 480);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lain";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Deactivate += new System.EventHandler(this.MainForm_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.helperMenu.ResumeLayout(false);
            this.moreMenu.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.headerLayout.ResumeLayout(false);
            this.headerLayout.PerformLayout();
            this.searchBox.ResumeLayout(false);
            this.searchBox.PerformLayout();
            this.actionBar.ResumeLayout(false);
            this.actionBar.PerformLayout();
            this.helperPanel.ResumeLayout(false);
            this.leftPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AccountView)).EndInit();
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip helperMenu;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem openLinkBtn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ediToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip moreMenu;
        private System.Windows.Forms.ToolStripMenuItem btnAnalyze;
        private System.Windows.Forms.ToolStripMenuItem modifyRemoveAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem btnUpdate;
        private System.Windows.Forms.ToolStripMenuItem btnAbout;
        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.TableLayoutPanel headerLayout;
        private Lain.FluentLabel label27;
        private Lain.InputBox searchBox;
        private Lain.IconLabel searchIcon;
        private Lain.FluentTextBox txtSearch;
        private Lain.IconLabel clearPic;
        private Lain.ModernButton btnAdd;
        private Lain.ModernButton btnLock;
        private Lain.ModernButton btnOptions;
        private Lain.ModernButton btnMore;
        private System.Windows.Forms.FlowLayoutPanel actionBar;
        private Lain.ModernButton btnCopyUser;
        private Lain.ModernButton btnCopyPassword;
        private Lain.ModernButton btnOpenLink;
        private System.Windows.Forms.Panel actionDivider;
        private Lain.ModernButton btnModify;
        private Lain.ModernButton btnRemove;
        private System.Windows.Forms.Panel helperPanel;
        private Lain.Card leftPanel;
        internal Lain.FluentGrid AccountView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private Lain.TagColumn Column5;
        private System.Windows.Forms.ToolStripMenuItem favoriteMenuItem;
        private Lain.ModernButton btnView;
        private Lain.ModernButton btnFavorite;
        private Lain.FluentLabel lblEmpty;
        private System.Windows.Forms.Panel statusBar;
        private Lain.FluentLabel lblStatus;
        private Lain.FluentLabel lblHint;
        private System.Windows.Forms.Timer timerAutoLock;
        private System.Windows.Forms.Timer toastTimer;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
