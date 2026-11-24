using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Lain
{
    public sealed partial class MainForm : Form
    {
        readonly string _latestVersionLink = "https://raw.githubusercontent.com/hellzerg/lain/master/version.txt";

        readonly string _noNewVersionMessage = "You already have the latest version!";
        readonly string _betaVersionMessage = "You are using an experimental version!";

        const int ONE_MINUTE_IN_MILLISECONDS = 60000;

        bool IsDialogOpen = false;

        internal static List<LainAccount> Accounts = new List<LainAccount>();
        internal static VaultKey Key;

        // Installs the key (and accounts) produced by a re-key; the list rows hold account references, so it is rebuilt.
        // False when the vault was locked meanwhile (the files on disk are already updated).
        internal static bool ReplaceVault(VaultKey oldKey, VaultKey newKey, List<LainAccount> accounts)
        {
            if (Key != oldKey)
            {
                newKey.Dispose();
                return false;
            }

            List<LainAccount> previous = Accounts;
            Key = newKey;
            Accounts = accounts;
            if (!ReferenceEquals(oldKey, newKey)) oldKey.Dispose();
            if (!ReferenceEquals(previous, accounts))
            {
                HashSet<LainAccount> kept = new HashSet<LainAccount>(accounts);
                foreach (LainAccount a in previous) if (!kept.Contains(a)) a.Forget();
            }

            MainForm main = Application.OpenForms.OfType<MainForm>().FirstOrDefault();
            if (main != null) main.LoadAccounts();
            return true;
        }

        bool _suppressSearch = false;

        enum ListSort { Name, RecentlyUsed, RecentlyModified }

        // view filter: all accounts, favorites only, or a single tag (case-insensitive)
        bool _filterFavorites;
        string _filterTag;
        ListSort _sort = ListSort.Name;
        DataGridViewColumn _nameSortColumn;
        ListSortDirection _nameSortDirection = ListSortDirection.Ascending;
        readonly ContextMenuStrip _viewMenu = new ContextMenuStrip();
        readonly ToolStripMenuItem _tagsMenuItem = new ToolStripMenuItem("Tags");

        readonly DataGridViewTextBoxColumn _colCreated = new DataGridViewTextBoxColumn { HeaderText = "Created", Name = "ColCreated", ReadOnly = true, MinimumWidth = 60 };
        readonly DataGridViewTextBoxColumn _colModified = new DataGridViewTextBoxColumn { HeaderText = "Modified", Name = "ColModified", ReadOnly = true, MinimumWidth = 60 };

        // typing is coalesced into one list rebuild; "last used" stamps are saved in one write
        readonly Timer _searchTimer = new Timer { Interval = 120 };
        readonly Timer _saveTimer = new Timer { Interval = 2000 };
        bool _savePending;
        Font _accountFont;
        Font _accountHeaderFont;
        Font _passwordFont;

        const int WM_SETREDRAW = 0x000B;
        const int WM_SETTINGCHANGE = 0x001A;
        const int WM_THEMECHANGED = 0x031A;

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        internal MainForm(VaultKey key, List<LainAccount> accounts)
        {
            InitializeComponent();
            Disposed += (s, e) => DisposeAccountFonts();

            DoubleBuffered = true;
            KeyPreview = true;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            SetupChrome();
            Options.ApplyTheme(this);
            FixColors();

            TriggerTimer();

            Key = key;
            Accounts = accounts ?? new List<LainAccount>();
            this.Text = "Lain";

            RestoreWindowState();
            SetFontSize();

            clearPic.Visible = false;
            LoadAccounts();
        }

        private void SetupChrome()
        {
            BackColor = Fluent.Background;
            ForeColor = Fluent.Text;

            searchBox.SetLeading(searchIcon);
            searchBox.SetEditor(txtSearch);
            searchBox.AddTrailing(clearPic);
            Fluent.SetPlaceholder(txtSearch, "Search by title, username or tag  (Ctrl+F)");

            actionDivider.BackColor = Fluent.Border;

            AccountView.ApplyColors(null, null);

            AccountView.Controls.Add(lblEmpty);
            lblEmpty.BackColor = Fluent.Surface;
            lblEmpty.ForeColor = Fluent.TextSecondary;
            AccountView.Resize += (s, e) => PositionEmptyState();
            AccountView.SortCompare += AccountView_SortCompare;
            AccountView.Sorted += AccountView_Sorted;
            AccountView.ColumnHeaderMouseClick += (s, e) => { if (_sort != ListSort.Name) { _sort = ListSort.Name; UpdateViewButton(); } };

            AccountView.Columns.AddRange(new DataGridViewColumn[] { _colCreated, _colModified });
            _searchTimer.Tick += (s, e) => { _searchTimer.Stop(); Search(); };
            _saveTimer.Tick += (s, e) => FlushPendingSave();
            components.Add(_searchTimer);
            components.Add(_saveTimer);

            lblStatus.ForeColor = Fluent.TextSecondary;
            lblHint.ForeColor = Fluent.Blend(Fluent.TextSecondary, Fluent.Background, 0.35f);

            // the placeholder gives the item its submenu arrow; real entries are built each time the menu opens
            _tagsMenuItem.DropDownItems.Add(new ToolStripMenuItem("New tag\u2026"));
            helperMenu.Items.Insert(helperMenu.Items.IndexOf(favoriteMenuItem) + 1, _tagsMenuItem);
            helperMenu.Opening += (s, e) => BuildTagsMenu();

            Fluent.StyleMenu(helperMenu);
            Fluent.StyleMenu(moreMenu);
            Fluent.StyleMenu(_viewMenu);
            components.Add(_viewMenu);
            Fluent.SetMenuGlyph(copyToolStripMenuItem, Fluent.Glyphs.Contact, DeviceDpi);
            Fluent.SetMenuGlyph(toolStripMenuItem1, Fluent.Glyphs.Key, DeviceDpi);
            Fluent.SetMenuGlyph(openLinkBtn, Fluent.Glyphs.Link, DeviceDpi);
            Fluent.SetMenuGlyph(favoriteMenuItem, Fluent.Glyphs.Star, DeviceDpi);
            Fluent.SetMenuGlyph(_tagsMenuItem, Fluent.Glyphs.Tag, DeviceDpi);
            Fluent.SetMenuGlyph(newToolStripMenuItem, Fluent.Glyphs.Add, DeviceDpi);
            Fluent.SetMenuGlyph(ediToolStripMenuItem, Fluent.Glyphs.Edit, DeviceDpi);
            Fluent.SetMenuGlyph(removeToolStripMenuItem, Fluent.Glyphs.Delete, DeviceDpi);
            Fluent.SetMenuGlyph(btnAnalyze, Fluent.Glyphs.Shield, DeviceDpi);
            Fluent.SetMenuGlyph(modifyRemoveAll, Fluent.Glyphs.Clear, DeviceDpi);
            Fluent.SetMenuGlyph(btnUpdate, Fluent.Glyphs.Refresh, DeviceDpi);
            Fluent.SetMenuGlyph(btnAbout, Fluent.Glyphs.Info, DeviceDpi);
            removeToolStripMenuItem.ForeColor = Fluent.Danger;
            modifyRemoveAll.ForeColor = Fluent.Danger;

            Fluent.StyleToolTip(toolTip);
            toolTip.SetToolTip(btnAdd, "New account (Ctrl+N)");
            toolTip.SetToolTip(btnLock, "Lock vault (Ctrl+L)");
            toolTip.SetToolTip(btnOptions, "Settings (Ctrl+O)");
            toolTip.SetToolTip(btnMore, "More");
            toolTip.SetToolTip(btnView, "Filter and sort");
            toolTip.SetToolTip(btnFavorite, "Add to or remove from favorites (Ctrl+D)");
            toolTip.SetToolTip(btnCopyUser, "Copy username (Ctrl+Q)");
            toolTip.SetToolTip(btnCopyPassword, "Copy password (Ctrl+W)");
            toolTip.SetToolTip(btnOpenLink, "Open link in browser (Ctrl+E)");
            toolTip.SetToolTip(btnModify, "Edit account (Enter)");
            toolTip.SetToolTip(btnRemove, "Delete account (Del)");
            toolTip.SetToolTip(clearPic, "Clear search (Esc)");

            Fluent.Apply(this);
        }

        private void PositionEmptyState()
        {
            int top = AccountView.ColumnHeadersHeight;
            lblEmpty.SetBounds(0, top, AccountView.Width, Math.Max(0, AccountView.Height - top));
        }

        // Refreshes action availability, empty state and the status bar after the list or selection changes.
        private void UpdateView()
        {
            // single-account actions (copy, open, edit) need exactly one row; delete, favorite and tags work on many
            int selectedCount = AccountView.SelectedRows.Count;
            bool hasSelection = selectedCount == 1;
            bool anySelected = selectedCount > 0;
            string link = hasSelection ? Convert.ToString(AccountView.SelectedRows[0].Cells[3].Value) : string.Empty;
            bool hasLink = hasSelection && !string.IsNullOrWhiteSpace(link);

            btnCopyUser.Enabled = hasSelection;
            btnCopyPassword.Enabled = hasSelection;
            btnModify.Enabled = hasSelection;
            btnRemove.Enabled = anySelected;
            btnOpenLink.Enabled = hasLink;
            btnFavorite.Enabled = anySelected;
            favoriteMenuItem.Enabled = anySelected;
            _tagsMenuItem.Enabled = anySelected;

            // with a mixed selection the action favorites everything, so it only reads "remove" when all are favorites
            bool isFavorite = anySelected && SelectedAccounts().All(a => AccountMeta.Read(Key, a).Favorite);
            btnFavorite.Glyph = isFavorite ? Fluent.Glyphs.StarFill : Fluent.Glyphs.Star;
            btnFavorite.Text = isFavorite ? "Unfavorite" : "Favorite";
            favoriteMenuItem.Text = isFavorite ? "Remove from favorites" : "Add to favorites";

            copyToolStripMenuItem.Enabled = hasSelection;
            toolStripMenuItem1.Enabled = hasSelection;
            ediToolStripMenuItem.Enabled = hasSelection;
            removeToolStripMenuItem.Enabled = anySelected;
            openLinkBtn.Enabled = hasLink;

            btnAnalyze.Enabled = Accounts.Count > 0;
            modifyRemoveAll.Enabled = Accounts.Count > 0;

            if (AccountView.Rows.Count == 0)
            {
                if (Accounts.Count == 0)
                {
                    lblEmpty.Text = "Your vault is empty\n\nSelect \u201CNew account\u201D or press Ctrl+N to add your first account.";
                }
                else if (txtSearch.Text.Trim().Length > 0)
                {
                    lblEmpty.Text = string.Format("No accounts match \u201C{0}\u201D{1}", txtSearch.Text.Trim(), IsFiltered ? "\n\nin " + FilterLabel() : string.Empty);
                }
                else
                {
                    lblEmpty.Text = _filterFavorites
                        ? "No favorites yet\n\nSelect an account and press Ctrl+D to pin it here."
                        : string.Format("No accounts tagged \u201C{0}\u201D", _filterTag);
                }
                PositionEmptyState();
                lblEmpty.Visible = true;
                lblEmpty.BringToFront();
            }
            else
            {
                lblEmpty.Visible = false;
            }

            if (!toastTimer.Enabled) ShowCount();
        }

        private void ShowCount()
        {
            lblStatus.ForeColor = Fluent.TextSecondary;
            int selectedCount = AccountView.SelectedRows.Count;
            if (selectedCount > 1)
            {
                lblStatus.Text = string.Format("{0} of {1} selected", selectedCount, AccountView.Rows.Count);
            }
            else if (string.IsNullOrEmpty(txtSearch.Text) && !IsFiltered)
            {
                lblStatus.Text = string.Format("{0} {1}", Accounts.Count, Accounts.Count == 1 ? "account" : "accounts");
            }
            else
            {
                lblStatus.Text = string.Format("{0} of {1} accounts", AccountView.Rows.Count, Accounts.Count);
            }
        }

        internal void ShowToast(string message)
        {
            toastTimer.Stop();
            lblStatus.ForeColor = Fluent.Success;
            lblStatus.Text = "\u2713  " + message;
            toastTimer.Start();
        }

        private void toastTimer_Tick(object sender, EventArgs e)
        {
            toastTimer.Stop();
            ShowCount();
        }

        private void RestoreWindowState()
        {
            FormWindowState savedState = Enum.IsDefined(typeof(FormWindowState), Options.CurrentOptions.WindowState)
                ? Options.CurrentOptions.WindowState
                : FormWindowState.Normal;
            Size savedSize = Options.CurrentOptions.WindowSize;
            if (savedSize.Width <= 0 || savedSize.Height <= 0) savedSize = new Size(907, 681);

            WindowState = FormWindowState.Normal;
            Size = savedSize;

            if (Options.CurrentOptions.WindowLocation.HasValue)
            {
                Location = Options.CurrentOptions.WindowLocation.Value;
            }
            else
            {
                CenterToScreen();
            }

            Rectangle bounds = new Rectangle(Location, Size);
            if (!HasUsableVisibleBounds(bounds))
            {
                Screen target = Screen.PrimaryScreen ?? Screen.FromPoint(Cursor.Position);
                Rectangle workArea = target.WorkingArea;

                if (MinimumSize.Width > workArea.Width || MinimumSize.Height > workArea.Height)
                {
                    MinimumSize = new Size(Math.Min(MinimumSize.Width, workArea.Width),
                                           Math.Min(MinimumSize.Height, workArea.Height));
                }

                int width = Math.Min(Math.Max(MinimumSize.Width, Width), workArea.Width);
                int height = Math.Min(Math.Max(MinimumSize.Height, Height), workArea.Height);
                Size = new Size(width, height);
                Location = new Point(
                    Math.Max(workArea.Left, Math.Min(Location.X, workArea.Right - Width)),
                    Math.Max(workArea.Top, Math.Min(Location.Y, workArea.Bottom - Height)));
            }

            if (savedState != FormWindowState.Normal) WindowState = savedState;
        }

        private static bool HasUsableVisibleBounds(Rectangle bounds)
        {
            if (bounds.Width <= 0 || bounds.Height <= 0) return false;

            Rectangle titleBar = new Rectangle(
                bounds.Left,
                bounds.Top,
                Math.Min(160, bounds.Width),
                Math.Min(40, bounds.Height));
            int requiredWidth = Math.Min(100, titleBar.Width);
            int requiredHeight = Math.Min(24, titleBar.Height);

            foreach (Screen screen in Screen.AllScreens)
            {
                Rectangle intersection = Rectangle.Intersect(titleBar, screen.WorkingArea);
                if (intersection.Width >= requiredWidth && intersection.Height >= requiredHeight) return true;
            }

            return false;
        }

        private void SaveWindowState()
        {
            Options.CurrentOptions.WindowState = this.WindowState;

            if (this.WindowState == FormWindowState.Normal)
            {
                Options.CurrentOptions.WindowLocation = this.Location;
                Options.CurrentOptions.WindowSize = this.Size;
            }
            else
            {
                Options.CurrentOptions.WindowLocation = this.RestoreBounds.Location;
                Options.CurrentOptions.WindowSize = this.RestoreBounds.Size;
            }
        }

        private string NewVersionMessage(string latest)
        {
            return string.Format("There is a new version available!\n\nLatest version: {0}\nCurrent version: {1}\n\nDo you want to download it now?", latest, Program.GetCurrentVersionToString());
        }

        private void TriggerTimer()
        {
            timerAutoLock.Interval = Options.CurrentOptions.Minutes * ONE_MINUTE_IN_MILLISECONDS;
        }

        private string NewDownloadLink(string latestVersion)
        {
            return string.Format("https://github.com/hellzerg/lain/releases/download/{0}/Lain-{0}.exe", latestVersion);
        }

        private void CheckForUpdate()
        {
            WebClient client = new WebClient
            {
                Encoding = Encoding.UTF8
            };

            string latestVersion = string.Empty;
            try
            {
                latestVersion = client.DownloadString(_latestVersionLink);
            }
            catch (Exception ex)
            {
                FluentMessageBox.Show(this, ex.Message, "Couldn't check for updates", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (!string.IsNullOrEmpty(latestVersion))
            {
                if (float.Parse(latestVersion) > Program.GetCurrentVersion())
                {
                    if (FluentMessageBox.Show(this, NewVersionMessage(latestVersion), "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Update", "Not now") == DialogResult.Yes)
                    {
                        // PATCHING PROCESS
                        try
                        {
                            Assembly currentAssembly = Assembly.GetEntryAssembly();

                            if (currentAssembly == null)
                            {
                                currentAssembly = Assembly.GetCallingAssembly();
                            }

                            string appFolder = Path.GetDirectoryName(currentAssembly.Location);
                            string appName = Path.GetFileNameWithoutExtension(currentAssembly.Location);
                            string appExtension = Path.GetExtension(currentAssembly.Location);

                            string archiveFile = Path.Combine(appFolder, appName + "_old" + appExtension);
                            string appFile = Path.Combine(appFolder, appName + appExtension);
                            string tempFile = Path.Combine(appFolder, appName + "_tmp" + appExtension);

                            // DOWNLOAD NEW VERSION
                            client.DownloadFile(NewDownloadLink(latestVersion), tempFile);

                            // DELETE PREVIOUS BACK-UP
                            if (File.Exists(archiveFile))
                            {
                                File.Delete(archiveFile);
                            }

                            // MAKE BACK-UP
                            File.Move(appFile, archiveFile);

                            // PATCH
                            File.Move(tempFile, appFile);

                            // BYPASS SINGLE-INSTANCE MECHANISM
                            Program.RestartLain();

                            Application.Restart();
                        }
                        catch (Exception ex)
                        {
                            FluentMessageBox.Show(this, ex.Message, "Update failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                else if (float.Parse(latestVersion) == Program.GetCurrentVersion())
                {
                    FluentMessageBox.Show(this, _noNewVersionMessage, "No update available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    FluentMessageBox.Show(this, _betaVersionMessage, "No update available", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Search()
        {
            if (_suppressSearch) return;
            LoadAccounts();
        }

        internal void FixColors()
        {
            lblHint.ForeColor = Fluent.Blend(Fluent.TextSecondary, Fluent.Background, 0.35f);
            AccountView.ApplyColors(null, null);
        }

        internal void ApplyCurrentTheme()
        {
            Options.ApplyTheme(this);
            Fluent.StyleMenu(helperMenu);
            Fluent.StyleMenu(moreMenu);
            Fluent.StyleMenu(_viewMenu);
            FixColors();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if ((m.Msg != WM_SETTINGCHANGE && m.Msg != WM_THEMECHANGED) || Options.CurrentOptions.ThemeMode != ThemeMode.System) return;

            Fluent.RefreshPalette();
            try
            {
                foreach (Form form in Application.OpenForms)
                {
                    Options.ApplyTheme(form);
                }
                Fluent.StyleMenu(helperMenu);
                Fluent.StyleMenu(moreMenu);
                Fluent.StyleMenu(_viewMenu);
                FixColors();
            }
            finally
            {
                Fluent.CompletePaletteChange();
            }
        }

        private void CopyAccount()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1)
            {
                try
                {
                    Clipboard.SetText(rows[0].Cells[1].Value.ToString());
                    ShowToast("Username copied to clipboard");
                    MarkUsed(rows[0]);
                }
                catch { }
            }
        }

        private void CopyPassword()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1)
            {
                try
                {
                    LainAccount account = AccountOf(rows[0]);
                    if (account != null)
                    {
                        SecureClipboard.CopySecret(Key.Decrypt(account.Password()));
                        ShowToast(SecureClipboard.AutoClear
                            ? "Password copied, clears in " + SecureClipboard.Seconds + "s"
                            : "Password copied to clipboard");
                        MarkUsed(rows[0]);
                    }
                }
                catch { }
            }
        }

        private void OpenLink()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1)
            {
                var link = rows[0].Cells[3].Value;
                if (link == null) return;

                if (!string.IsNullOrEmpty(link.ToString().Trim()))
                {
                    if (!link.ToString().StartsWith("http://") && !link.ToString().StartsWith("https://")) link = "https://" + link;
                    System.Diagnostics.Process.Start(link.ToString());
                    MarkUsed(rows[0]);
                }
            }
        }

        private void Modify()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            if (rows.Count == 1 && AccountOf(rows[0]) != null)
            {
                if (Authorize(LoginType.Authorize, true))
                {
                    NewForm f = new NewForm(NewType.Modify, AccountOf(rows[0]));

                    IsDialogOpen = true;
                    DialogResult r = f.ShowDialog();
                    IsDialogOpen = false;

                    if (r == DialogResult.OK)
                    {
                        LoadAccounts(f.SavedAccount);
                        Program.SaveSettings();
                    }
                }
            }
        }

        private bool Authorize(LoginType type, bool simple = false, int count = 1)
        {
            bool result = false;

            if (Options.CurrentOptions.Authorize)
            {
                LoginForm f = new LoginForm(type);

                IsDialogOpen = true;
                DialogResult dr = f.ShowDialog();
                IsDialogOpen = false;

                if (dr == DialogResult.Yes)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                if (simple)
                {
                    return true;
                }

                string title = string.Empty;
                switch (type)
                {
                    case LoginType.Remove:
                        title = count > 1 ? string.Format("Delete {0} accounts?", count) : "Delete account?";
                        break;
                    case LoginType.RemoveAll:
                        title = "Delete all accounts?";
                        break;
                }

                IsDialogOpen = true;
                string message = type == LoginType.RemoveAll
                    ? "Every account in your vault will be permanently deleted. This can't be undone."
                    : count > 1
                        ? string.Format("The {0} selected accounts will be permanently deleted. This can't be undone.", count)
                        : "This account will be permanently deleted. This can't be undone.";
                DialogResult dr = FluentMessageBox.Show(this, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, "Delete", "Cancel", true);
                IsDialogOpen = false;

                if (dr == DialogResult.Yes)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        private void Remove()
        {
            var rows = AccountView.SelectedRows;
            if (rows.Count <= 0) return;

            int count = rows.Count;
            if (Authorize(LoginType.Remove, false, count))
            {
                foreach (DataGridViewRow row in rows)
                {
                    LainAccount account = AccountOf(row);
                    if (account != null) Accounts.Remove(account);
                }

                LoadAccounts();
                Program.SaveSettings();
                if (count > 1) ShowToast(string.Format("{0} accounts deleted", count));
            }
        }

        private void CreateNewAccount()
        {
            NewForm f = new NewForm(NewType.New);
            IsDialogOpen = true;
            DialogResult r = f.ShowDialog(this);
            IsDialogOpen = false;

            if (r == DialogResult.OK)
            {
                LoadAccounts(f.SavedAccount);
                Program.SaveSettings();
            }
        }

        private void RemoveAll()
        {
            if (AccountView.Rows.Count > 0)
            {
                if (Authorize(LoginType.RemoveAll))
                {
                    Accounts.Clear();
                    LoadAccounts();
                    Program.SaveSettings();
                }
            }
        }

        private void Lock()
        {
            Program.SetMainForm(new LoginForm(LoginType.Login, this.WindowState));
            IsDialogOpen = true;
            timerAutoLock.Stop();
            this.Close();
            // the closed form already saved; now drop the key from memory
            Program.CloseVault();
            Program.ShowMainForm();
        }

        // Rebuilds the list while keeping the search filter, sort, selection and scroll position.
        // select: an account to select afterwards (e.g. one that was just saved).
        internal void LoadAccounts(LainAccount select = null)
        {
            if (Key == null) return;
            _searchTimer.Stop();

            // Keep decrypted account metadata cached only for this unlocked session; credential fields are not cached.
            bool showPasswords = !Options.CurrentOptions.HidePasswords;
            using (VaultKey.Cipher c = Key.Begin())
            {
                foreach (LainAccount x in Accounts) AccountMeta.Read(c, x);
            }

            string term = txtSearch.Text.Trim();

            if (select != null && term.Length > 0 && !Matches(select, term))
            {
                // a just-saved account that doesn't match the filter would be invisible
                _suppressSearch = true;
                txtSearch.Clear();
                _suppressSearch = false;
                term = string.Empty;
            }

            if (select != null && IsFiltered && !PassesFilter(AccountMeta.Read(Key, select))) ClearFilter();

            clearPic.Visible = term.Length > 0;

            HashSet<LainAccount> selected = new HashSet<LainAccount>();
            if (select != null)
            {
                selected.Add(select);
            }
            else
            {
                foreach (DataGridViewRow row in AccountView.SelectedRows)
                {
                    LainAccount a = AccountOf(row);
                    if (a != null) selected.Add(a);
                }
            }

            int previousIndex = AccountView.CurrentRow != null ? AccountView.CurrentRow.Index : -1;
            int previousTop = AccountView.Rows.Count > 0 ? AccountView.FirstDisplayedScrollingRowIndex : -1;
            bool hadFocus = AccountView.Focused;

            bool redrawSuspended = AccountView.IsHandleCreated;
            if (redrawSuspended) SendMessage(AccountView.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);

            try
            {
                AccountView.Rows.Clear();
                AccountView.Columns[2].Visible = showPasswords;
                _colCreated.Visible = Options.CurrentOptions.ShowCreatedColumn;
                _colModified.Visible = Options.CurrentOptions.ShowModifiedColumn;

                bool anyFavorite = false, anyTags = false;
                List<DataGridViewRow> rows = new List<DataGridViewRow>(Accounts.Count);
                using (VaultKey.Cipher c = Key.Begin())
                {
                    foreach (LainAccount x in Accounts)
                    {
                        AccountMeta meta = x.CachedMeta;
                        anyFavorite |= meta.Favorite;
                        anyTags |= meta.Tags.Count > 0;

                        if (!PassesFilter(meta)) continue;

                        string name = c.Decrypt(x.Name());
                        string email = c.Decrypt(x.Email());
                        if (term.Length > 0 && name.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0
                            && email.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0
                            && !meta.Tags.Any(t => t.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)) continue;

                        DataGridViewRow row = new DataGridViewRow { Height = AccountView.RowTemplate.Height };
                        row.CreateCells(AccountView,
                            name,
                            email,
                            showPasswords ? c.Decrypt(x.Password()) : string.Empty,
                            c.Decrypt(x.Link()),
                            TagPills.Join(meta.Tags),
                            FormatDate(meta.Created),
                            FormatDate(meta.Modified ?? meta.Created));
                        row.Tag = x;
                        rows.Add(row);
                    }
                }
                AccountView.Rows.AddRange(rows.ToArray());

                Column5.Visible = anyTags;
                AccountView.SetStarSlot(0, anyFavorite, AccountView.DefaultCellStyle.Font);
                SetAutoSizeColumns();
                SortRows();

                RestoreSelection(selected, previousIndex);

                if (select == null && previousTop > -1 && AccountView.Rows.Count > 0)
                {
                    try
                    {
                        AccountView.FirstDisplayedScrollingRowIndex = Math.Min(previousTop, AccountView.Rows.Count - 1);
                    }
                    catch (InvalidOperationException) { }
                }
            }
            finally
            {
                if (redrawSuspended)
                {
                    SendMessage(AccountView.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                    AccountView.Invalidate(true);
                }
            }

            if (hadFocus || select != null) AccountView.Focus();

            this.Text = string.Format("Lain {0}", Program.GetCurrentVersionToString());
            UpdateView();
        }

        internal static string FormatDate(DateTime? utc)
        {
            return utc.HasValue ? utc.Value.ToLocalTime().ToString("d") : "\u2014";
        }

        static LainAccount AccountOf(DataGridViewRow row)
        {
            return row == null ? null : row.Tag as LainAccount;
        }

        static AccountMeta MetaOf(DataGridViewRow row)
        {
            LainAccount a = AccountOf(row);
            return a == null || Key == null ? a?.CachedMeta : AccountMeta.Read(Key, a);
        }

        private static bool Matches(LainAccount account, string term)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (account.PlainName(Key).IndexOf(term, comparison) >= 0) return true;
            if (account.PlainEmail(Key).IndexOf(term, comparison) >= 0) return true;
            return AccountMeta.Read(Key, account).Tags.Any(t => t.IndexOf(term, comparison) >= 0);
        }

        // distinct tags across the vault, for autocomplete and the filter menu
        internal static List<string> AllTags()
        {
            if (Key == null) return new List<string>();
            return Accounts
                .SelectMany(a => AccountMeta.Read(Key, a).Tags)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(t => t, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
        }

        bool IsFiltered { get { return _filterFavorites || _filterTag != null; } }

        bool PassesFilter(AccountMeta meta)
        {
            if (_filterFavorites && !meta.Favorite) return false;
            if (_filterTag != null && !meta.Tags.Any(t => string.Equals(t, _filterTag, StringComparison.OrdinalIgnoreCase))) return false;
            return true;
        }

        string FilterLabel()
        {
            if (_filterFavorites) return "Favorites";
            if (_filterTag != null) return _filterTag;
            return "All accounts";
        }

        void ClearFilter()
        {
            _filterFavorites = false;
            _filterTag = null;
            UpdateViewButton();
        }

        // Favorites stay pinned on top in every sort; the rest follow the chosen order.
        private void SortRows()
        {
            if (AccountView.Rows.Count < 2) return;

            if (_sort == ListSort.Name)
            {
                DataGridViewColumn column = _nameSortColumn != null && _nameSortColumn.Visible ? _nameSortColumn : AccountView.Columns[0];
                AccountView.Sort(column, _nameSortDirection);
            }
            else
            {
                AccountView.Sort(new RecentComparer(_sort == ListSort.RecentlyUsed));
            }
        }

        private void AccountView_Sorted(object sender, EventArgs e)
        {
            if (AccountView.SortedColumn == null) return;
            _nameSortColumn = AccountView.SortedColumn;
            _nameSortDirection = AccountView.SortOrder == SortOrder.Descending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        private void AccountView_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            int pin = FavoriteFirst(AccountView.Rows[e.RowIndex1], AccountView.Rows[e.RowIndex2]);
            if (pin != 0)
            {
                // the grid negates the result for descending sorts; favorites must stay on top regardless
                e.SortResult = AccountView.SortOrder == SortOrder.Descending ? -pin : pin;
            }
            else if (e.Column == _colCreated || e.Column == _colModified)
            {
                AccountMeta m1 = MetaOf(AccountView.Rows[e.RowIndex1]), m2 = MetaOf(AccountView.Rows[e.RowIndex2]);
                DateTime d1 = DateOf(m1, e.Column == _colModified), d2 = DateOf(m2, e.Column == _colModified);
                e.SortResult = d1.CompareTo(d2);
                if (e.SortResult == 0)
                {
                    e.SortResult = string.Compare(Convert.ToString(AccountView.Rows[e.RowIndex1].Cells[0].Value),
                        Convert.ToString(AccountView.Rows[e.RowIndex2].Cells[0].Value), StringComparison.CurrentCultureIgnoreCase);
                }
            }
            else
            {
                e.SortResult = string.Compare(Convert.ToString(e.CellValue1), Convert.ToString(e.CellValue2), StringComparison.CurrentCultureIgnoreCase);
                if (e.SortResult == 0)
                {
                    e.SortResult = string.Compare(Convert.ToString(AccountView.Rows[e.RowIndex1].Cells[0].Value),
                        Convert.ToString(AccountView.Rows[e.RowIndex2].Cells[0].Value), StringComparison.CurrentCultureIgnoreCase);
                }
            }
            e.Handled = true;
        }

        static DateTime DateOf(AccountMeta m, bool modified)
        {
            if (m == null) return DateTime.MinValue;
            DateTime? d = modified ? (m.Modified ?? m.Created) : m.Created;
            return d ?? DateTime.MinValue;
        }

        static int FavoriteFirst(DataGridViewRow a, DataGridViewRow b)
        {
            bool fa = MetaOf(a)?.Favorite == true;
            bool fb = MetaOf(b)?.Favorite == true;
            return fa == fb ? 0 : (fa ? -1 : 1);
        }

        sealed class RecentComparer : System.Collections.IComparer
        {
            readonly bool _used;

            internal RecentComparer(bool used) { _used = used; }

            public int Compare(object x, object y)
            {
                DataGridViewRow a = (DataGridViewRow)x, b = (DataGridViewRow)y;
                int pin = FavoriteFirst(a, b);
                if (pin != 0) return pin;

                DateTime da = When(MetaOf(a)), db = When(MetaOf(b));
                int byDate = db.CompareTo(da);
                if (byDate != 0) return byDate;

                return string.Compare(Convert.ToString(a.Cells[0].Value), Convert.ToString(b.Cells[0].Value), StringComparison.CurrentCultureIgnoreCase);
            }

            DateTime When(AccountMeta m)
            {
                if (m == null) return DateTime.MinValue;
                DateTime? d = _used ? m.LastUsed : (m.Modified ?? m.Created);
                return d ?? DateTime.MinValue;
            }
        }

        private void UpdateViewButton()
        {
            bool custom = IsFiltered || _sort != ListSort.Name;
            string text = string.Empty;
            if (IsFiltered) text = FilterLabel();
            else if (_sort == ListSort.RecentlyUsed) text = "Recently used";
            else if (_sort == ListSort.RecentlyModified) text = "Recently modified";

            btnView.Glyph = !IsFiltered && custom ? Fluent.Glyphs.Sort : Fluent.Glyphs.Filter;
            btnView.Variant = custom ? ButtonVariant.Secondary : ButtonVariant.Subtle;
            btnView.Text = text;
            if (text.Length == 0)
            {
                btnView.AutoSize = false;
                btnView.Size = new Size(btnLock.Width, btnLock.Height);
            }
            else
            {
                btnView.AutoSize = true;
                btnView.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                btnView.MinimumSize = new Size(0, btnLock.Height);
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            _viewMenu.Items.Clear();

            _viewMenu.Items.Add(ViewItem("All accounts", Fluent.Glyphs.Filter, !IsFiltered, () => ClearFilter()));
            _viewMenu.Items.Add(ViewItem("Favorites", Fluent.Glyphs.StarFill, _filterFavorites, () => { _filterFavorites = true; _filterTag = null; }));

            List<string> tags = AllTags();
            if (tags.Count > 0)
            {
                ToolStripMenuItem tagRoot = ViewItem("Tags", Fluent.Glyphs.Tag, _filterTag != null, null);
                foreach (string tag in tags)
                {
                    string t = tag;
                    tagRoot.DropDownItems.Add(ViewItem(t, null, string.Equals(_filterTag, t, StringComparison.OrdinalIgnoreCase), () => { _filterTag = t; _filterFavorites = false; }));
                }
                Fluent.StyleMenu(tagRoot.DropDown);
                _viewMenu.Items.Add(tagRoot);
            }

            _viewMenu.Items.Add(new ToolStripSeparator());
            _viewMenu.Items.Add(ViewItem("Sort by name", Fluent.Glyphs.Sort, _sort == ListSort.Name, () => { _sort = ListSort.Name; _nameSortColumn = AccountView.Columns[0]; _nameSortDirection = ListSortDirection.Ascending; }));
            _viewMenu.Items.Add(ViewItem("Recently used", null, _sort == ListSort.RecentlyUsed, () => _sort = ListSort.RecentlyUsed));
            _viewMenu.Items.Add(ViewItem("Recently modified", null, _sort == ListSort.RecentlyModified, () => _sort = ListSort.RecentlyModified));

            Fluent.StyleMenu(_viewMenu);
            _viewMenu.Show(btnView, new Point(0, btnView.Height + 4));
        }

        ToolStripMenuItem ViewItem(string text, string glyph, bool active, Action apply)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            Fluent.SetMenuGlyph(item, active ? Fluent.Glyphs.Checkmark : (glyph ?? string.Empty), DeviceDpi);
            if (active) item.ForeColor = Fluent.Accent;
            if (apply != null)
            {
                item.Click += (s, e) =>
                {
                    apply();
                    UpdateViewButton();
                    LoadAccounts();
                };
            }
            return item;
        }

        // Selected accounts in on-screen order.
        private List<LainAccount> SelectedAccounts()
        {
            List<LainAccount> result = new List<LainAccount>();
            foreach (DataGridViewRow row in AccountView.SelectedRows.Cast<DataGridViewRow>().OrderBy(r => r.Index))
            {
                LainAccount account = AccountOf(row);
                if (account != null) result.Add(account);
            }
            return result;
        }

        // Applies one metadata change to every selected account with a single vault write and a list refresh
        // that keeps the selection, focus and scroll position.
        private void UpdateSelectedMeta(Action<AccountMeta> change)
        {
            List<LainAccount> accounts = SelectedAccounts();
            if (accounts.Count == 0) return;

            foreach (LainAccount account in accounts)
            {
                AccountMeta meta = AccountMeta.Read(Key, account).Clone();
                change(meta);
                AccountMeta.Write(Key, account, meta);
            }

            Program.SaveSettings();
            LoadAccounts();
        }

        private void ToggleFavorite()
        {
            List<LainAccount> accounts = SelectedAccounts();
            if (accounts.Count == 0) return;

            // a mixed selection becomes all favorites; only an all-favorite selection is cleared
            bool favorite = !accounts.All(a => AccountMeta.Read(Key, a).Favorite);
            UpdateSelectedMeta(meta => meta.Favorite = favorite);

            string noun = accounts.Count == 1 ? string.Empty : string.Format("{0} accounts ", accounts.Count);
            string text = favorite ? noun + "added to favorites" : noun + "removed from favorites";
            ShowToast(char.ToUpperInvariant(text[0]) + text.Substring(1));
        }

        private static bool HasTag(AccountMeta meta, string tag)
        {
            return meta.Tags.Any(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
        }

        // Rebuilds the context menu's Tags submenu for the current selection:
        // a check means every selected account has the tag, a dash means only some do.
        private void BuildTagsMenu()
        {
            ToolStripDropDown drop = _tagsMenuItem.DropDown;
            drop.SuspendLayout();
            try
            {
                foreach (ToolStripItem old in drop.Items.Cast<ToolStripItem>().ToList()) old.Dispose();
                drop.Items.Clear();

                List<AccountMeta> metas = SelectedAccounts().Select(a => AccountMeta.Read(Key, a)).ToList();
                List<ToolStripItem> items = new List<ToolStripItem>();
                List<KeyValuePair<ToolStripMenuItem, bool>> marks = new List<KeyValuePair<ToolStripMenuItem, bool>>();

                foreach (string tag in AllTags())
                {
                    string t = tag;
                    int having = metas.Count(m => HasTag(m, t));
                    bool all = metas.Count > 0 && having == metas.Count;

                    ToolStripMenuItem item = new ToolStripMenuItem(t);
                    item.Click += (s, e) => ToggleTag(t);
                    if (having > 0) marks.Add(new KeyValuePair<ToolStripMenuItem, bool>(item, all));
                    else Fluent.SetMenuGlyph(item, string.Empty, DeviceDpi);
                    items.Add(item);
                }

                if (items.Count > 0) items.Add(new ToolStripSeparator());

                ToolStripMenuItem newTag = new ToolStripMenuItem("New tag\u2026");
                Fluent.SetMenuGlyph(newTag, Fluent.Glyphs.Add, DeviceDpi);
                newTag.Click += (s, e) => AddNewTag();
                items.Add(newTag);

                ToolStripMenuItem clear = new ToolStripMenuItem("Clear tags");
                Fluent.SetMenuGlyph(clear, Fluent.Glyphs.Clear, DeviceDpi);
                clear.Enabled = metas.Any(m => m.Tags.Count > 0);
                clear.Click += (s, e) =>
                {
                    UpdateSelectedMeta(meta => meta.Tags.Clear());
                    ShowToast("Tags cleared");
                };
                items.Add(clear);

                drop.Items.AddRange(items.ToArray());
                Fluent.StyleMenuItems(drop);

                foreach (var mark in marks)
                {
                    Fluent.SetMenuGlyph(mark.Key, mark.Value ? Fluent.Glyphs.Checkmark : Fluent.Glyphs.Partial, DeviceDpi);
                    if (mark.Value) mark.Key.ForeColor = Fluent.Accent;
                }
            }
            finally
            {
                drop.ResumeLayout();
            }
        }

        // Adds the tag to every selected account, or removes it when they all already have it.
        private void ToggleTag(string tag)
        {
            List<LainAccount> accounts = SelectedAccounts();
            if (accounts.Count == 0) return;

            bool remove = accounts.All(a => HasTag(AccountMeta.Read(Key, a), tag));
            if (!remove && accounts.Any(a => !HasTag(AccountMeta.Read(Key, a), tag) && AccountMeta.Read(Key, a).Tags.Count >= AccountMeta.MaxTags))
            {
                ShowToast(string.Format("An account can have at most {0} tags", AccountMeta.MaxTags));
                return;
            }

            UpdateSelectedMeta(meta =>
            {
                if (remove) meta.Tags.RemoveAll(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
                else if (!HasTag(meta, tag)) meta.Tags.Add(tag);
            });
            ShowToast(string.Format(remove ? "Removed tag \u201C{0}\u201D" : "Tagged \u201C{0}\u201D", tag));
        }

        private void AddNewTag()
        {
            int count = AccountView.SelectedRows.Count;
            if (count == 0) return;

            IsDialogOpen = true;
            string text = FluentPrompt.AskText(this, "New tag",
                count == 1 ? "Add a tag to the selected account." : string.Format("Add a tag to the {0} selected accounts.", count),
                string.Empty, "e.g. work", AccountMeta.MaxTagLength, "Add");
            IsDialogOpen = false;

            List<string> parsed = AccountMeta.ParseTags(text);
            if (parsed.Count == 0) return;

            // reuse an existing tag's casing so "Work" and "work" don't end up as two filters
            string tag = AllTags().FirstOrDefault(t => string.Equals(t, parsed[0], StringComparison.OrdinalIgnoreCase)) ?? parsed[0];
            if (SelectedAccounts().All(a => HasTag(AccountMeta.Read(Key, a), tag)))
            {
                ShowToast(string.Format("Already tagged \u201C{0}\u201D", tag));
                return;
            }
            ToggleTag(tag);
        }

        // Records when an account was last used; the list is not re-sorted so rows never jump under the cursor.
        private void MarkUsed(DataGridViewRow row)
        {
            try
            {
                LainAccount account = AccountOf(row);
                if (account == null) return;

                AccountMeta meta = AccountMeta.Read(Key, account).Clone();
                meta.LastUsed = DateTime.UtcNow;
                AccountMeta.Write(Key, account, meta);

                // a burst of copies costs one vault write
                _savePending = true;
                _saveTimer.Stop();
                _saveTimer.Start();
            }
            catch { }
        }

        private void FlushPendingSave()
        {
            _saveTimer.Stop();
            if (!_savePending) return;
            _savePending = false;
            Program.SaveSettings();
        }

        private void favorite_Click(object sender, EventArgs e)
        {
            ToggleFavorite();
        }

        private void RestoreSelection(HashSet<LainAccount> accounts, int fallbackIndex)
        {
            AccountView.ClearSelection();
            if (AccountView.Rows.Count == 0) return;

            DataGridViewRow current = null;
            List<DataGridViewRow> matches = new List<DataGridViewRow>();

            foreach (DataGridViewRow row in AccountView.Rows)
            {
                LainAccount a = AccountOf(row);
                if (a != null && accounts.Contains(a)) matches.Add(row);
            }

            if (matches.Count > 0)
            {
                current = matches[0];
            }
            else
            {
                current = AccountView.Rows[Math.Max(0, Math.Min(fallbackIndex, AccountView.Rows.Count - 1))];
                matches.Add(current);
            }

            AccountView.CurrentCell = current.Cells[0];
            AccountView.ClearSelection();
            foreach (DataGridViewRow row in matches) row.Selected = true;
        }

        internal void ApplyDateColumns()
        {
            _colCreated.Visible = Options.CurrentOptions.ShowCreatedColumn;
            _colModified.Visible = Options.CurrentOptions.ShowModifiedColumn;
        }

        internal void SetAutoSizeColumns()
        {
            if (Options.CurrentOptions.AutoSizeColumns)
            {
                AccountView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                Column5.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                _colCreated.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                _colModified.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            else
            {
                AccountView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                AccountView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }
        }

        // weak, reused, old and (on request) breached passwords
        private void AnalyzeAccounts()
        {
            if (Accounts.Count == 0) return;

            bool refreshSearchAfterDialog = _searchTimer.Enabled;
            _searchTimer.Stop();
            LainAccount edit = null;

            try
            {
                using (SecurityReportForm f = new SecurityReportForm())
                {
                    IsDialogOpen = true;
                    f.ShowDialog(this);
                    edit = f.EditAccount;
                }
            }
            finally
            {
                IsDialogOpen = false;
                if (refreshSearchAfterDialog && !IsDisposed && !Disposing) Search();
            }

            if (edit != null)
            {
                LoadAccounts(edit);
                Modify();
            }
        }

        internal void SetFontSize()
        {
            float size = 11f;
            if (Options.CurrentOptions.FontSize == 0) size = 10f;
            if (Options.CurrentOptions.FontSize == 2) size = 12.5f;

            Font nextAccountFont = Fluent.Ui(size);
            Font nextHeaderFont = Fluent.UiSemibold(size - 1.5f);
            Font nextPasswordFont = Fluent.Mono(size - 0.5f);
            Font previousAccountFont = _accountFont;
            Font previousHeaderFont = _accountHeaderFont;
            Font previousPasswordFont = _passwordFont;

            _accountFont = nextAccountFont;
            _accountHeaderFont = nextHeaderFont;
            _passwordFont = nextPasswordFont;

            leftPanel.Font = nextAccountFont;
            lblEmpty.Font = nextAccountFont;
            AccountView.ApplyColors(nextAccountFont, nextHeaderFont);
            Column3.DefaultCellStyle = new DataGridViewCellStyle(Column3.DefaultCellStyle) { Font = nextPasswordFont };

            int rowHeight = (int)(nextAccountFont.Height * (Options.CurrentOptions.CompactRows ? 1.5f : 2f));
            AccountView.ColumnHeadersHeight = (int)(nextAccountFont.Height * 2.1f);
            AccountView.RowTemplate.Height = rowHeight;

            int firstRow = AccountView.FirstDisplayedScrollingRowIndex;
            AccountView.SuspendLayout();
            try
            {
                foreach (DataGridViewRow row in AccountView.Rows)
                {
                    if (row.Height != rowHeight) row.Height = rowHeight;
                }
            }
            finally
            {
                AccountView.ResumeLayout();
            }

            if (firstRow >= 0 && firstRow < AccountView.RowCount) AccountView.FirstDisplayedScrollingRowIndex = firstRow;
            DataGridViewRow current = AccountView.CurrentRow;
            if (current != null && current.Visible && !current.Displayed) AccountView.FirstDisplayedScrollingRowIndex = current.Index;
            PositionEmptyState();

            DisposeFontIfReplaced(previousAccountFont, nextAccountFont, nextHeaderFont, nextPasswordFont);
            DisposeFontIfReplaced(previousHeaderFont, nextAccountFont, nextHeaderFont, nextPasswordFont);
            DisposeFontIfReplaced(previousPasswordFont, nextAccountFont, nextHeaderFont, nextPasswordFont);
        }

        private static void DisposeFontIfReplaced(Font previous, params Font[] current)
        {
            if (previous == null) return;
            foreach (Font font in current)
            {
                if (ReferenceEquals(previous, font)) return;
            }
            previous.Dispose();
        }

        private void DisposeAccountFonts()
        {
            DisposeFontIfReplaced(_accountFont);
            DisposeFontIfReplaced(_accountHeaderFont, _accountFont);
            DisposeFontIfReplaced(_passwordFont, _accountFont, _accountHeaderFont);
            _accountFont = _accountHeaderFont = _passwordFont = null;
        }

        private void AccountView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void btnMore_Click(object sender, EventArgs e)
        {
            moreMenu.Show(btnMore, new Point(btnMore.Width, btnMore.Height + 4), ToolStripDropDownDirection.BelowLeft);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            CreateNewAccount();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyAccount();
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            RemoveAll();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            Lock();
        }

        private void btnOptions_Click(object sender, EventArgs e)
        {
            OptionsForm f = new OptionsForm(this);
            IsDialogOpen = true;
            f.ShowDialog(this);
            IsDialogOpen = false;
            TriggerTimer();
            AccountView.Focus();
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            IsDialogOpen = true;
            f.ShowDialog();
            IsDialogOpen = false;
        }

        private void clearPic_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewAccount();
        }

        private void ediToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Modify();
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Remove();
        }

        private void timerAutoLock_Tick(object sender, EventArgs e)
        {
            Lock();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            timerAutoLock.Stop();
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            if (Options.CurrentOptions.AutoLock && !IsDialogOpen)
            {
                timerAutoLock.Start();
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            if (_suppressSearch) return;

            // typing is debounced; clearing the box responds at once
            _searchTimer.Stop();
            if (txtSearch.TextLength == 0) Search();
            else _searchTimer.Start();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            CheckForUpdate();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            FlushPendingSave();
            SaveWindowState();
            Options.SaveSettings();
            ClearSensitiveUi();

            // only removes a password we copied; anything the user copied since is left alone
            SecureClipboard.ClearIfOurs();
        }

        private void ClearSensitiveUi()
        {
            _searchTimer.Stop();
            AccountView.CancelEdit();

            foreach (DataGridViewRow row in AccountView.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value is string) cell.Value = string.Empty;
                }
            }

            AccountView.ClearSelection();
            AccountView.Rows.Clear();

            // Filters and dynamically generated menu labels can also contain decrypted tags.
            _filterTag = null;
            _filterFavorites = false;
            _viewMenu.Close();
            _viewMenu.Items.Clear();
            _tagsMenuItem.DropDownItems.Clear();
            UpdateViewButton();

            _suppressSearch = true;
            try { txtSearch.Clear(); }
            finally { _suppressSearch = false; }

            clearPic.Visible = false;
            lblEmpty.Text = string.Empty;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyPassword();
        }

        private void AccountView_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0 || e.Button != MouseButtons.Left) return;
            Modify();
        }

        private void openLinkBtn_Click(object sender, EventArgs e)
        {
            OpenLink();
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            AnalyzeAccounts();
        }

        private void AccountView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex != -1)
            {
                DataGridViewRow row = AccountView.Rows[e.RowIndex];

                // right-clicking inside a multi-selection acts on all of it; elsewhere it selects just that row
                if (!row.Selected) AccountView.CurrentCell = row.Cells[e.ColumnIndex == -1 ? 1 : e.ColumnIndex];
                row.Selected = true;

                AccountView.Focus();
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                if (string.IsNullOrEmpty(txtSearch.Text)) AccountView.Focus();
                else txtSearch.Clear();
                e.SuppressKeyPress = true;
            }

            if ((e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter) && AccountView.Rows.Count > 0)
            {
                AccountView.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Q)
            {
                copyToolStripMenuItem.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W)
            {
                toolStripMenuItem1.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.E)
            {
                openLinkBtn.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
            {
                txtSearch.Focus();
                if (!string.IsNullOrEmpty(txtSearch.Text)) txtSearch.SelectAll();
            }

            if (e.KeyCode == Keys.Delete && !txtSearch.Focused)
            {
                removeToolStripMenuItem.PerformClick();
            }

            if (e.KeyCode == Keys.Enter && e.Modifiers == Keys.None && !txtSearch.Focused)
            {
                ediToolStripMenuItem.PerformClick();
                e.SuppressKeyPress = true;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                CreateNewAccount();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.L)
            {
                Lock();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.O)
            {
                btnOptions.PerformClick();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.D)
            {
                ToggleFavorite();
                e.SuppressKeyPress = true;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.R)
            {
                if (btnAnalyze.Enabled) AnalyzeAccounts();
                e.SuppressKeyPress = true;
            }
        }
    }
}
