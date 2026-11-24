using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lain
{
    // Combined vault health check: weak, reused and old passwords, plus an optional online breach check.
    // Passwords are never displayed or copied; only account titles and descriptions of the problem.
    public sealed partial class SecurityReportForm : Form
    {
        enum IssueKind { Breached, Reused, Weak, Old }

        sealed class Issue
        {
            internal IssueKind Kind;
            internal string Account;
            internal LainAccount Ref;
            internal string Details;
        }

        const int OldAfterDays = 365;
        const string BreachPrivacyNotice = "Breach check is optional. Only the first 5 characters of each password's SHA-1 hash are sent to " +
                                            "haveibeenpwned.com; your passwords never leave this computer.";

        sealed class AnalysisResult
        {
            internal List<Issue> Issues;
            internal int CheckedCount;
        }

        sealed class BreachCandidates
        {
            internal Dictionary<string, List<LainAccount>> AccountsByHash;
            internal List<string> Hashes;
        }

        readonly List<Issue> _issues = new List<Issue>();
        CancellationTokenSource _analysisCancel;
        CancellationTokenSource _breachCancel;
        Font _issueFont;
        int _checkedCount;
        bool _breachChecked;
        bool _closeRequested;
        bool _allowClose;
        LainAccount _pendingEditAccount;

        // The account the user chose to edit, or null.
        internal LainAccount EditAccount { get; private set; }

        public SecurityReportForm()
        {
            InitializeComponent();
            Options.ApplyTheme(this);

            grid.ApplyColors(null, null);
            _issueFont = Fluent.UiSemibold(grid.DefaultCellStyle.Font.SizeInPoints);
            Disposed += (s, e) => { if (_issueFont != null) _issueFont.Dispose(); };
            gridCard.BackColor = Fluent.Surface;
            grid.Controls.Add(lblEmpty);
            lblEmpty.ForeColor = Fluent.TextSecondary;
            lblEmpty.BackColor = Fluent.Surface;
            lblSubtitle.ForeColor = Fluent.TextSecondary;
            lblStatus.ForeColor = Fluent.TextSecondary;
            shieldIcon.GlyphColor = Fluent.Accent;

            grid.SelectionChanged += (s, e) => btnEdit.Enabled = grid.SelectedRows.Count > 0;
            grid.CellMouseDoubleClick += (s, e) => { if (e.RowIndex >= 0 && e.Button == MouseButtons.Left) EditSelected(); };
            grid.KeyDown += grid_KeyDown;
            grid.Resize += (s, e) => CenterEmptyLabel();
            btnEdit.Click += (s, e) => EditSelected();
            btnCopy.Click += btnCopy_Click;
            btnBreach.Click += btnBreach_Click;
            FormClosing += SecurityReportForm_FormClosing;
            FormClosed += (s, e) => ClearSensitiveReportData();
            Shown += SecurityReportForm_Shown;

            Fluent.StyleDialog(this);
            grid.Enabled = false;
            btnEdit.Enabled = false;
            btnCopy.Enabled = false;
            btnBreach.Enabled = false;
            lblEmpty.Text = "Analyzing passwords\u2026";
            lblEmpty.ForeColor = Fluent.TextSecondary;
            lblEmpty.Visible = true;
            lblStatus.Text = "Checking password strength, reuse and age\u2026";
        }

        private async void SecurityReportForm_Shown(object sender, EventArgs e)
        {
            CancellationTokenSource cancellation = new CancellationTokenSource();
            _analysisCancel = cancellation;

            try
            {
                VaultKey key = MainForm.Key;
                LainAccount[] accounts = MainForm.Accounts.ToArray();
                AnalysisResult result = await Task.Run(() => Analyze(key, accounts, cancellation.Token), cancellation.Token);
                if (IsDisposed || _closeRequested) return;

                _issues.AddRange(result.Issues);
                _checkedCount = result.CheckedCount;
                lblStatus.ForeColor = Fluent.TextSecondary;
                lblStatus.Text = BreachPrivacyNotice;
                Render(null);
                grid.Enabled = true;
                btnCopy.Enabled = true;
                btnBreach.Enabled = true;
            }
            catch (OperationCanceledException)
            {
                if (!IsDisposed && !_closeRequested)
                {
                    lblStatus.Text = "Security analysis cancelled.";
                    lblStatus.ForeColor = Fluent.TextSecondary;
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed && !_closeRequested)
                {
                    lblStatus.Text = "The security analysis failed: " + ex.Message;
                    lblStatus.ForeColor = Fluent.Danger;
                    lblEmpty.Text = "The security report could not be generated.";
                    lblEmpty.ForeColor = Fluent.Danger;
                    lblEmpty.Visible = true;
                    btnEdit.Enabled = false;
                    btnCopy.Enabled = false;
                    btnBreach.Enabled = false;
                }
            }
            finally
            {
                if (ReferenceEquals(_analysisCancel, cancellation)) _analysisCancel = null;
                cancellation.Dispose();
                FinishCloseIfRequested();
            }
        }

        private static AnalysisResult Analyze(VaultKey key, LainAccount[] accounts, CancellationToken cancellationToken)
        {
            DateTime now = DateTime.UtcNow;
            Dictionary<string, List<LainAccount>> byPasswordFingerprint = new Dictionary<string, List<LainAccount>>(StringComparer.Ordinal);
            AnalysisResult result = new AnalysisResult { Issues = new List<Issue>(), CheckedCount = 0 };
            byte[] comparisonKey = new byte[32];

            try
            {
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(comparisonKey);

                using (HMACSHA256 fingerprint = new HMACSHA256(comparisonKey))
                using (VaultKey.Cipher c = key.Begin())
                {
                    foreach (LainAccount account in accounts)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        string title = c.Decrypt(account.Name());
                        string email = c.Decrypt(account.Email());
                        string password = c.Decrypt(account.Password());
                        if (password.Length == 0) continue;
                        result.CheckedCount++;

                        StrengthResult strength = PasswordStrength.Evaluate(password, title, email);
                        if (strength.Score < PasswordStrength.WeakBelow)
                        {
                            result.Issues.Add(new Issue
                            {
                                Kind = IssueKind.Weak,
                                Account = title,
                                Ref = account,
                                Details = string.IsNullOrEmpty(strength.Hint) ? strength.Label : strength.Label + " \u2014 " + strength.Hint
                            });
                        }

                        // Keep only a process-local keyed fingerprint in the reuse index, never the plaintext password.
                        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                        byte[] digest = null;
                        string passwordFingerprint;
                        try
                        {
                            digest = fingerprint.ComputeHash(passwordBytes);
                            passwordFingerprint = Convert.ToBase64String(digest);
                        }
                        finally
                        {
                            Array.Clear(passwordBytes, 0, passwordBytes.Length);
                            if (digest != null) Array.Clear(digest, 0, digest.Length);
                        }

                        List<LainAccount> same;
                        if (!byPasswordFingerprint.TryGetValue(passwordFingerprint, out same))
                            byPasswordFingerprint[passwordFingerprint] = same = new List<LainAccount>();
                        same.Add(account);

                        AccountMeta meta = AccountMeta.Read(c, account);
                        DateTime? changed = meta.PasswordChanged ?? meta.Created;
                        if (changed.HasValue && (now - changed.Value).TotalDays >= OldAfterDays)
                        {
                            result.Issues.Add(new Issue { Kind = IssueKind.Old, Account = title, Ref = account, Details = "Not changed for " + Age(now - changed.Value) });
                        }
                    }
                }

                foreach (List<LainAccount> same in byPasswordFingerprint.Values)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (same.Count < 2) continue;
                    foreach (LainAccount account in same)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        string others = string.Join(", ", same.Where(a => a != account).Select(a => a.PlainName(key)));
                        result.Issues.Add(new Issue { Kind = IssueKind.Reused, Account = account.PlainName(key), Ref = account, Details = "Same password as " + others });
                    }
                }

                return result;
            }
            finally
            {
                byPasswordFingerprint.Clear();
                Array.Clear(comparisonKey, 0, comparisonKey.Length);
            }
        }

        private void ClearSensitiveReportData()
        {
            foreach (Issue issue in _issues)
            {
                issue.Account = null;
                issue.Details = null;
                issue.Ref = null;
            }
            _issues.Clear();

            foreach (DataGridViewRow row in grid.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value is string) cell.Value = string.Empty;
                }
            }
            grid.Rows.Clear();
            lblEmpty.Text = string.Empty;
            lblStatus.Text = string.Empty;
            lblSubtitle.Text = string.Empty;
        }

        private void SecurityReportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_allowClose) return;
            if (_analysisCancel == null && _breachCancel == null) return;

            e.Cancel = true;
            _closeRequested = true;
            if (_analysisCancel != null) _analysisCancel.Cancel();
            if (_breachCancel != null) _breachCancel.Cancel();
        }

        private void FinishCloseIfRequested()
        {
            if (!_closeRequested || _analysisCancel != null || _breachCancel != null || IsDisposed) return;

            _allowClose = true;
            if (_pendingEditAccount != null)
            {
                EditAccount = _pendingEditAccount;
                DialogResult = DialogResult.OK;
            }
            Close();
        }


        private static string Age(TimeSpan span)
        {
            int months = (int)(span.TotalDays / 30.44);
            if (months < 24) return months + " months";
            return (months / 12) + " years";
        }

        private static string KindText(IssueKind kind)
        {
            switch (kind)
            {
                case IssueKind.Breached: return "Breached";
                case IssueKind.Reused: return "Reused";
                case IssueKind.Weak: return "Weak";
                default: return "Old";
            }
        }

        private static Color KindColor(IssueKind kind)
        {
            switch (kind)
            {
                case IssueKind.Breached: return Fluent.Danger;
                case IssueKind.Reused: return Fluent.Caution;
                case IssueKind.Weak: return Fluent.Warning;
                default: return Fluent.TextSecondary;
            }
        }

        private int Count(IssueKind kind)
        {
            return _issues.Count(i => i.Kind == kind);
        }

        private void Render(LainAccount selectAccount)
        {
            if (selectAccount == null && grid.SelectedRows.Count > 0) selectAccount = grid.SelectedRows[0].Tag as LainAccount;

            Dictionary<IssueKind, DataGridViewCellStyle> issueStyles = new Dictionary<IssueKind, DataGridViewCellStyle>();
            foreach (IssueKind kind in Enum.GetValues(typeof(IssueKind)))
            {
                Color color = KindColor(kind);
                issueStyles[kind] = new DataGridViewCellStyle
                {
                    ForeColor = color,
                    SelectionForeColor = color,
                    Font = _issueFont
                };
            }
            DataGridViewCellStyle detailsStyle = new DataGridViewCellStyle
            {
                ForeColor = Fluent.TextSecondary,
                SelectionForeColor = Fluent.Text
            };

            grid.SuspendLayout();
            try
            {
                grid.Rows.Clear();
                foreach (Issue issue in _issues.OrderBy(i => i.Kind).ThenBy(i => i.Account, StringComparer.CurrentCultureIgnoreCase))
                {
                    int index = grid.Rows.Add(KindText(issue.Kind), issue.Account, issue.Details);
                    DataGridViewRow row = grid.Rows[index];
                    row.Tag = issue.Ref;
                    row.Cells[0].Style = issueStyles[issue.Kind];
                    row.Cells[2].ToolTipText = issue.Details;
                    row.Cells[2].Style = detailsStyle;
                }
            }
            finally
            {
                grid.ResumeLayout();
            }

            grid.ClearSelection();
            DataGridViewRow select = null;
            foreach (DataGridViewRow row in grid.Rows)
            {
                if (selectAccount != null && row.Tag == selectAccount) { select = row; break; }
            }
            if (select != null)
            {
                select.Selected = true;
                grid.CurrentCell = select.Cells[0];
            }
            btnEdit.Enabled = grid.SelectedRows.Count > 0;

            List<string> parts = new List<string>();
            if (_breachChecked) parts.Add(Count(IssueKind.Breached) + " breached");
            parts.Add(Count(IssueKind.Reused) + " reused");
            parts.Add(Count(IssueKind.Weak) + " weak");
            parts.Add(Count(IssueKind.Old) + " old");
            lblSubtitle.Text = string.Format("{0} password{1} checked  \u00B7  {2}", _checkedCount, _checkedCount == 1 ? "" : "s",
                string.Join("  \u00B7  ", parts));

            lblEmpty.Visible = grid.Rows.Count == 0;
            lblEmpty.Text = _checkedCount == 0
                ? "There are no passwords to check yet."
                : "\u2713  No problems found. Your passwords look healthy.";
            lblEmpty.ForeColor = _checkedCount == 0 ? Fluent.TextSecondary : Fluent.Success;
            shieldIcon.GlyphColor = grid.Rows.Count == 0 ? Fluent.Success : (Count(IssueKind.Breached) > 0 ? Fluent.Danger : Fluent.Accent);
            shieldIcon.Invalidate();
            CenterEmptyLabel();
        }

        private void CenterEmptyLabel()
        {
            int top = grid.ColumnHeadersHeight;
            lblEmpty.Width = Math.Max(0, grid.ClientSize.Width - 20);
            lblEmpty.Location = new Point(10, top + Math.Max(0, (grid.ClientSize.Height - top - lblEmpty.Height) / 2));
        }

        private void EditSelected()
        {
            if (_closeRequested || grid.SelectedRows.Count == 0) return;
            LainAccount selected = grid.SelectedRows[0].Tag as LainAccount;
            if (selected == null) return;

            if (_analysisCancel != null || _breachCancel != null)
            {
                _pendingEditAccount = selected;
                _closeRequested = true;
                if (_analysisCancel != null) _analysisCancel.Cancel();
                if (_breachCancel != null) _breachCancel.Cancel();
                return;
            }

            EditAccount = selected;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && grid.SelectedRows.Count > 0)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                EditSelected();
            }
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Lain security report \u2014 " + DateTime.Now.ToString("g"));
            sb.AppendLine(lblSubtitle.Text);
            foreach (IGrouping<IssueKind, Issue> group in _issues.OrderBy(i => i.Kind).GroupBy(i => i.Kind))
            {
                sb.AppendLine();
                sb.AppendLine(KindText(group.Key) + " (" + group.Count() + ")");
                foreach (Issue issue in group.OrderBy(i => i.Account, StringComparer.CurrentCultureIgnoreCase))
                {
                    sb.AppendLine("  " + issue.Account + ": " + issue.Details);
                }
            }

            try
            {
                Clipboard.SetText(sb.ToString());
                btnCopy.Text = "Copied";
                btnCopy.Glyph = Fluent.Glyphs.Checkmark;
            }
            catch { }
        }

        private static BreachCandidates BuildBreachCandidates(VaultKey key, LainAccount[] accounts, CancellationToken cancellationToken)
        {
            Dictionary<string, List<LainAccount>> accountsByHash = new Dictionary<string, List<LainAccount>>(StringComparer.Ordinal);
            try
            {
                using (VaultKey.Cipher c = key.Begin())
                {
                    foreach (LainAccount account in accounts)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        string password = c.Decrypt(account.Password());
                        if (password.Length == 0) continue;

                        string hash = BreachCheck.Sha1Hex(password);
                        List<LainAccount> matchingAccounts;
                        if (!accountsByHash.TryGetValue(hash, out matchingAccounts))
                            accountsByHash[hash] = matchingAccounts = new List<LainAccount>();
                        matchingAccounts.Add(account);
                    }
                }

                return new BreachCandidates
                {
                    AccountsByHash = accountsByHash,
                    Hashes = accountsByHash.Keys.ToList()
                };
            }
            catch
            {
                accountsByHash.Clear();
                throw;
            }
        }

        private async void btnBreach_Click(object sender, EventArgs e)
        {
            if (_breachCancel != null)
            {
                _breachCancel.Cancel();
                return;
            }
            if (_analysisCancel != null) return;

            CancellationTokenSource cancel = new CancellationTokenSource();
            _breachCancel = cancel;
            VaultKey key = null;
            LainAccount[] accounts = null;
            BreachCandidates candidates = null;

            btnBreach.Text = "Cancel check";
            btnBreach.Glyph = Fluent.Glyphs.Cancel;
            lblStatus.ForeColor = Fluent.TextSecondary;
            lblStatus.Text = "Preparing password hashes locally\u2026";

            IProgress<int[]> progress = new Progress<int[]>(p =>
            {
                if (!IsDisposed && !_closeRequested)
                    lblStatus.Text = string.Format("Checking with haveibeenpwned.com\u2026  {0} of {1}", p[0], p[1]);
            });

            try
            {
                key = MainForm.Key;
                accounts = MainForm.Accounts.ToArray();
                candidates = await Task.Run(() => BuildBreachCandidates(key, accounts, cancel.Token), cancel.Token);
                cancel.Token.ThrowIfCancellationRequested();
                if (IsDisposed || _closeRequested) return;
                if (candidates.Hashes.Count == 0)
                {
                    lblStatus.Text = BreachPrivacyNotice;
                    return;
                }

                lblStatus.Text = "Checking with haveibeenpwned.com\u2026";
                Dictionary<string, int> counts = await Task.Run(() =>
                    BreachCheck.Check(candidates.Hashes, cancel.Token, (done, total) => progress.Report(new[] { done, total })), cancel.Token);
                if (IsDisposed || _closeRequested) return;

                _issues.RemoveAll(i => i.Kind == IssueKind.Breached);
                int breached = 0;
                foreach (KeyValuePair<string, int> pair in counts)
                {
                    if (pair.Value <= 0) continue;
                    foreach (LainAccount account in candidates.AccountsByHash[pair.Key])
                    {
                        breached++;
                        _issues.Add(new Issue
                        {
                            Kind = IssueKind.Breached,
                            Account = account.PlainName(key),
                            Ref = account,
                            Details = string.Format("Seen {0:N0} time{1} in data breaches \u2014 change it now", pair.Value, pair.Value == 1 ? "" : "s")
                        });
                    }
                }
                _breachChecked = true;
                Render(null);

                lblStatus.ForeColor = breached > 0 ? Fluent.Danger : Fluent.Success;
                lblStatus.Text = breached > 0
                    ? string.Format("{0} password{1} appeared in known data breaches. Change {2} as soon as possible.",
                        breached, breached == 1 ? "" : "s", breached == 1 ? "it" : "them")
                    : "\u2713  None of your passwords were found in known data breaches.";
            }
            catch (OperationCanceledException)
            {
                if (!IsDisposed && !_closeRequested)
                {
                    lblStatus.Text = "Breach check cancelled.";
                    lblStatus.ForeColor = Fluent.TextSecondary;
                }
            }
            catch (Exception ex)
            {
                if (!IsDisposed && !_closeRequested)
                {
                    lblStatus.ForeColor = Fluent.Danger;
                    lblStatus.Text = "The breach check failed: " + ex.Message;
                }
            }
            finally
            {
                if (candidates != null)
                {
                    foreach (List<LainAccount> matchingAccounts in candidates.AccountsByHash.Values) matchingAccounts.Clear();
                    candidates.AccountsByHash.Clear();
                    candidates.Hashes.Clear();
                }
                if (ReferenceEquals(_breachCancel, cancel)) _breachCancel = null;
                cancel.Dispose();
                if (!IsDisposed && !_closeRequested)
                {
                    btnBreach.Text = _breachChecked ? "Check again" : "Check for breaches";
                    btnBreach.Glyph = "\uE774";
                }
                FinishCloseIfRequested();
            }
        }
    }
}
