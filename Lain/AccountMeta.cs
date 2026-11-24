using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Lain
{
    // Non-secret account metadata. Stored encrypted (as JSON) in the 6th field of each account.
    public sealed class AccountMeta
    {
        internal const int MaxTags = 20;
        internal const int MaxTagLength = 32;

        [JsonProperty("fav")]
        public bool Favorite { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [JsonProperty("created")]
        public DateTime? Created { get; set; }

        [JsonProperty("modified")]
        public DateTime? Modified { get; set; }

        [JsonProperty("pwchanged")]
        public DateTime? PasswordChanged { get; set; }

        [JsonProperty("used")]
        public DateTime? LastUsed { get; set; }

        internal AccountMeta Clone()
        {
            return new AccountMeta
            {
                Favorite = Favorite,
                Tags = new List<string>(Tags ?? new List<string>()),
                Created = Created,
                Modified = Modified,
                PasswordChanged = PasswordChanged,
                LastUsed = LastUsed
            };
        }

        internal static AccountMeta NewNow()
        {
            DateTime now = DateTime.UtcNow;
            return new AccountMeta { Created = now, Modified = now, PasswordChanged = now };
        }

        // Trimmed, case-insensitively unique, bounded tags; commas separate tags so they can't be part of one.
        internal static List<string> NormalizeTags(IEnumerable<string> tags)
        {
            List<string> result = new List<string>();
            if (tags == null) return result;

            foreach (string raw in tags)
            {
                if (raw == null) continue;
                foreach (string part in raw.Split(','))
                {
                    string t = new string(part.Where(c => !char.IsControl(c)).ToArray()).Trim();
                    if (t.Length == 0) continue;
                    if (t.Length > MaxTagLength) t = t.Substring(0, MaxTagLength).Trim();
                    if (result.Any(x => string.Equals(x, t, StringComparison.OrdinalIgnoreCase))) continue;
                    result.Add(t);
                    if (result.Count >= MaxTags) return result;
                }
            }
            return result;
        }

        internal static List<string> ParseTags(string text)
        {
            return NormalizeTags(new[] { text ?? string.Empty });
        }

        internal string TagText()
        {
            return string.Join(", ", Tags ?? new List<string>());
        }

        // Returns the account's metadata (cached after the first read); never throws for bad or missing data.
        internal static AccountMeta Read(VaultKey key, LainAccount account)
        {
            if (account.CachedMeta != null) return account.CachedMeta;
            return Parse(account, string.IsNullOrEmpty(account.Meta()) ? null : (Func<string>)(() => key.Decrypt(account.Meta())));
        }

        internal static AccountMeta Read(VaultKey.Cipher cipher, LainAccount account)
        {
            if (account.CachedMeta != null) return account.CachedMeta;
            return Parse(account, string.IsNullOrEmpty(account.Meta()) ? null : (Func<string>)(() => cipher.Decrypt(account.Meta())));
        }

        static AccountMeta Parse(LainAccount account, Func<string> decrypt)
        {
            AccountMeta meta = null;
            if (decrypt != null)
            {
                try
                {
                    meta = JsonConvert.DeserializeObject<AccountMeta>(decrypt(), new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.None,
                        DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                        MaxDepth = 8
                    });
                }
                catch { meta = null; }
            }

            meta = meta ?? new AccountMeta();
            meta.Tags = NormalizeTags(meta.Tags);
            account.CachedMeta = meta;
            return meta;
        }

        internal static void Write(VaultKey key, LainAccount account, AccountMeta meta)
        {
            meta.Tags = NormalizeTags(meta.Tags);
            string json = JsonConvert.SerializeObject(meta, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                NullValueHandling = NullValueHandling.Ignore
            });
            account.SetMeta(key.Encrypt(json), meta);
        }
    }
}
