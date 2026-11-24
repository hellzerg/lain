using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Lain
{
    // Small Fluent dialogs: a single-line text prompt and the one-time recovery code display.
    [DesignerCategory("Code")]
    public sealed class FluentPrompt : Form
    {
        const int Width96 = 444;
        const int FooterHeight = 68;

        FluentTextBox _input;
        ModernButton _ok;

        FluentPrompt(string title, string glyph, Color glyphColor, string message)
        {
            SuspendLayout();
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            Font = Fluent.Ui(10F);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Lain";

            Controls.Add(new IconLabel { Glyph = glyph, IconSize = 18F, GlyphColor = glyphColor, Location = new Point(24, 18), Size = new Size(36, 36) });
            Controls.Add(new FluentLabel { AutoSize = true, Font = Fluent.Display(16F), UseMnemonic = false, Text = title, Location = new Point(62, 20) });
            Controls.Add(new FluentLabel { ForeColor = Fluent.TextSecondary, UseMnemonic = false, Text = message, Location = new Point(30, 64), Size = new Size(384, 42) });
        }

        void Finish(int contentBottom, ModernButton accept, ModernButton cancel)
        {
            FooterPanel footer = new FooterPanel { Dock = DockStyle.Bottom, Height = FooterHeight };
            int x = Width96 - 30;
            foreach (ModernButton b in new[] { accept, cancel })
            {
                if (b == null) continue;
                b.Size = new Size(110, 36);
                x -= b.Width;
                b.Location = new Point(x, 16);
                x -= 8;
                footer.Controls.Add(b);
            }
            Controls.Add(footer);

            ClientSize = new Size(Width96, contentBottom + FooterHeight);
            AcceptButton = accept;
            CancelButton = cancel ?? accept;
            ResumeLayout(false);
            PerformLayout();

            Options.ApplyTheme(this);
            Fluent.StyleDialog(this);
        }

        // Returns the trimmed text, or null when cancelled.
        internal static string AskText(IWin32Window owner, string title, string message, string initial, string placeholder, int maxLength, string acceptText = "Save")
        {
            using (FluentPrompt f = new FluentPrompt(title, Fluent.Glyphs.Edit, Fluent.Accent, message))
            {
                InputBox box = new InputBox { Location = new Point(30, 112), Size = new Size(384, 38), TabIndex = 0 };
                f._input = new FluentTextBox { Font = Fluent.Ui(11.5F), MaxLength = maxLength, Text = initial ?? string.Empty, Size = new Size(320, 20) };
                box.SetEditor(f._input);
                Fluent.SetPlaceholder(f._input, placeholder);
                f.Controls.Add(box);

                f._ok = new ModernButton { Text = acceptText, Variant = ButtonVariant.Primary, DialogResult = DialogResult.OK };
                ModernButton cancel = new ModernButton { Text = "Cancel", DialogResult = DialogResult.Cancel };
                f._input.TextChanged += (s, e) => f._ok.Enabled = f._input.Text.Trim().Length > 0;
                f._ok.Enabled = f._input.Text.Trim().Length > 0;
                f.Finish(176, f._ok, cancel);
                f.ActiveControl = f._input;
                f._input.SelectAll();

                return f.ShowDialog(owner) == DialogResult.OK ? f._input.Text.Trim() : null;
            }
        }

        // Shows a newly generated recovery code once; the dialog closes only after the user confirms saving it.
        internal static void ShowRecoveryCode(IWin32Window owner, string code)
        {
            using (FluentPrompt f = new FluentPrompt("Save your recovery code", Fluent.Glyphs.Shield, Fluent.Caution,
                "Without your keys, this code and your master password are the only way in. It won't be shown again."))
            {
                Card card = new Card { Location = new Point(30, 116), Size = new Size(384, 64) };
                FluentLabel recoveryLabel = new FluentLabel
                {
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    Font = Fluent.Mono(12F),
                    UseMnemonic = false,
                    Text = code,
                    TextAlign = ContentAlignment.MiddleCenter
                };
                card.Controls.Add(recoveryLabel);
                f.Controls.Add(card);

                ModernButton copy = new ModernButton { Text = "Copy", Glyph = Fluent.Glyphs.Copy, Variant = ButtonVariant.Secondary, Location = new Point(30, 192), Size = new Size(110, 32), TabIndex = 1 };
                copy.Click += (s, e) =>
                {
                    SecureClipboard.CopySecret(code);
                    copy.Text = "Copied";
                };
                f.Controls.Add(copy);

                MoonCheck saved = new MoonCheck { Text = "I saved this code somewhere safe", AutoSize = false, Size = new Size(384, 28), Location = new Point(30, 238), TabIndex = 0 };
                f.Controls.Add(saved);

                ModernButton done = new ModernButton { Text = "Done", Variant = ButtonVariant.Primary, DialogResult = DialogResult.OK, Enabled = false };
                saved.CheckedChanged += (s, e) => done.Enabled = saved.Checked;
                f.FormClosing += (s, e) => { if (!saved.Checked && e.CloseReason == CloseReason.UserClosing) e.Cancel = true; };
                f.FormClosed += (s, e) => { recoveryLabel.Text = string.Empty; code = string.Empty; };
                f.Finish(284, done, null);
                f.CancelButton = null;
                f.ControlBox = false;
                f.ActiveControl = copy;

                f.ShowDialog(owner);
            }
        }
    }
}
