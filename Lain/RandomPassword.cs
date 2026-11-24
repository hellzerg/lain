using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Lain
{
    public static class RandomPassword
    {
        const int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        const int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        const string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        const string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        const string PASSWORD_CHARS_NUMERIC = "0123456789";
        internal const string DefaultSymbols = "*$-+?_&=!%{}/";
        internal const int MaxSymbols = 32;

        static string _symbols = DefaultSymbols;

        public static bool WithUpperChars { get; set; } = true;
        public static bool WithLowerChars { get; set; } = true;
        public static bool WithNumbers { get; set; } = true;
        public static bool WithSpecialChars { get; set; } = true;

        // Symbols used when WithSpecialChars is on; always sanitized. Empty means no symbols at all.
        public static string Symbols
        {
            get { return _symbols; }
            set { _symbols = SanitizeSymbols(value); }
        }

        // Printable ASCII punctuation only (no letters, digits, spaces or control chars), duplicates removed, order kept.
        internal static string SanitizeSymbols(string value)
        {
            if (string.IsNullOrEmpty(value)) return string.Empty;
            System.Text.StringBuilder sb = new System.Text.StringBuilder(MaxSymbols);
            foreach (char c in value)
            {
                if (c <= ' ' || c > '~' || char.IsLetterOrDigit(c)) continue;
                if (sb.ToString().IndexOf(c) >= 0) continue;
                sb.Append(c);
                if (sb.Length == MaxSymbols) break;
            }
            return sb.ToString();
        }

        internal const int DefaultMinLength = 12;
        internal const int MinLengthFloor = 4, MinLengthCeiling = 128;
        internal const int MinPerTypeFloor = 1, MinPerTypeCeiling = 5;

        internal static int ClampMinLength(int value)
        {
            return Math.Max(MinLengthFloor, Math.Min(MinLengthCeiling, value));
        }

        internal static int ClampMinPerType(int value)
        {
            return Math.Max(MinPerTypeFloor, Math.Min(MinPerTypeCeiling, value));
        }

        public static string Generate()
        {
            return Generate(DEFAULT_MIN_PASSWORD_LENGTH, DEFAULT_MAX_PASSWORD_LENGTH);
        }

        public static string Generate(int length)
        {
            return Generate(length, length);
        }

        public static string Generate(int minLength, int maxLength)
        {
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength) return null;

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                int length = minLength + Next(rng, maxLength - minLength + 1);
                return Build(rng, length, 1);
            }
        }

        // Exactly 'length' characters with at least 'minPerType' of every enabled character type
        // (lowered when the length can't fit that many of each).
        public static string GenerateWithMinimums(int length, int minPerType)
        {
            if (length <= 0) return null;
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                return Build(rng, length, Math.Max(0, minPerType));
            }
        }

        static string Build(RandomNumberGenerator rng, int length, int minPerType)
        {
            List<string> groups = new List<string>(4);
            if (WithLowerChars) groups.Add(PASSWORD_CHARS_LCASE);
            if (WithUpperChars) groups.Add(PASSWORD_CHARS_UCASE);
            if (WithNumbers) groups.Add(PASSWORD_CHARS_NUMERIC);
            string symbols = _symbols;
            if (WithSpecialChars && symbols.Length > 0) groups.Add(symbols);
            if (groups.Count == 0) return string.Empty;

            int perType = Math.Min(minPerType, length / groups.Count);
            string all = string.Concat(groups);
            char[] password = new char[length];
            int i = 0;

            foreach (string g in groups)
            {
                for (int k = 0; k < perType; k++) password[i++] = g[Next(rng, g.Length)];
            }
            while (i < length) password[i++] = all[Next(rng, all.Length)];

            // Fisher-Yates so the guaranteed characters don't sit at fixed positions
            for (int j = length - 1; j > 0; j--)
            {
                int s = Next(rng, j + 1);
                char t = password[j]; password[j] = password[s]; password[s] = t;
            }

            string result = new string(password);
            Array.Clear(password, 0, password.Length);
            return result;
        }

        // Uniform integer in [0, max) using rejection sampling (no modulo bias).
        static int Next(RandomNumberGenerator rng, int max)
        {
            if (max <= 1) return 0;
            byte[] buffer = new byte[4];
            uint limit = uint.MaxValue - (uint.MaxValue % (uint)max);
            uint value;
            do
            {
                rng.GetBytes(buffer);
                value = BitConverter.ToUInt32(buffer, 0);
            } while (value >= limit);
            return (int)(value % (uint)max);
        }
    }
}