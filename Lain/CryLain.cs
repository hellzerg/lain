using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Lain
{
    internal sealed class CryLain
    {
        // default SALT
        // can be overriden from command-line
        internal static string SALT = "e3080105715443ef14ef2a09be11fa9d";

        internal static string HashKey(string key)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(key + SALT));
                return Convert.ToBase64String(hashed);
            }
        }

        private static string HashKeyCustomSalt(string key, string customSalt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashed = sha256.ComputeHash(Encoding.UTF8.GetBytes(key + customSalt));
                return Convert.ToBase64String(hashed);
            }
        }

        internal static SecureString ToSecureString(string key)
        {
            SecureString ss = new SecureString();

            foreach (char c in key)
            {
                ss.AppendChar(c);
            }

            ss.MakeReadOnly();
            return ss;
        }

        internal static string ToInsecureString(SecureString key)
        {
            string s = string.Empty;
            IntPtr ip = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(key);

            try
            {
                s = System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ip);
            }
            catch { }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ip);
            }

            return s;
        }

        internal string Encrypt(string key, string data)
        {
            string encrypted = null;
            byte[][] keys = GetHashKeys(key);

            try
            {
                encrypted = Protect(data, keys[0], keys[1]);
            }
            catch { }

            return encrypted;
        }

        internal string Decrypt(string key, string data)
        {
            string decrypted = null;
            byte[][] keys = GetHashKeys(key);

            try
            {
                decrypted = Unprotect(data, keys[0], keys[1]);
            }
            catch { }

            return decrypted;
        }

        internal static bool ChangeSalt(string toSalt, string oldKey)
        {
            string _newKey = CryLain.HashKeyCustomSalt(oldKey, toSalt);

            if (string.IsNullOrEmpty(oldKey) || string.IsNullOrEmpty(_newKey)) return false;

            try
            {
                File.WriteAllText(Required.LainSerial, _newKey);
                return true;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
                return false;
            }
            finally
            {
                oldKey = string.Empty;
                _newKey = string.Empty;
                toSalt = string.Empty;
            }
        }

        private string Protect(string plainText, byte[] Key, byte[] IV)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            byte[] encrypted;

            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt =
                            new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        private string Unprotect(string cipherTextString, byte[] Key, byte[] IV)
        {
            byte[] cipherText = Convert.FromBase64String(cipherTextString);

            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plainText = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt =
                            new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }

        private byte[][] GetHashKeys(string key)
        {
            byte[][] result = new byte[2][];
            Encoding enc = Encoding.UTF8;

            SHA256 sha256 = new SHA256CryptoServiceProvider();

            byte[] rawKey = enc.GetBytes(key);
            byte[] rawIV = enc.GetBytes(key);

            byte[] hashKey = sha256.ComputeHash(rawKey);
            byte[] hashIV = sha256.ComputeHash(rawIV);

            Array.Resize(ref hashIV, 16);

            result[0] = hashKey;
            result[1] = hashIV;

            return result;
        }
    }
}
