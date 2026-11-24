using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Lain
{
    // Have I Been Pwned "Pwned Passwords" lookup using k-anonymity: only the first 5 hex characters of the
    // SHA-1 hash ever leave the machine, and padded responses hide which suffix was of interest.
    internal static class BreachCheck
    {
        const string RangeUrl = "https://api.pwnedpasswords.com/range/";

        internal static string Sha1Hex(string password)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            try
            {
                using (SHA1 sha = SHA1.Create())
                {
                    byte[] hash = sha.ComputeHash(bytes);
                    StringBuilder sb = new StringBuilder(40);
                    foreach (byte b in hash) sb.Append(b.ToString("X2"));
                    Array.Clear(hash, 0, hash.Length);
                    return sb.ToString();
                }
            }
            finally
            {
                Array.Clear(bytes, 0, bytes.Length);
            }
        }

        // Returns how many times each hash appears in known breaches (0 = not found). One request per distinct prefix.
        internal static Dictionary<string, int> Check(IEnumerable<string> sha1Hashes, CancellationToken cancel, Action<int, int> progress)
        {
            Dictionary<string, HashSet<string>> byPrefix = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
            foreach (string h in sha1Hashes)
            {
                cancel.ThrowIfCancellationRequested();
                if (h == null || h.Length != 40) continue;
                string prefix = h.Substring(0, 5);
                HashSet<string> hashes;
                if (!byPrefix.TryGetValue(prefix, out hashes)) byPrefix[prefix] = hashes = new HashSet<string>(StringComparer.Ordinal);
                hashes.Add(h);
            }

            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.Ordinal);
            int done = 0;
            int progressInterval = Math.Max(1, (byPrefix.Count + 99) / 100);
            foreach (KeyValuePair<string, HashSet<string>> pair in byPrefix)
            {
                cancel.ThrowIfCancellationRequested();
                Dictionary<string, int> range = QueryRange(pair.Key, cancel);
                foreach (string h in pair.Value)
                {
                    int count;
                    result[h] = range.TryGetValue(h.Substring(5), out count) ? count : 0;
                }
                done++;
                if (progress != null && (done == 1 || done == byPrefix.Count || done % progressInterval == 0))
                    progress(done, byPrefix.Count);
            }
            return result;
        }

        static Dictionary<string, int> QueryRange(string prefix, CancellationToken cancel)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(RangeUrl + prefix);
            request.Method = "GET";
            request.UserAgent = "Lain-Password-Manager";
            request.Headers["Add-Padding"] = "true";
            request.Timeout = 15000;
            request.ReadWriteTimeout = 15000;
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);

            using (cancel.Register(() => request.Abort()))
            {
                try
                {
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII))
                    {
                        Dictionary<string, int> range = new Dictionary<string, int>(StringComparer.Ordinal);
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            int colon = line.IndexOf(':');
                            if (colon != 35) continue;
                            int count;
                            if (int.TryParse(line.Substring(colon + 1).Trim(), out count) && count > 0)
                                range[line.Substring(0, 35).ToUpperInvariant()] = count;
                        }
                        return range;
                    }
                }
                catch (WebException) when (cancel.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancel);
                }
            }
        }
    }
}
