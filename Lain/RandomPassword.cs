using System;
using System.Linq;
using System.Security.Cryptography;

namespace Lain
{
    public sealed class RandomPassword
    {
        private static int DEFAULT_MIN_PASSWORD_LENGTH = 8;
        private static int DEFAULT_MAX_PASSWORD_LENGTH = 10;

        private static string PASSWORD_CHARS_LCASE = "abcdefgijkmnopqrstwxyz";
        private static string PASSWORD_CHARS_UCASE = "ABCDEFGHJKLMNPQRSTWXYZ";
        private static string PASSWORD_CHARS_NUMERIC = "0123456789";
        private static string PASSWORD_CHARS_SPECIAL = "*$-+?_&=!%{}/";

        public static bool WithUpperChars { get; set; } = true;
        public static bool WithLowerChars { get; set; } = true;
        public static bool WithNumbers { get; set; } = true;
        public static bool WithSpecialChars { get; set; } = true;

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
            if (minLength <= 0 || maxLength <= 0 || minLength > maxLength)
                return null;

            char[][] charGroups = new char[][]
            {
                WithLowerChars ? PASSWORD_CHARS_LCASE.ToCharArray() : null,
                WithUpperChars ? PASSWORD_CHARS_UCASE.ToCharArray() : null,
                WithNumbers ? PASSWORD_CHARS_NUMERIC.ToCharArray() : null,
                WithSpecialChars ? PASSWORD_CHARS_SPECIAL.ToCharArray() : null
            }
            .Where(arr => arr != null).ToArray();

            if (charGroups.Length <= 0)
            {
                return string.Empty;
            }

            int[] charsLeftInGroup = new int[charGroups.Length];

            for (int i = 0; i < charsLeftInGroup.Length; i++)
                charsLeftInGroup[i] = charGroups[i].Length;

            int[] leftGroupsOrder = new int[charGroups.Length];

            for (int i = 0; i < leftGroupsOrder.Length; i++)
                leftGroupsOrder[i] = i;

            byte[] randomBytes = new byte[4];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);

            int seed = BitConverter.ToInt32(randomBytes, 0);

            Random random = new Random(seed);

            char[] password = null;

            if (minLength < maxLength)
                password = new char[random.Next(minLength, maxLength + 1)];
            else
                password = new char[minLength];

            int nextCharIdx;

            int nextGroupIdx;

            int nextLeftGroupsOrderIdx;

            int lastCharIdx;

            int lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;

            for (int i = 0; i < password.Length; i++)
            {
                if (lastLeftGroupsOrderIdx == 0)
                    nextLeftGroupsOrderIdx = 0;
                else
                    nextLeftGroupsOrderIdx = random.Next(0,
                                                         lastLeftGroupsOrderIdx);

                nextGroupIdx = leftGroupsOrder[nextLeftGroupsOrderIdx];

                lastCharIdx = charsLeftInGroup[nextGroupIdx] - 1;

                if (lastCharIdx == 0)
                    nextCharIdx = 0;
                else
                    nextCharIdx = random.Next(0, lastCharIdx + 1);

                password[i] = charGroups[nextGroupIdx][nextCharIdx];

                if (lastCharIdx == 0)
                    charsLeftInGroup[nextGroupIdx] =
                                              charGroups[nextGroupIdx].Length;
                else
                {
                    if (lastCharIdx != nextCharIdx)
                    {
                        char temp = charGroups[nextGroupIdx][lastCharIdx];
                        charGroups[nextGroupIdx][lastCharIdx] =
                                    charGroups[nextGroupIdx][nextCharIdx];
                        charGroups[nextGroupIdx][nextCharIdx] = temp;
                    }
                    charsLeftInGroup[nextGroupIdx]--;
                }

                if (lastLeftGroupsOrderIdx == 0)
                    lastLeftGroupsOrderIdx = leftGroupsOrder.Length - 1;
                else
                {
                    if (lastLeftGroupsOrderIdx != nextLeftGroupsOrderIdx)
                    {
                        int temp = leftGroupsOrder[lastLeftGroupsOrderIdx];
                        leftGroupsOrder[lastLeftGroupsOrderIdx] =
                                    leftGroupsOrder[nextLeftGroupsOrderIdx];
                        leftGroupsOrder[nextLeftGroupsOrderIdx] = temp;
                    }
                    lastLeftGroupsOrderIdx--;
                }
            }

            return new string(password);
        }
    }
}

