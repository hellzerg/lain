using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Media;
using System.Windows.Forms;

namespace Lain
{
    // Fluent replacement for MessageBox: same results, dark themed, sized to its message.
    [DesignerCategory("Code")]
    public sealed class FluentMessageBox : Form
    {
        const int Margin96 = 28;
        const int MinTextWidth = 340;
        const int MaxTextWidth = 500;
        const int FooterHeight = 68;
        const int ButtonHeight = 36;
        const int ButtonMinWidth = 100;
        const int ButtonGap = 8;

        readonly string _text;
        readonly string _title;
        readonly MessageBoxIcon _icon;
        readonly ModernButton _cancel;

        FluentMessageBox(string text, string title, MessageBoxButtons buttons, MessageBoxIcon icon, string yesText, string noText, bool danger)
        {
            _text = text ?? string.Empty;
            _title = string.IsNullOrEmpty(title) ? "Lain" : title;
            _icon = icon;

            SuspendLayout();
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = Fluent.Ui(10F);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            KeyPreview = true;
            Text = "Lain";

            float factor = CurrentAutoScaleDimensions.Width / 96F;
            if (factor <= 0) factor = 1F;
            Func<int, int> logical = px => (int)Math.Ceiling(px / factor);

            // header: optional icon + title
            int x = Margin96;
            int y = 22;
            string glyph = IconGlyph(icon);
            Font titleFont = Fluent.Display(14F);
            int titleHeight = logical(Fluent.MeasureText(_title, titleFont).Height);
            int headerHeight = Math.Max(32, titleHeight);
            if (glyph != null)
            {
                IconLabel iconLabel = new IconLabel
                {
                    Glyph = glyph,
                    IconSize = 18F,
                    GlyphColor = IconColor(icon),
                    Location = new Point(Margin96 - 4, y + (headerHeight - 32) / 2),
                    Size = new Size(32, 32)
                };
                Controls.Add(iconLabel);
                x = Margin96 + 36;
            }

            // message width follows the text, within limits
            Font messageFont = Fluent.Ui(10.5F);
            const TextFormatFlags measure = TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix | TextFormatFlags.TextBoxControl;
            int singleLine = logical(Fluent.MeasureText(_text, messageFont, Size.Empty, TextFormatFlags.NoPrefix).Width) + 4;
            int titleWidth = logical(Fluent.MeasureText(_title, titleFont).Width) + (x - Margin96) + 4;
            int textWidth = Math.Max(MinTextWidth, Math.Min(MaxTextWidth, Math.Max(singleLine, titleWidth)));

            // buttons decide the minimum width too
            List<ModernButton> footerButtons = new List<ModernButton>();
            ModernButton accept = null;
            switch (buttons)
            {
                case MessageBoxButtons.OKCancel:
                    accept = AddButton(footerButtons, "OK", DialogResult.OK, danger ? ButtonVariant.Danger : ButtonVariant.Primary);
                    _cancel = AddButton(footerButtons, "Cancel", DialogResult.Cancel, ButtonVariant.Secondary);
                    break;
                case MessageBoxButtons.YesNo:
                    accept = AddButton(footerButtons, yesText ?? "Yes", DialogResult.Yes, danger ? ButtonVariant.Danger : ButtonVariant.Primary);
                    _cancel = AddButton(footerButtons, noText ?? "No", DialogResult.No, ButtonVariant.Secondary);
                    break;
                case MessageBoxButtons.YesNoCancel:
                    accept = AddButton(footerButtons, yesText ?? "Yes", DialogResult.Yes, danger ? ButtonVariant.Danger : ButtonVariant.Primary);
                    AddButton(footerButtons, noText ?? "No", DialogResult.No, ButtonVariant.Secondary);
                    _cancel = AddButton(footerButtons, "Cancel", DialogResult.Cancel, ButtonVariant.Secondary);
                    break;
                case MessageBoxButtons.RetryCancel:
                    accept = AddButton(footerButtons, "Try again", DialogResult.Retry, ButtonVariant.Primary);
                    _cancel = AddButton(footerButtons, "Cancel", DialogResult.Cancel, ButtonVariant.Secondary);
                    break;
                case MessageBoxButtons.AbortRetryIgnore:
                    accept = AddButton(footerButtons, "Try again", DialogResult.Retry, ButtonVariant.Primary);
                    AddButton(footerButtons, "Ignore", DialogResult.Ignore, ButtonVariant.Secondary);
                    _cancel = AddButton(footerButtons, "Abort", DialogResult.Abort, ButtonVariant.Secondary);
                    break;
                default:
                    accept = AddButton(footerButtons, "OK", DialogResult.OK, ButtonVariant.Primary);
                    _cancel = accept;
                    break;
            }

            int buttonsWidth = 0;
            foreach (ModernButton b in footerButtons)
            {
                b.Width = Math.Max(ButtonMinWidth, logical(b.GetPreferredSize(Size.Empty).Width) + 8);
                b.Height = ButtonHeight;
                buttonsWidth += b.Width + ButtonGap;
            }
            buttonsWidth -= ButtonGap;
            textWidth = Math.Max(textWidth, buttonsWidth);

            FluentLabel titleLabel = new FluentLabel
            {
                AutoSize = false,
                Font = titleFont,
                UseMnemonic = false,
                Text = _title,
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(x, y),
                Size = new Size(Margin96 + textWidth - x, headerHeight)
            };
            Controls.Add(titleLabel);

            y += headerHeight + 12;
            int messageHeight = logical(Fluent.MeasureText(_text, messageFont, new Size((int)(textWidth * factor), int.MaxValue), measure).Height) + 4;
            FluentLabel messageLabel = new FluentLabel
            {
                AutoSize = false,
                Font = messageFont,
                UseMnemonic = false,
                ForeColor = Fluent.TextSecondary,
                Text = _text,
                Location = new Point(Margin96, y),
                Size = new Size(textWidth, messageHeight)
            };
            if (_text.Length > 0) Controls.Add(messageLabel);
            else y -= 12;

            int contentBottom = y + (_text.Length > 0 ? messageHeight : 0) + 26;
            int width = textWidth + Margin96 * 2;

            FooterPanel footer = new FooterPanel { Dock = DockStyle.Bottom, Height = FooterHeight };
            int bx = width - Margin96;
            for (int i = footerButtons.Count - 1; i >= 0; i--)
            {
                ModernButton b = footerButtons[i];
                bx -= b.Width;
                b.Location = new Point(bx, (FooterHeight - ButtonHeight) / 2);
                b.TabIndex = i;
                bx -= ButtonGap;
                footer.Controls.Add(b);
            }
            Controls.Add(footer);

            ClientSize = new Size(width, contentBottom + FooterHeight);
            AcceptButton = accept;
            CancelButton = _cancel;
            ActiveControl = accept;

            ResumeLayout(false);
            PerformLayout();

            Options.ApplyTheme(this);
            Fluent.StyleDialog(this);
        }

        ModernButton AddButton(List<ModernButton> list, string text, DialogResult result, ButtonVariant variant)
        {
            ModernButton b = new ModernButton { Text = text, DialogResult = result, Variant = variant };
            list.Add(b);
            return b;
        }

        static string IconGlyph(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: return "\uEA39";
                case MessageBoxIcon.Warning: return Fluent.Glyphs.Warning;
                case MessageBoxIcon.Question: return "\uE9CE";
                case MessageBoxIcon.Information: return Fluent.Glyphs.Info;
                default: return null;
            }
        }

        static Color IconColor(MessageBoxIcon icon)
        {
            switch (icon)
            {
                case MessageBoxIcon.Error: return Fluent.Danger;
                case MessageBoxIcon.Warning: return Fluent.Warning;
                default: return Fluent.Accent;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            switch (_icon)
            {
                case MessageBoxIcon.Error: SystemSounds.Hand.Play(); break;
                case MessageBoxIcon.Warning: SystemSounds.Exclamation.Play(); break;
                case MessageBoxIcon.Question: SystemSounds.Question.Play(); break;
                case MessageBoxIcon.Information: SystemSounds.Asterisk.Play(); break;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // the title-bar close button behaves like the cancel/no button
            if (DialogResult == DialogResult.Cancel && _cancel != null) DialogResult = _cancel.DialogResult;
            base.OnFormClosing(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                try { Clipboard.SetText(_title + Environment.NewLine + Environment.NewLine + _text); } catch { }
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        internal static DialogResult Show(string text, string title = null, MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Information, string yesText = null, string noText = null, bool danger = false)
        {
            return Show(null, text, title, buttons, icon, yesText, noText, danger);
        }

        internal static DialogResult Show(IWin32Window owner, string text, string title = null, MessageBoxButtons buttons = MessageBoxButtons.OK,
            MessageBoxIcon icon = MessageBoxIcon.Information, string yesText = null, string noText = null, bool danger = false)
        {
            if (owner == null)
            {
                Form active = Form.ActiveForm;
                if (active != null && active.Visible && !active.IsDisposed) owner = active;
            }

            using (FluentMessageBox box = new FluentMessageBox(text, title, buttons, icon, yesText, noText, danger))
            {
                if (owner != null)
                {
                    box.StartPosition = FormStartPosition.CenterParent;
                    box.ShowInTaskbar = false;
                }
                else
                {
                    // nothing to attach to yet (startup): stay findable in the taskbar
                    box.StartPosition = FormStartPosition.CenterScreen;
                    box.ShowInTaskbar = true;
                    box.TopMost = true;
                }
                return owner != null ? box.ShowDialog(owner) : box.ShowDialog();
            }
        }
    }
}
