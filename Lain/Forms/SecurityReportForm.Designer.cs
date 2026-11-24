namespace Lain
{
    partial class SecurityReportForm
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
            System.Windows.Forms.DataGridViewCellStyle headerStyle = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle cellStyle = new System.Windows.Forms.DataGridViewCellStyle();
            this.shieldIcon = new Lain.IconLabel();
            this.lblTitle = new Lain.FluentLabel();
            this.lblSubtitle = new Lain.FluentLabel();
            this.gridCard = new Lain.Card();
            this.grid = new Lain.FluentGrid();
            this.colIssue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAccount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDetails = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblEmpty = new Lain.FluentLabel();
            this.lblStatus = new Lain.FluentLabel();
            this.footer = new Lain.FooterPanel();
            this.btnBreach = new Lain.ModernButton();
            this.btnCopy = new Lain.ModernButton();
            this.btnEdit = new Lain.ModernButton();
            this.btnClose = new Lain.ModernButton();
            this.gridCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            // 
            // shieldIcon
            // 
            this.shieldIcon.Active = true;
            this.shieldIcon.Glyph = "\uEA18";
            this.shieldIcon.IconSize = 18F;
            this.shieldIcon.Location = new System.Drawing.Point(24, 18);
            this.shieldIcon.Name = "shieldIcon";
            this.shieldIcon.Size = new System.Drawing.Size(36, 36);
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = Fluent.Display(16F);
            this.lblTitle.Location = new System.Drawing.Point(62, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Security report";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubtitle.AutoEllipsis = true;
            this.lblSubtitle.Location = new System.Drawing.Point(29, 62);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(702, 22);
            // 
            // gridCard
            // 
            this.gridCard.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.gridCard.Controls.Add(this.grid);
            this.gridCard.Location = new System.Drawing.Point(30, 96);
            this.gridCard.Name = "gridCard";
            this.gridCard.Padding = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.gridCard.Size = new System.Drawing.Size(700, 330);
            this.gridCard.TabIndex = 0;
            // 
            // grid
            // 
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.grid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            headerStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            headerStyle.Font = Fluent.UiSemibold(9.5F);
            headerStyle.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            headerStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.ColumnHeadersDefaultCellStyle = headerStyle;
            this.grid.ColumnHeadersHeight = 36;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIssue,
            this.colAccount,
            this.colDetails});
            cellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyle.Font = Fluent.Ui(10.5F);
            cellStyle.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            cellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = cellStyle;
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grid.EnableHeadersVisualStyles = false;
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.ReadOnly = true;
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 36;
            this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grid.ShowCellErrors = false;
            this.grid.ShowCellToolTips = true;
            this.grid.ShowEditingIcon = false;
            this.grid.StandardTab = true;
            this.grid.TabIndex = 0;
            // 
            // colIssue
            // 
            this.colIssue.FillWeight = 22F;
            this.colIssue.HeaderText = "Issue";
            this.colIssue.MinimumWidth = 90;
            this.colIssue.Name = "colIssue";
            this.colIssue.ReadOnly = true;
            this.colIssue.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colAccount
            // 
            this.colAccount.FillWeight = 34F;
            this.colAccount.HeaderText = "Account";
            this.colAccount.MinimumWidth = 100;
            this.colAccount.Name = "colAccount";
            this.colAccount.ReadOnly = true;
            this.colAccount.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // colDetails
            // 
            this.colDetails.FillWeight = 64F;
            this.colDetails.HeaderText = "Details";
            this.colDetails.MinimumWidth = 120;
            this.colDetails.Name = "colDetails";
            this.colDetails.ReadOnly = true;
            this.colDetails.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lblEmpty
            // 
            this.lblEmpty.Name = "lblEmpty";
            this.lblEmpty.Size = new System.Drawing.Size(400, 60);
            this.lblEmpty.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmpty.Visible = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.AutoEllipsis = true;
            this.lblStatus.Location = new System.Drawing.Point(30, 434);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(700, 40);
            // 
            // footer
            // 
            this.footer.Controls.Add(this.btnBreach);
            this.footer.Controls.Add(this.btnCopy);
            this.footer.Controls.Add(this.btnEdit);
            this.footer.Controls.Add(this.btnClose);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Location = new System.Drawing.Point(0, 484);
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(760, 66);
            this.footer.TabIndex = 1;
            // 
            // btnBreach
            // 
            this.btnBreach.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBreach.Glyph = "\uE774";
            this.btnBreach.Location = new System.Drawing.Point(30, 15);
            this.btnBreach.Name = "btnBreach";
            this.btnBreach.Size = new System.Drawing.Size(190, 36);
            this.btnBreach.TabIndex = 0;
            this.btnBreach.Text = "Check for breaches";
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCopy.Glyph = "\uE8C8";
            this.btnCopy.Location = new System.Drawing.Point(226, 15);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(140, 36);
            this.btnCopy.TabIndex = 1;
            this.btnCopy.Text = "Copy report";
            this.btnCopy.Variant = Lain.ButtonVariant.Subtle;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Enabled = false;
            this.btnEdit.Glyph = "\uE70F";
            this.btnEdit.Location = new System.Drawing.Point(474, 15);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(140, 36);
            this.btnEdit.TabIndex = 2;
            this.btnEdit.Text = "Edit account";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(620, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 36);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.Variant = Lain.ButtonVariant.Primary;
            // 
            // SecurityReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(760, 550);
            this.Controls.Add(this.shieldIcon);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblSubtitle);
            this.Controls.Add(this.gridCard);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.footer);
            this.DoubleBuffered = true;
            this.Font = Fluent.Ui(10F);
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 420);
            this.Name = "SecurityReportForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Security report";
            this.gridCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Lain.IconLabel shieldIcon;
        private Lain.FluentLabel lblTitle;
        private Lain.FluentLabel lblSubtitle;
        private Lain.Card gridCard;
        private Lain.FluentGrid grid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIssue;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAccount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDetails;
        private Lain.FluentLabel lblEmpty;
        private Lain.FluentLabel lblStatus;
        private Lain.FooterPanel footer;
        private Lain.ModernButton btnBreach;
        private Lain.ModernButton btnCopy;
        private Lain.ModernButton btnEdit;
        private Lain.ModernButton btnClose;
    }
}
