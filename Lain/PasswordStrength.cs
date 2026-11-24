using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace Lain
{
    internal struct StrengthResult
    {
        internal int Score;      // 0 (very weak) .. 4 (very strong); -1 for empty
        internal double Bits;    // rough effective entropy
        internal string Label;
        internal string Hint;

        internal Color Color
        {
            get
            {
                switch (Score)
                {
                    case 0: return Fluent.Danger;
                    case 1: return Fluent.Caution;
                    case 2: return Fluent.Warning;
                    default: return Fluent.Success;
                }
            }
        }
    }

    // Offline password strength estimate: effective entropy of the characters after discounting repeats,
    // sequences, keyboard runs, years and common passwords/words. Deliberately conservative.
    internal static class PasswordStrength
    {
        internal const int WeakBelow = 2;
        internal const int MinimumMasterPasswordLength = 15;
        internal const int MinimumMfaMasterPasswordLength = 12;
        internal const int MinimumMasterPasswordScore = 3;

        internal static bool IsAcceptableMasterPassword(string password)
        {
            return IsAcceptableMasterPassword(password, MinimumMasterPasswordLength);
        }

        internal static bool IsAcceptableMasterPassword(string password, int minimumLength)
        {
            if (string.IsNullOrEmpty(password) || minimumLength < 1) return false;
            int characterCount = CountUnicodeScalars(password);
            return characterCount >= minimumLength && Evaluate(password).Score >= MinimumMasterPasswordScore;
        }

        // Count Unicode scalar values rather than UTF-16 code units. Reject unpaired surrogates so distinct
        // malformed strings cannot collapse to the same replacement-character byte sequence during KDF input.
        static int CountUnicodeScalars(string value)
        {
            int count = 0;
            for (int i = 0; i < value.Length; i++)
            {
                char current = value[i];
                if (char.IsHighSurrogate(current))
                {
                    if (i + 1 >= value.Length || !char.IsLowSurrogate(value[i + 1])) return -1;
                    i++;
                }
                else if (char.IsLowSurrogate(current))
                {
                    return -1;
                }
                count++;
            }
            return count;
        }

        static readonly string[] Rows = { "`1234567890-=", "qwertyuiop[]\\", "asdfghjkl;'", "zxcvbnm,./" };

        static readonly HashSet<string> Common = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "password", "passw0rd", "p@ssword", "p@ssw0rd", "123456", "12345678", "123456789", "1234567890", "qwerty", "qwertyuiop",
            "abc123", "111111", "000000", "letmein", "welcome", "monkey", "dragon", "master", "login", "admin", "administrator",
            "iloveyou", "sunshine", "princess", "football", "baseball", "shadow", "superman", "batman", "trustno1", "starwars",
            "whatever", "freedom", "secret", "hello", "charlie", "michael", "jordan", "hunter", "ranger", "pokemon", "computer",
            "internet", "qazwsx", "zaq12wsx", "asdfgh", "asdfghjkl", "1q2w3e4r", "1qaz2wsx", "changeme", "default", "guest",
            "root", "test", "summer", "winter", "spring", "autumn", "love", "lovely", "family", "flower", "cookie", "chocolate",
            "soccer", "hockey", "killer", "matrix", "mustang", "access", "pass", "passwort", "motdepasse"
        };

        internal static StrengthResult Evaluate(string password, params string[] context)
        {
            StrengthResult r = new StrengthResult { Score = -1, Label = string.Empty, Hint = string.Empty };
            if (string.IsNullOrEmpty(password)) return r;

            int pool = 0;
            bool lower = password.Any(char.IsLower), upper = password.Any(char.IsUpper), digit = password.Any(char.IsDigit);
            bool symbol = password.Any(c => !char.IsLetterOrDigit(c) && c < 128), unicode = password.Any(c => c >= 128);
            if (lower) pool += 26;
            if (upper) pool += 26;
            if (digit) pool += 10;
            if (symbol) pool += 33;
            if (unicode) pool += 100;
            if (pool == 0) pool = 26;
            double perChar = Math.Log(pool, 2);

            string lowerPw = password.ToLowerInvariant();
            double bits = 0;
            for (int i = 0; i < password.Length; i++)
            {
                if (i > 0 && IsPredictable(lowerPw, i)) bits += 1.5;
                else bits += perChar;
            }

            // a short unit repeated (e.g. "abcabcabc") is barely stronger than the unit itself
            int unit = RepeatUnit(password);
            if (unit < password.Length) bits = Math.Min(bits, bits * unit / password.Length + Math.Log(password.Length / unit, 2) + 1);

            string hint = string.Empty;
            string core = StripDecorations(lowerPw);
            if (Common.Contains(lowerPw) || Common.Contains(core) || Common.Contains(Unleet(core)))
            {
                bits = Math.Min(bits, 10 + Math.Max(0, password.Length - core.Length) * 3);
                hint = "Based on a very common password";
            }
            else if (context != null && context.Any(c => !string.IsNullOrEmpty(c) && c.Length >= 3 && lowerPw.Contains(c.Trim().ToLowerInvariant())))
            {
                bits -= 12;
                hint = "Avoid using the title or username";
            }
            else if (ContainsYear(password))
            {
                bits -= 6;
            }

            bits = Math.Max(0, bits);
            r.Bits = bits;
            r.Score = bits < 28 ? 0 : bits < 40 ? 1 : bits < 60 ? 2 : bits < 80 ? 3 : 4;

            switch (r.Score)
            {
                case 0: r.Label = "Very weak"; break;
                case 1: r.Label = "Weak"; break;
                case 2: r.Label = "Fair"; break;
                case 3: r.Label = "Strong"; break;
                default: r.Label = "Very strong"; break;
            }

            if (hint.Length == 0 && r.Score < 3)
            {
                if (password.Length < MinimumMasterPasswordLength) hint = "Use a longer passphrase";
                else hint = "Avoid repeats and predictable sequences";
            }
            r.Hint = hint;
            return r;
        }

        static bool IsPredictable(string s, int i)
        {
            char a = s[i - 1], b = s[i];
            if (a == b) return true;
            if (char.IsLetterOrDigit(a) && char.IsLetterOrDigit(b) && Math.Abs(a - b) == 1) return true;

            foreach (string row in Rows)
            {
                int ia = row.IndexOf(a), ib = row.IndexOf(b);
                if (ia >= 0 && ib >= 0 && Math.Abs(ia - ib) == 1) return true;
            }
            return false;
        }

        static int RepeatUnit(string s)
        {
            for (int len = 1; len <= s.Length / 2; len++)
            {
                if (s.Length % len != 0) continue;
                bool repeats = true;
                for (int i = len; i < s.Length && repeats; i++) repeats = s[i] == s[i % len];
                if (repeats) return len;
            }
            return s.Length;
        }

        static string StripDecorations(string s)
        {
            int start = 0, end = s.Length;
            while (start < end && !char.IsLetter(s[start])) start++;
            while (end > start && !char.IsLetter(s[end - 1])) end--;
            return end > start ? s.Substring(start, end - start) : s;
        }

        static string Unleet(string s)
        {
            return s.Replace('@', 'a').Replace('4', 'a').Replace('3', 'e').Replace('1', 'i').Replace('!', 'i')
                .Replace('0', 'o').Replace('$', 's').Replace('5', 's').Replace('7', 't');
        }

        static bool ContainsYear(string s)
        {
            for (int i = 0; i + 4 <= s.Length; i++)
            {
                int year;
                if (int.TryParse(s.Substring(i, 4), out year) && year >= 1940 && year <= DateTime.Now.Year + 5) return true;
            }
            return false;
        }
    }

    // Four-segment strength bar with a label, shown under password fields.
    [DesignerCategory("Code")]
    public sealed class StrengthMeter : Control
    {
        StrengthResult _result = new StrengthResult { Score = -1, Label = string.Empty, Hint = string.Empty };

        public StrengthMeter()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.Selectable, false);
            TabStop = false;
            Font = Fluent.Ui(9F);
        }

        internal StrengthResult Result { get { return _result; } }

        internal void Evaluate(string password, params string[] context)
        {
            _result = PasswordStrength.Evaluate(password, context);
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Color back = Fluent.EffectiveBackColor(Parent);
            g.Clear(back);

            int barWidth = Math.Min(Fluent.Scale(this, 168), Width / 2);
            int gap = Fluent.Scale(this, 4);
            int h = Fluent.Scale(this, 4);
            int y = (Height - h) / 2;
            int seg = (barWidth - gap * 3) / 4;
            int filled = _result.Score < 0 ? 0 : Math.Min(4, _result.Score + 1);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            for (int i = 0; i < 4; i++)
            {
                Color c = i < filled ? _result.Color : Fluent.Control;
                using (GraphicsPath path = Fluent.RoundedRect(new RectangleF(i * (seg + gap), y, seg, h), h / 2f))
                using (SolidBrush b = new SolidBrush(c))
                {
                    g.FillPath(b, path);
                }
            }

            if (_result.Score < 0) return;

            int x = barWidth + Fluent.Scale(this, 12);
            Rectangle rect = new Rectangle(x, 0, Width - x, Height);
            TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding;
            Fluent.DrawText(g, _result.Label, Font, rect, _result.Color, back, flags);

            if (_result.Hint.Length > 0)
            {
                int lw = Fluent.MeasureText(g, _result.Label, Font, Size.Empty, flags).Width + Fluent.Scale(this, 8);
                Fluent.DrawText(g, "\u00B7  " + _result.Hint, Font, new Rectangle(x + lw, 0, Width - x - lw, Height), Fluent.TextSecondary, back,
                    flags | TextFormatFlags.EndEllipsis);
            }
        }
    }
}
