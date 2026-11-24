using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace Lain
{
    /*
     * Current vault format v4
     *
     *  KDF             : PBKDF2-HMAC-SHA256; 64 derived bytes = AES-256 key + HMAC-SHA256 key.
     *  Field encryption: AES-256-GCM; random 96-bit nonce and 128-bit authentication tag per field.
     *                    base64( 0x03 | nonce[12] | ciphertext | tag[16] ); the field version is AAD.
     *  Data section    : "LAINVLT4" | KDF id | AEAD id | KEM id | iterations | salt | verifier |
     *                    count | 6 fields per account | HMAC-SHA256(file).
     *  Suite ids       : KDF 1 = PBKDF2-HMAC-SHA256; AEAD 2 = AES-256-GCM; KEM 0 = none.
     *                    Unknown ids are rejected rather than silently downgraded.
     *
     *  Formats "LAINVLT3" and "LAINVLT2" and field version 0x02 (AES-256-CBC + HMAC-SHA256) remain
     *  readable. After a successful unlock, their fields are re-encrypted and the data section is rewritten
     *  as v4. The KEM id is explicit so a future version can add a standardized key-encapsulation scheme
     *  (for example ML-KEM) without changing the AES-GCM field encoding.
     *
     * Legacy v1 vaults - unsalted SHA-256 key, key-derived static IV, no MAC, BinaryFormatter data - are
     * migrated automatically the first time they are unlocked. The v1 data is read by LegacyVaultReader, a
     * strict parser that never instantiates types from the file.
     */
    internal static partial class CryLain
    {
        // Legacy v1 only: salt for the login hash. Can be overridden from the command-line or Lain.salt.
        internal static string SALT = "e3080105715443ef14ef2a09be11fa9d";

        internal const int Iterations = 600000;
        const int MinVaultIterations = 1;
        const int MaxVaultIterations = 10000000;
        const int SaltSize = 16;
        const int VerifierSize = 32;
        const string SerialPrefix = "lain2$";
        const int CurrentDataVersion = 4;
        const byte KdfPbkdf2Sha256 = 1;
        const byte AeadLegacyCbcHmacSha256 = 1;
        const byte AeadAes256Gcm = 2;
        const byte KemNone = 0;
        internal const byte LegacyFieldVersion = 2;
        internal const byte GcmFieldVersion = 3;
        static readonly byte[] DataMagic = Encoding.ASCII.GetBytes("LAINVLT4");
        static readonly byte[] DataMagicV3 = Encoding.ASCII.GetBytes("LAINVLT3");
        static readonly byte[] DataMagicV2 = Encoding.ASCII.GetBytes("LAINVLT2");
        static readonly byte[] LegacyVerifierLabel = Encoding.UTF8.GetBytes("lain-vault-verifier-v2");
        static readonly byte[] CurrentVerifierLabel = Encoding.UTF8.GetBytes("lain-vault-verifier-v4|kdf=1|aead=2|kem=0");

        sealed class VaultHeader
        {
            internal int Iterations;
            internal byte[] Salt;
            internal byte[] Verifier;
            internal int FormatVersion;
            internal byte KdfId;
            internal byte AeadId;
            internal byte KemId;
            internal int HeaderLength;
        }

        #region Public API

        internal static bool IsModernVault()
        {
            return File.Exists(Required.LainVault)
                || (File.Exists(Required.LainSerial) && File.ReadAllText(Required.LainSerial).StartsWith(SerialPrefix, StringComparison.Ordinal))
                || HasModernData();
        }

        // Creates a new, empty vault protected by the given master password.
        internal static VaultKey CreateVault(string password)
        {
            EnsureAcceptableMasterPassword(password);

            // never silently overwrite files that don't belong to a vault
            string stamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            foreach (string file in new[] { Required.LainVault, Required.LainData, Required.LainKeys })
            {
                if (File.Exists(file)) File.Move(file, file + "." + stamp + ".bak");
            }

            VaultHeader header;
            VaultKey key = NewKey(password, out header);
            try
            {
                WriteVault(BuildData(key, header, new List<LainAccount>()), null);
                DeleteSaltFile();
                return key;
            }
            catch
            {
                key.Dispose();
                throw;
            }
        }

        // Returns the vault key and decrypted-on-demand accounts, or null when the password is wrong.
        // Throws InvalidDataException when the vault fails its integrity checks.
        internal static VaultKey Unlock(string password, out List<LainAccount> accounts)
        {
            return Unlock(new UnlockFactor { Password = password }, out accounts);
        }

        // Unlocks with the factors the vault's key mode needs (see CryLain.Keys.cs); null when they are wrong.
        internal static VaultKey Unlock(UnlockFactor factor, out List<LainAccount> accounts)
        {
            accounts = null;
            if (!VaultExists()) return null;

            string password = factor.Password;
            if (!IsModernVault()) return string.IsNullOrEmpty(password) ? null : MigrateLegacy(password, out accounts);

            VaultFiles files = LoadFiles();
            if (files.Keys != null) return UnlockKeyed(factor, files, out accounts);
            if (string.IsNullOrEmpty(password)) return null;

            VaultHeader header = HeaderOf(files);
            VaultKey key = OpenKey(password, header);
            if (key == null) return null;

            try
            {
                accounts = files.Data != null ? ReadData(key, files.Data) : new List<LainAccount>();
                key = CompleteUnlock(key, header, files, ref accounts, factor);
                return key;
            }
            catch
            {
                key.Dispose();
                throw;
            }
        }

        // session: the unlocked vault key, needed in "password and key" mode where the password alone opens nothing.
        internal static bool VerifyPassword(string password, VaultKey session = null)
        {
            if (string.IsNullOrEmpty(password) || !VaultExists()) return false;
            if (!IsModernVault()) return VerifyLegacyPassword(password);

            VaultFiles files = LoadFiles();
            if (files.Keys != null) return Verify(new UnlockFactor { Password = password }, session);
            using (VaultKey key = OpenKey(password, HeaderOf(files))) return key != null;
        }

        internal static void SaveVault(VaultKey key, List<LainAccount> accounts)
        {
            VaultFiles files = LoadFiles();
            VaultHeader header = HeaderOf(files);
            if (header.FormatVersion != CurrentDataVersion || (files.Keys != null && header.Iterations < Iterations))
            {
                VaultHeader upgradedHeader = CreateCurrentHeader(key, header, files.Keys != null);
                List<LainAccount> upgradedAccounts = ReencryptAccounts(key, accounts);
                byte[] upgradedData = BuildData(key, upgradedHeader, upgradedAccounts);
                ReadData(key, upgradedData);
                WriteVault(upgradedData, files.Keys);
                accounts.Clear();
                accounts.AddRange(upgradedAccounts);
                RetireSplitFiles();
                return;
            }

            WriteVault(BuildData(key, header, accounts), files.Keys);
        }

        // Re-encrypts the whole vault under a new master password (new salt, new keys). The new vault is built and
        // verified in memory and both files are backed up before anything is replaced; on failure the old vault stays.
        // Returns the new key and accounts; the caller owns them and must dispose the old key.
        // With security keys only the password slot is rewrapped (the vault key stays the same).
        internal static VaultKey ChangeMasterPassword(VaultKey oldKey, List<LainAccount> current, string newPassword, out List<LainAccount> accounts)
        {
            accounts = null;
            EnsureAcceptableMasterPassword(newPassword, GetRequiredMasterPasswordLength());
            if (HasKeyFile()) return ChangeKeyedPassword(oldKey, current, newPassword, out accounts);

            VaultHeader currentHeader = HeaderOf(LoadFiles());
            VaultHeader header;
            VaultKey key = NewKey(newPassword, TargetKdfIterations(currentHeader.Iterations), out header);
            return Rekey(oldKey, current, key, header, null,
                () => OpenKey(newPassword, HeaderOf(LoadFiles())), ChangeFailed, out accounts);
        }

        // Re-encrypts every account under newKey (taking ownership of it) and replaces the vault files. buildKeys
        // produces the new key file, or is null when the vault must not have one. reopen re-derives the key from the
        // files on disk to prove the new factors work. Everything is verified and backed up; on failure nothing changes.
        static VaultKey Rekey(VaultKey oldKey, List<LainAccount> current, VaultKey newKey, VaultHeader header, Func<VaultKey, byte[]> buildKeys,
            Func<VaultKey> reopen, Func<string, Exception> failed, out List<LainAccount> accounts)
        {
            accounts = null;
            List<string[]> plain = new List<string[]>(current.Count);
            VaultKey key = newKey;
            string backup = null;
            bool hadVault = File.Exists(Required.LainVault);
            try
            {
                using (VaultKey.Cipher c = oldKey.Begin())
                {
                    foreach (LainAccount a in current)
                    {
                        plain.Add(new[] { c.Decrypt(a.Name()), c.Decrypt(a.Email()), c.Decrypt(a.Password()),
                            c.Decrypt(a.Link()), c.Decrypt(a.Note()), c.Decrypt(a.Meta()) });
                    }
                }

                List<LainAccount> rekeyed = new List<LainAccount>(plain.Count);
                using (VaultKey.Cipher c = key.Begin())
                {
                    foreach (string[] p in plain)
                    {
                        rekeyed.Add(new LainAccount(c.Encrypt(p[0]), c.Encrypt(p[1]), c.Encrypt(p[2]), c.Encrypt(p[3]), c.Encrypt(p[4]),
                            p[5].Length == 0 ? string.Empty : c.Encrypt(p[5])));
                    }
                }

                byte[] data = BuildData(key, header, rekeyed);
                List<LainAccount> check = ReadData(key, data);
                if (check.Count != plain.Count) throw failed("the re-encrypted vault could not be verified");
                using (VaultKey.Cipher c = key.Begin())
                {
                    for (int i = 0; i < check.Count; i++)
                    {
                        string[] got = { c.Decrypt(check[i].Name()), c.Decrypt(check[i].Email()), c.Decrypt(check[i].Password()),
                            c.Decrypt(check[i].Link()), c.Decrypt(check[i].Note()), c.Decrypt(check[i].Meta()) };
                        for (int f = 0; f < got.Length; f++)
                        {
                            if (!string.Equals(got[f], plain[i][f], StringComparison.Ordinal)) throw failed("the re-encrypted vault could not be verified");
                        }
                        Array.Clear(got, 0, got.Length);
                    }
                }
                byte[] keys = buildKeys == null ? null : buildKeys(key);

                backup = BackupVaultFile(failed);

                try
                {
                    // one atomic write: data and key file change together
                    WriteVault(data, keys);
                }
                catch (Exception ex)
                {
                    RestoreVaultFile(backup, hadVault);
                    throw failed("the new vault could not be written: " + ex.Message);
                }

                // the file on disk must open with the new factors
                bool reopenedOk = false;
                try
                {
                    using (VaultKey reopened = reopen())
                    {
                        reopenedOk = reopened != null && ReadData(reopened, LoadFiles().Data).Count == plain.Count;
                    }
                }
                catch (Exception ex) when (ex is InvalidDataException || ex is CryptographicException || ex is IOException) { }
                if (!reopenedOk)
                {
                    RestoreVaultFile(backup, hadVault);
                    throw failed("the new vault could not be verified on disk");
                }

                if (backup != null) WipeFile(backup);
                backup = null;
                WipeSplitLeftovers();
                DeleteSaltFile();

                accounts = check;
                return key;
            }
            catch
            {
                if (key != null) key.Dispose();
                throw;
            }
            finally
            {
                foreach (string[] p in plain) Array.Clear(p, 0, p.Length);
            }
        }

        static Exception ChangeFailed(string reason)
        {
            return new InvalidDataException("The master password could not be changed (" + reason + "). Your vault still uses the old password.");
        }

        // Copies Lain.vault aside before it is replaced; null when the vault is still in split files (those aren't touched).
        static string BackupVaultFile(Func<string, Exception> failed)
        {
            if (!File.Exists(Required.LainVault)) return null;

            string backup = Required.LainVault + ".rekey-" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".bak";
            try
            {
                File.Copy(Required.LainVault, backup, false);
                File.SetAttributes(backup, FileAttributes.Normal);
                if (!FixedTimeEquals(File.ReadAllBytes(Required.LainVault), File.ReadAllBytes(backup))) throw new IOException("the copy does not match the original");
                return backup;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                WipeFile(backup);
                throw failed("a backup of Lain.vault could not be made: " + ex.Message);
            }
        }

        // Puts the pre-change vault back (best effort; the backup is kept if anything goes wrong).
        static void RestoreVaultFile(string backup, bool hadVault)
        {
            try
            {
                if (backup != null)
                {
                    byte[] original = File.ReadAllBytes(backup);
                    if (!File.Exists(Required.LainVault) || !FixedTimeEquals(File.ReadAllBytes(Required.LainVault), original)) WriteAtomic(Required.LainVault, original);
                }
                else if (!hadVault && File.Exists(Required.LainVault)) File.Delete(Required.LainVault);
                if (backup != null) WipeFile(backup);
            }
            catch { }
        }

        #endregion

        #region Key derivation

        static void EnsureAcceptableMasterPassword(string password)
        {
            EnsureAcceptableMasterPassword(password, PasswordStrength.MinimumMasterPasswordLength);
        }

        static void EnsureAcceptableMasterPassword(string password, int minimumLength)
        {
            if (!PasswordStrength.IsAcceptableMasterPassword(password, minimumLength))
            {
                throw new ArgumentException(
                    "The master password must be at least " + minimumLength +
                    " characters long and have a Strong or Very strong rating.", "password");
            }
        }

        static bool IsValidVaultIterationCount(int iterations)
        {
            return iterations >= MinVaultIterations && iterations <= MaxVaultIterations;
        }

        // Never lower an existing work factor. Older vaults are raised to the current baseline.
        internal static int TargetKdfIterations(int existingIterations)
        {
            if (!IsValidVaultIterationCount(existingIterations))
                throw new ArgumentOutOfRangeException("existingIterations", "The existing PBKDF2 iteration count is outside the supported range.");

            return Math.Max(Iterations, existingIterations);
        }

        internal static bool ShouldUpgradePasswordKdf(int existingIterations, bool passwordAvailable)
        {
            return passwordAvailable && existingIterations < Iterations;
        }

        static VaultHeader NewCurrentHeader()
        {
            return new VaultHeader
            {
                Iterations = Iterations,
                Salt = RandomBytes(SaltSize),
                FormatVersion = CurrentDataVersion,
                KdfId = KdfPbkdf2Sha256,
                AeadId = AeadAes256Gcm,
                KemId = KemNone,
                HeaderLength = DataMagic.Length + 3 + 4 + SaltSize + VerifierSize
            };
        }

        static byte[] VerifierLabelFor(VaultHeader header)
        {
            return header.FormatVersion >= CurrentDataVersion ? CurrentVerifierLabel : LegacyVerifierLabel;
        }

        static VaultHeader CreateCurrentHeader(VaultKey key, VaultHeader previous, bool hasKeyFile)
        {
            VaultHeader header = NewCurrentHeader();
            if (hasKeyFile)
            {
                // The key file, not this header, derives the password wrapping key for keyed vaults.
                // Keep this metadata at least at the current baseline without changing the random data key.
                header.Iterations = TargetKdfIterations(previous.Iterations);
            }
            else
            {
                // A password-only vault still uses this header's salt and count to derive its data key.
                // Preserve both unless a full password-KDF rekey is performed.
                header.Salt = (byte[])previous.Salt.Clone();
                header.Iterations = previous.Iterations;
            }

            header.Verifier = key.Mac(VerifierLabelFor(header));
            return header;
        }

        static VaultKey NewKey(string password, out VaultHeader header)
        {
            return NewKey(password, Iterations, out header);
        }

        static VaultKey NewKey(string password, int iterations, out VaultHeader header)
        {
            header = NewCurrentHeader();
            header.Iterations = TargetKdfIterations(iterations);
            VaultKey key = new VaultKey(DeriveKey(password, header.Salt, header.Iterations));
            header.Verifier = key.Mac(VerifierLabelFor(header));
            return key;
        }

        static VaultKey OpenKey(string password, VaultHeader header)
        {
            if (header.KdfId != KdfPbkdf2Sha256)
                throw new InvalidDataException("This vault uses an unsupported password key-derivation algorithm.");

            VaultKey key = new VaultKey(DeriveKey(password, header.Salt, header.Iterations));
            if (FixedTimeEquals(key.Mac(VerifierLabelFor(header)), header.Verifier)) return key;

            key.Dispose();
            return null;
        }

        [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
        static extern int BCryptOpenAlgorithmProvider(out IntPtr handle, string algId, string implementation, int flags);

        [DllImport("bcrypt.dll")]
        static extern int BCryptDeriveKeyPBKDF2(IntPtr prf, byte[] password, int cbPassword, byte[] salt, int cbSalt, ulong iterations, byte[] derivedKey, int cbDerivedKey, int flags);

        [DllImport("bcrypt.dll")]
        static extern int BCryptCloseAlgorithmProvider(IntPtr handle, int flags);

        const int BCRYPT_ALG_HANDLE_HMAC_FLAG = 0x8;

        static byte[] DeriveKey(string password, byte[] salt, int iterations)
        {
            if (!IsValidVaultIterationCount(iterations))
                throw new ArgumentOutOfRangeException("iterations", "The vault PBKDF2 iteration count is outside the supported range.");

            byte[] pw = Encoding.UTF8.GetBytes(password);
            byte[] derived = new byte[VaultKey.MaterialSize];

            try
            {
                // native CNG is ~10x faster than the managed implementation, allowing a high iteration count
                IntPtr alg;
                if (BCryptOpenAlgorithmProvider(out alg, "SHA256", null, BCRYPT_ALG_HANDLE_HMAC_FLAG) == 0)
                {
                    try
                    {
                        if (BCryptDeriveKeyPBKDF2(alg, pw, pw.Length, salt, salt.Length, (ulong)iterations, derived, derived.Length, 0) == 0) return derived;
                    }
                    finally
                    {
                        BCryptCloseAlgorithmProvider(alg, 0);
                    }
                }
            }
            catch (DllNotFoundException) { }
            catch (EntryPointNotFoundException) { }
            finally
            {
                Array.Clear(pw, 0, pw.Length);
            }

            Array.Clear(derived, 0, derived.Length);
            using (Rfc2898DeriveBytes kdf = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                return kdf.GetBytes(VaultKey.MaterialSize);
            }
        }

        #endregion

        #region Files

        static VaultHeader ParseSerial(string serial)
        {
            string[] parts = serial.Trim().Split('$');
            int iterations;
            if (parts.Length != 4 || parts[0] + "$" != SerialPrefix || !int.TryParse(parts[1], out iterations) || !IsValidVaultIterationCount(iterations))
                throw new InvalidDataException("The vault serial file is damaged.");

            VaultHeader h = new VaultHeader
            {
                Iterations = iterations,
                Salt = Convert.FromBase64String(parts[2]),
                Verifier = Convert.FromBase64String(parts[3]),
                FormatVersion = 3,
                KdfId = KdfPbkdf2Sha256,
                AeadId = AeadLegacyCbcHmacSha256,
                KemId = KemNone,
                HeaderLength = DataMagicV3.Length + 4 + SaltSize + VerifierSize
            };
            if (h.Salt.Length != SaltSize || h.Verifier.Length != VerifierSize) throw new InvalidDataException("The vault serial file is damaged.");
            return h;
        }

        static bool HasModernData()
        {
            if (!File.Exists(Required.LainData)) return false;
            using (FileStream fs = File.OpenRead(Required.LainData))
            {
                byte[] magic = new byte[DataMagic.Length];
                return fs.Read(magic, 0, magic.Length) == magic.Length && DataFieldCount(magic) > 0;
            }
        }

        // Number of fields per account for a data-file magic, or 0 when it isn't a modern vault.
        static int DataFieldCount(byte[] magic)
        {
            if (FixedTimeEquals(magic, DataMagic) || FixedTimeEquals(magic, DataMagicV3)) return 6;
            if (FixedTimeEquals(magic, DataMagicV2)) return 5;
            return 0;
        }

        static int DataFormatVersion(byte[] magic)
        {
            if (FixedTimeEquals(magic, DataMagic)) return 4;
            if (FixedTimeEquals(magic, DataMagicV3)) return 3;
            if (FixedTimeEquals(magic, DataMagicV2)) return 2;
            return 0;
        }

        static VaultHeader ReadDataHeader(byte[] file)
        {
            try
            {
                using (BinaryReader r = new BinaryReader(new MemoryStream(file)))
                {
                    byte[] magic = r.ReadBytes(DataMagic.Length);
                    int version = DataFormatVersion(magic);
                    if (version == 0) throw new InvalidDataException("The vault data file is not in a recognized format.");

                    VaultHeader h = new VaultHeader
                    {
                        FormatVersion = version,
                        KdfId = KdfPbkdf2Sha256,
                        AeadId = version >= CurrentDataVersion ? AeadAes256Gcm : AeadLegacyCbcHmacSha256,
                        KemId = KemNone
                    };

                    if (version >= CurrentDataVersion)
                    {
                        h.KdfId = r.ReadByte();
                        h.AeadId = r.ReadByte();
                        h.KemId = r.ReadByte();
                    }

                    h.Iterations = r.ReadInt32();
                    h.Salt = r.ReadBytes(SaltSize);
                    h.Verifier = r.ReadBytes(VerifierSize);
                    h.HeaderLength = checked((int)r.BaseStream.Position);

                    if (!IsValidVaultIterationCount(h.Iterations) || h.Salt.Length != SaltSize || h.Verifier.Length != VerifierSize)
                        throw new InvalidDataException("The vault data file is damaged.");

                    if (h.KdfId != KdfPbkdf2Sha256 || h.KemId != KemNone ||
                        (h.AeadId != AeadLegacyCbcHmacSha256 && h.AeadId != AeadAes256Gcm) ||
                        (version >= CurrentDataVersion && h.AeadId != AeadAes256Gcm))
                        throw new InvalidDataException("This vault uses an unsupported cryptographic suite. Update Lain to a compatible version.");

                    return h;
                }
            }
            catch (EndOfStreamException)
            {
                throw new InvalidDataException("The vault data file is damaged.");
            }
        }

        static byte[] BuildData(VaultKey key, VaultHeader header, List<LainAccount> accounts)
        {
            if (header.FormatVersion != CurrentDataVersion || header.KdfId != KdfPbkdf2Sha256 ||
                header.AeadId != AeadAes256Gcm || header.KemId != KemNone)
                throw new InvalidDataException("The requested vault cryptographic suite is not supported for writing.");

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter w = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    w.Write(DataMagic);
                    w.Write(header.KdfId);
                    w.Write(header.AeadId);
                    w.Write(header.KemId);
                    w.Write(header.Iterations);
                    w.Write(header.Salt);
                    w.Write(header.Verifier);
                    w.Write(accounts.Count);
                    foreach (LainAccount a in accounts)
                    {
                        w.Write(a.Name() ?? string.Empty);
                        w.Write(a.Email() ?? string.Empty);
                        w.Write(a.Password() ?? string.Empty);
                        w.Write(a.Link() ?? string.Empty);
                        w.Write(a.Note() ?? string.Empty);
                        w.Write(a.Meta() ?? string.Empty);
                    }
                    w.Flush();
                    w.Write(key.Mac(ms.GetBuffer(), 0, checked((int)ms.Length)));
                }

                return ms.ToArray();
            }
        }

        static List<LainAccount> ReadData(VaultKey key, byte[] file)
        {
            const string damaged = "The vault data failed its integrity check. It may be corrupted or have been tampered with.";

            if (file == null || file.Length < DataMagic.Length + 4 + SaltSize + VerifierSize + 4 + 32)
                throw new InvalidDataException(damaged);

            VaultHeader header = ReadDataHeader(file);
            if (file.Length < header.HeaderLength + 4 + 32) throw new InvalidDataException(damaged);

            byte[] body = new byte[file.Length - 32];
            byte[] mac = new byte[32];
            Buffer.BlockCopy(file, 0, body, 0, body.Length);
            Buffer.BlockCopy(file, body.Length, mac, 0, 32);
            if (!FixedTimeEquals(key.Mac(body), mac)) throw new InvalidDataException(damaged);

            try
            {
                List<LainAccount> accounts = new List<LainAccount>();
                using (VaultKey.Cipher c = key.Begin())
                using (BinaryReader r = new BinaryReader(new MemoryStream(body), Encoding.UTF8))
                {
                    int fields = DataFieldCount(r.ReadBytes(DataMagic.Length));
                    if (fields == 0) throw new InvalidDataException(damaged);
                    r.BaseStream.Position = header.HeaderLength;
                    int count = r.ReadInt32();
                    if (count < 0) throw new InvalidDataException(damaged);

                    for (int i = 0; i < count; i++)
                    {
                        LainAccount a = new LainAccount(r.ReadString(), r.ReadString(), r.ReadString(), r.ReadString(), r.ReadString(),
                            fields >= 6 ? r.ReadString() : string.Empty);
                        // every value is authenticated individually as well
                        byte expectedFieldVersion = header.AeadId == AeadAes256Gcm ? GcmFieldVersion : LegacyFieldVersion;
                        c.Verify(a.Name(), expectedFieldVersion); c.Verify(a.Email(), expectedFieldVersion);
                        c.Verify(a.Password(), expectedFieldVersion); c.Verify(a.Link(), expectedFieldVersion);
                        c.Verify(a.Note(), expectedFieldVersion); c.Verify(a.Meta(), expectedFieldVersion);
                        accounts.Add(a);
                    }
                    if (r.BaseStream.Position != r.BaseStream.Length) throw new InvalidDataException(damaged);
                }
                return accounts;
            }
            catch (Exception ex) when (ex is EndOfStreamException || ex is CryptographicException)
            {
                throw new InvalidDataException(damaged);
            }
        }

        static List<LainAccount> ReencryptAccounts(VaultKey key, List<LainAccount> accounts)
        {
            List<LainAccount> reencrypted = new List<LainAccount>(accounts.Count);
            List<string[]> plaintext = new List<string[]>(accounts.Count);
            try
            {
                using (VaultKey.Cipher cipher = key.Begin())
                {
                    foreach (LainAccount account in accounts)
                    {
                        plaintext.Add(new[]
                        {
                            cipher.Decrypt(account.Name()),
                            cipher.Decrypt(account.Email()),
                            cipher.Decrypt(account.Password()),
                            cipher.Decrypt(account.Link()),
                            cipher.Decrypt(account.Note()),
                            cipher.Decrypt(account.Meta())
                        });
                    }
                }

                using (VaultKey.Cipher cipher = key.Begin())
                {
                    foreach (string[] values in plaintext)
                    {
                        reencrypted.Add(new LainAccount(
                            cipher.Encrypt(values[0]),
                            cipher.Encrypt(values[1]),
                            cipher.Encrypt(values[2]),
                            cipher.Encrypt(values[3]),
                            cipher.Encrypt(values[4]),
                            values[5].Length == 0 ? string.Empty : cipher.Encrypt(values[5])));
                    }
                }

                return reencrypted;
            }
            finally
            {
                foreach (string[] values in plaintext) Array.Clear(values, 0, values.Length);
            }
        }

        static void ReplaceAccounts(List<LainAccount> target, List<LainAccount> replacement)
        {
            target.Clear();
            target.AddRange(replacement);
        }

        static void WriteAtomic(string path, byte[] bytes)
        {
            string tmp = path + ".tmp";
            try
            {
                using (FileStream fs = new FileStream(tmp, FileMode.Create, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush(true);
                }

                if (!File.Exists(path))
                {
                    File.Move(tmp, path);
                    return;
                }

                try
                {
                    File.Replace(tmp, path, null);
                }
                catch (PlatformNotSupportedException)
                {
                    File.Delete(path);
                    File.Move(tmp, path);
                }
            }
            catch
            {
                try { if (File.Exists(tmp)) File.Delete(tmp); } catch (IOException) { } catch (UnauthorizedAccessException) { }
                throw;
            }
        }

        #endregion

        #region Helpers

        internal static byte[] RandomBytes(int count)
        {
            byte[] b = new byte[count];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) rng.GetBytes(b);
            return b;
        }

        internal static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }

        #endregion

        #region Legacy v1 (migration and salt tools only)

        internal static string HashKey(string key)
        {
            return HashKeyCustomSalt(key, SALT);
        }

        static string HashKeyCustomSalt(string key, string customSalt)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(key + customSalt)));
            }
        }

        internal static bool VerifyLegacyPassword(string password)
        {
            return FixedTimeEquals(Encoding.UTF8.GetBytes(HashKey(password)), Encoding.UTF8.GetBytes(File.ReadAllText(Required.LainSerial)));
        }

        internal static bool ChangeSalt(string toSalt, string oldKey)
        {
            if (string.IsNullOrEmpty(oldKey)) return false;

            try
            {
                File.WriteAllText(Required.LainSerial, HashKeyCustomSalt(oldKey, toSalt));
                return true;
            }
            catch (Exception err)
            {
                FluentMessageBox.Show(err.Message, "Lain", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Environment.Exit(0);
                return false;
            }
        }

        // Upgrades a v1 vault directly to v4. Nothing is written until the new vault has been built and verified
        // in memory, and the original v1 files are copied to a backup before they are replaced.
        static VaultKey MigrateLegacy(string password, out List<LainAccount> accounts)
        {
            accounts = null;
            if (!VerifyLegacyPassword(password)) return null;

            List<LainAccount> legacy = File.Exists(Required.LainData) ? ReadLegacyData(File.ReadAllBytes(Required.LainData)) : new List<LainAccount>();
            List<string[]> plain = new List<string[]>(legacy.Count);
            VaultKey key = null;
            try
            {
                foreach (LainAccount a in legacy)
                {
                    plain.Add(new[] { LegacyDecrypt(password, a.Name()), LegacyDecrypt(password, a.Email()), LegacyDecrypt(password, a.Password()),
                        LegacyDecrypt(password, a.Link()), LegacyDecrypt(password, a.Note()) });
                }

                VaultHeader header;
                key = NewKey(password, out header);

                List<LainAccount> migrated = new List<LainAccount>(plain.Count);
                foreach (string[] p in plain) migrated.Add(new LainAccount(key.Encrypt(p[0]), key.Encrypt(p[1]), key.Encrypt(p[2]), key.Encrypt(p[3]), key.Encrypt(p[4])));

                // build the new vault and read it back in memory; every field of every account must match
                byte[] data = BuildData(key, header, migrated);
                List<LainAccount> check = ReadData(key, data);
                if (check.Count != plain.Count) throw MigrationFailed("the upgraded vault could not be verified");
                for (int i = 0; i < check.Count; i++)
                {
                    string[] got = { key.Decrypt(check[i].Name()), key.Decrypt(check[i].Email()), key.Decrypt(check[i].Password()),
                        key.Decrypt(check[i].Link()), key.Decrypt(check[i].Note()) };
                    for (int f = 0; f < got.Length; f++)
                    {
                        if (!string.Equals(got[f], plain[i][f], StringComparison.Ordinal)) throw MigrationFailed("the upgraded vault could not be verified");
                    }
                    Array.Clear(got, 0, got.Length);
                }

                BackupLegacyFiles();

                // Keep the v1 backups until the v4 file has been reopened and its password verifier,
                // whole-file HMAC, field authentication and migrated contents have all been checked.
                List<LainAccount> persistedAccounts;
                try
                {
                    WriteVault(data, null);
                    persistedAccounts = VerifyMigratedLegacyVault(password, plain);
                }
                catch (Exception ex)
                {
                    // A partial or unverifiable Lain.vault would take precedence over the intact v1 files.
                    try { if (File.Exists(Required.LainVault)) File.Delete(Required.LainVault); }
                    catch (IOException) { }
                    catch (UnauthorizedAccessException) { }

                    if (File.Exists(Required.LainVault))
                        throw MigrationFailed("the new vault could not be verified and the incomplete Lain.vault could not be removed. Your original v1 files and backups are still present. Remove the incomplete Lain.vault before retrying. " + ex.Message);

                    throw MigrationFailed("the upgraded vault could not be written and verified: " + ex.Message);
                }

                foreach (string file in new[] { Required.LainData, Required.LainSerial })
                {
                    try { File.Delete(file); }
                    catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { }
                }
                DeleteSaltFile();

                // A verified v4 vault is now authoritative. Remove only v1 migration backups; split-vault
                // backups remain subject to their separate retention policy.
                if (!DeleteV1Backups())
                {
                    try { System.Diagnostics.Trace.TraceWarning("Lain migrated the v1 vault successfully, but one or more v1 backup files could not be deleted."); }
                    catch { }
                }

                Options.CurrentOptions.LegacyBackupUnlocks = 0;
                TrySaveSettings();

                accounts = persistedAccounts;
                return key;
            }
            catch
            {
                if (key != null) key.Dispose();
                throw;
            }
            finally
            {
                foreach (string[] p in plain) Array.Clear(p, 0, p.Length);
            }
        }

        // Reopen the file from disk, independently derive its data key from the supplied password,
        // authenticate the complete v4 data section, and compare every migrated field before backups are removed.
        static List<LainAccount> VerifyMigratedLegacyVault(string password, List<string[]> expected)
        {
            VaultFiles files = LoadFiles();
            if (!files.Merged || files.Data == null || files.Keys != null)
                throw new InvalidDataException("the written vault container is incomplete or contains unexpected security-key data");

            VaultHeader header = HeaderOf(files);
            if (header.FormatVersion != CurrentDataVersion || header.Iterations < Iterations)
                throw new InvalidDataException("the written vault is not using the current v4 format and password work factor");

            using (VaultKey verifiedKey = OpenKey(password, header))
            {
                if (verifiedKey == null)
                    throw new InvalidDataException("the written vault could not be reopened with the original master password");

                List<LainAccount> persisted = ReadData(verifiedKey, files.Data);
                if (persisted.Count != expected.Count)
                    throw new InvalidDataException("the written vault contains a different number of accounts");

                using (VaultKey.Cipher cipher = verifiedKey.Begin())
                {
                    for (int i = 0; i < persisted.Count; i++)
                    {
                        string[] actual =
                        {
                            cipher.Decrypt(persisted[i].Name()),
                            cipher.Decrypt(persisted[i].Email()),
                            cipher.Decrypt(persisted[i].Password()),
                            cipher.Decrypt(persisted[i].Link()),
                            cipher.Decrypt(persisted[i].Note())
                        };

                        try
                        {
                            if (expected[i] == null || expected[i].Length != actual.Length)
                                throw new InvalidDataException("the written vault contains an invalid account record");

                            for (int field = 0; field < actual.Length; field++)
                            {
                                if (!string.Equals(actual[field], expected[i][field], StringComparison.Ordinal))
                                    throw new InvalidDataException("the written vault does not match the original account data");
                            }
                        }
                        finally
                        {
                            Array.Clear(actual, 0, actual.Length);
                        }
                    }
                }

                return persisted;
            }
        }

        static InvalidDataException MigrationFailed(string reason)
        {
            return new InvalidDataException("Lain could not upgrade your vault to the new format (" + reason + "). Your vault has not been changed.");
        }

        static List<LainAccount> ReadLegacyData(byte[] bytes)
        {
            try
            {
                return LegacyVaultReader.Read(bytes);
            }
            catch (InvalidDataException ex)
            {
                throw new InvalidDataException(ex.Message + " Your vault has not been changed.");
            }
        }

        #region Legacy backups

        // Retention for split-vault backups and any v1 backups left behind by an older build or blocked deletion.
        // Backups created by the current v1 migration are removed immediately after the v4 vault is verified.
        internal const int BackupRetentionUnlocks = 3;
        const string BackupTag = ".v1-";

        static void BackupLegacyFiles()
        {
            string stamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            foreach (string file in new[] { Required.LainData, Required.LainSerial, Required.LainSalt })
            {
                if (!File.Exists(file)) continue;

                string backup = file + BackupTag + stamp + ".bak";
                try
                {
                    File.Copy(file, backup, false);
                    if (!FixedTimeEquals(File.ReadAllBytes(file), File.ReadAllBytes(backup))) throw new IOException("the copy does not match the original");
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    throw MigrationFailed("a backup of " + Path.GetFileName(file) + " could not be made: " + ex.Message);
                }
            }
        }

        // Deletes v1 migration backups immediately after the replacement v4 vault has been reopened and verified.
        // WipeFile is best-effort (storage media may retain previous blocks); File.Delete is retried if wiping fails.
        internal static bool DeleteV1Backups()
        {
            List<string> backups;
            try { backups = FindBackups(BackupTag); }
            catch { return false; }

            bool allDeleted = true;
            foreach (string backup in backups)
            {
                try
                {
                    if (!File.Exists(backup)) continue;

                    try { File.SetAttributes(backup, FileAttributes.Normal); }
                    catch { }

                    if (!WipeFile(backup) && File.Exists(backup))
                    {
                        try
                        {
                            File.SetAttributes(backup, FileAttributes.Normal);
                            File.Delete(backup);
                        }
                        catch { }
                    }

                    if (File.Exists(backup)) allDeleted = false;
                }
                catch
                {
                    allDeleted = false;
                }
            }

            return allDeleted;
        }

        // Backups kept for BackupRetentionUnlocks: split files from before Lain.vault and v1 backups that
        // could not be deleted immediately or were created by an older build.
        internal static string[] FindLegacyBackups()
        {
            List<string> found = new List<string>(FindBackups(BackupTag));
            found.AddRange(FindBackups(SplitTag));
            return found.ToArray();
        }

        static List<string> FindBackups(string tag)
        {
            List<string> found = new List<string>();
            if (!Directory.Exists(Required.DataFolder)) return found;
            foreach (string file in new[] { Required.LainData, Required.LainSerial, Required.LainKeys, Required.LainSalt })
            {
                foreach (string b in Directory.GetFiles(Required.DataFolder, Path.GetFileName(file) + tag + "*.bak"))
                {
                    if (b.EndsWith(".bak", StringComparison.OrdinalIgnoreCase)) found.Add(b);
                }
            }
            return found;
        }

        // Called after a modern vault was unlocked and fully read; retires legacy backups once the new vault has proven itself.
        static void NoteSuccessfulUnlock()
        {
            try
            {
                string[] backups = FindLegacyBackups();
                int unlocks = backups.Length == 0 ? 0 : Options.CurrentOptions.LegacyBackupUnlocks + 1;

                if (unlocks >= BackupRetentionUnlocks)
                {
                    bool allGone = true;
                    foreach (string b in backups) allGone &= WipeFile(b);
                    unlocks = allGone ? 0 : unlocks;
                }

                if (unlocks != Options.CurrentOptions.LegacyBackupUnlocks)
                {
                    Options.CurrentOptions.LegacyBackupUnlocks = unlocks;
                    TrySaveSettings();
                }
            }
            catch { }
        }

        // Overwrites a file with zeros before deleting it (best effort; SSDs may keep old copies).
        static bool WipeFile(string path)
        {
            try
            {
                long length = new FileInfo(path).Length;
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.None, 4096, FileOptions.WriteThrough))
                {
                    byte[] zeros = new byte[4096];
                    for (long done = 0; done < length; done += zeros.Length) fs.Write(zeros, 0, (int)Math.Min(zeros.Length, length - done));
                    fs.Flush(true);
                }
                File.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static void TrySaveSettings()
        {
            try
            {
                Options.SaveSettings();
            }
            catch { }
        }

        #endregion

        static string LegacyDecrypt(string password, string data)
        {
            // v1 stored empty values as null
            if (data == null) return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] key = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                byte[] iv = new byte[16];
                Buffer.BlockCopy(key, 0, iv, 0, 16);

                try
                {
                    using (Aes aes = Aes.Create())
                    using (ICryptoTransform dec = aes.CreateDecryptor(key, iv))
                    {
                        byte[] cipher = Convert.FromBase64String(data);
                        return Encoding.UTF8.GetString(dec.TransformFinalBlock(cipher, 0, cipher.Length));
                    }
                }
                catch (Exception ex) when (ex is CryptographicException || ex is FormatException)
                {
                    throw MigrationFailed("an entry could not be decrypted");
                }
                finally
                {
                    Array.Clear(key, 0, key.Length);
                }
            }
        }

        #endregion
    }

    // Holds the derived vault keys, encrypted in memory with DPAPI except for the brief moment they are used.
    internal sealed class VaultKey : IDisposable
    {
        internal const int MaterialSize = 64;
        const int KeySize = 32;

        readonly byte[] _material = new byte[MaterialSize];
        readonly object _sync = new object();
        byte[] _secret;
        bool _disposed;

        internal VaultKey(byte[] material)
        {
            Buffer.BlockCopy(material, 0, _material, 0, MaterialSize);
            Array.Clear(material, 0, material.Length);
            ProtectedMemory.Protect(_material, MemoryProtectionScope.SameProcess);
        }

        // Copy of the raw key material; clear it as soon as it has been used.
        internal byte[] Material()
        {
            byte[] copy = new byte[MaterialSize];
            lock (_sync)
            {
                if (_disposed) throw new ObjectDisposedException("VaultKey");
                ProtectedMemory.Unprotect(_material, MemoryProtectionScope.SameProcess);
                Buffer.BlockCopy(_material, 0, copy, 0, MaterialSize);
                ProtectedMemory.Protect(_material, MemoryProtectionScope.SameProcess);
            }
            return copy;
        }

        // The security-key secret of a vault with security keys (32 bytes), kept for rewrapping while unlocked.
        internal void SetSecret(byte[] secret)
        {
            lock (_sync)
            {
                if (_disposed) throw new ObjectDisposedException("VaultKey");
                if (_secret != null) Array.Clear(_secret, 0, _secret.Length);
                _secret = null;
                if (secret == null) return;
                if (secret.Length % 16 != 0) throw new ArgumentException("Invalid secret size.");
                _secret = (byte[])secret.Clone();
                ProtectedMemory.Protect(_secret, MemoryProtectionScope.SameProcess);
            }
        }

        internal bool HasSecret { get { lock (_sync) return _secret != null; } }

        // Copy of the security-key secret or null; clear it after use.
        internal byte[] Secret()
        {
            lock (_sync)
            {
                if (_disposed) throw new ObjectDisposedException("VaultKey");
                if (_secret == null) return null;
                byte[] copy = new byte[_secret.Length];
                ProtectedMemory.Unprotect(_secret, MemoryProtectionScope.SameProcess);
                Buffer.BlockCopy(_secret, 0, copy, 0, copy.Length);
                ProtectedMemory.Protect(_secret, MemoryProtectionScope.SameProcess);
                return copy;
            }
        }

        internal VaultKey Clone()
        {
            VaultKey k = new VaultKey(Material());
            byte[] s = Secret();
            if (s != null)
            {
                k.SetSecret(s);
                Array.Clear(s, 0, s.Length);
            }
            return k;
        }

        // Unprotects the key material once for a batch of operations; dispose it as soon as the batch is done.
        internal Cipher Begin()
        {
            byte[] enc = new byte[KeySize], mac = new byte[KeySize];
            lock (_sync)
            {
                if (_disposed) throw new ObjectDisposedException("VaultKey");
                ProtectedMemory.Unprotect(_material, MemoryProtectionScope.SameProcess);
                Buffer.BlockCopy(_material, 0, enc, 0, KeySize);
                Buffer.BlockCopy(_material, KeySize, mac, 0, KeySize);
                ProtectedMemory.Protect(_material, MemoryProtectionScope.SameProcess);
            }
            return new Cipher(enc, mac);
        }

        internal byte[] Mac(byte[] data)
        {
            using (Cipher c = Begin()) return c.Mac(data);
        }

        internal byte[] Mac(byte[] data, int offset, int count)
        {
            using (Cipher c = Begin()) return c.Mac(data, offset, count);
        }

        internal string Encrypt(string plainText)
        {
            using (Cipher c = Begin()) return c.Encrypt(plainText);
        }

        // Throws CryptographicException when the value is malformed or fails authentication.
        internal string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return string.Empty;
            using (Cipher c = Begin()) return c.Decrypt(cipherText);
        }

        // Not thread-safe: one instance per batch, on one thread.
        internal sealed class Cipher : IDisposable
        {
            readonly byte[] _enc, _mac;
            readonly Aes _legacyAes;
            readonly HMACSHA256 _hmac;
            readonly RandomNumberGenerator _rng;
            CngAesGcm _gcm;
            bool _disposed;

            const int GcmNonceSize = 12;
            const int GcmTagSize = 16;
            const int LegacyIvSize = 16;
            const int LegacyMacSize = 32;
            static readonly byte[] FieldAadPrefix = Encoding.ASCII.GetBytes("lain-vault-field-v4|");

            internal Cipher(byte[] enc, byte[] mac)
            {
                _enc = enc;
                _mac = mac;
                _legacyAes = Aes.Create();
                _legacyAes.Mode = CipherMode.CBC;
                _legacyAes.Padding = PaddingMode.PKCS7;
                _hmac = new HMACSHA256(mac);
                _rng = RandomNumberGenerator.Create();
            }

            internal byte[] Mac(byte[] data)
            {
                Check();
                return _hmac.ComputeHash(data);
            }

            internal byte[] Mac(byte[] data, int offset, int count)
            {
                Check();
                return _hmac.ComputeHash(data, offset, count);
            }

            internal string Encrypt(string plainText)
            {
                Check();
                byte[] plain = Encoding.UTF8.GetBytes(plainText ?? string.Empty);
                byte[] nonce = new byte[GcmNonceSize];
                _rng.GetBytes(nonce);
                byte[] aad = GcmFieldAssociatedData();
                byte[] tag = null;
                byte[] ciphertext = null;
                try
                {
                    ciphertext = Gcm().Encrypt(plain, nonce, aad, out tag);
                    byte[] blob = new byte[1 + GcmNonceSize + ciphertext.Length + GcmTagSize];
                    blob[0] = CryLain.GcmFieldVersion;
                    Buffer.BlockCopy(nonce, 0, blob, 1, GcmNonceSize);
                    if (ciphertext.Length > 0) Buffer.BlockCopy(ciphertext, 0, blob, 1 + GcmNonceSize, ciphertext.Length);
                    Buffer.BlockCopy(tag, 0, blob, blob.Length - GcmTagSize, GcmTagSize);
                    return Convert.ToBase64String(blob);
                }
                finally
                {
                    Array.Clear(plain, 0, plain.Length);
                    Array.Clear(nonce, 0, nonce.Length);
                    Array.Clear(aad, 0, aad.Length);
                    if (tag != null) Array.Clear(tag, 0, tag.Length);
                    if (ciphertext != null) Array.Clear(ciphertext, 0, ciphertext.Length);
                }
            }

            // Authenticates a value without retaining plaintext. GCM verification necessarily decrypts into a
            // temporary byte buffer, which is immediately cleared; legacy CBC values are verified by HMAC only.
            internal void Verify(string cipherText, byte expectedFieldVersion)
            {
                if (string.IsNullOrEmpty(cipherText)) return;
                Check();

                byte[] blob = Decode(cipherText);
                if (blob.Length == 0 || blob[0] != expectedFieldVersion)
                    throw new CryptographicException("The vault field does not match its declared encryption suite.");

                if (blob[0] == CryLain.LegacyFieldVersion)
                {
                    AuthenticateLegacy(blob);
                    return;
                }

                if (blob[0] == CryLain.GcmFieldVersion)
                {
                    byte[] plain = DecryptGcm(blob);
                    Array.Clear(plain, 0, plain.Length);
                    return;
                }

                throw new CryptographicException("Unsupported vault field encryption version.");
            }

            internal string Decrypt(string cipherText)
            {
                if (string.IsNullOrEmpty(cipherText)) return string.Empty;
                Check();

                byte[] blob = Decode(cipherText);
                if (blob.Length == 0) throw new CryptographicException("Malformed vault value.");

                if (blob[0] == CryLain.GcmFieldVersion)
                {
                    byte[] plain = DecryptGcm(blob);
                    try { return Encoding.UTF8.GetString(plain); }
                    finally { Array.Clear(plain, 0, plain.Length); }
                }

                if (blob[0] == CryLain.LegacyFieldVersion)
                {
                    AuthenticateLegacy(blob);
                    int cipherLength = blob.Length - 1 - LegacyIvSize - LegacyMacSize;
                    byte[] iv = new byte[LegacyIvSize];
                    Buffer.BlockCopy(blob, 1, iv, 0, LegacyIvSize);
                    try
                    {
                        using (ICryptoTransform transform = _legacyAes.CreateDecryptor(_enc, iv))
                        {
                            byte[] plain = transform.TransformFinalBlock(blob, 1 + LegacyIvSize, cipherLength);
                            try { return Encoding.UTF8.GetString(plain); }
                            finally { Array.Clear(plain, 0, plain.Length); }
                        }
                    }
                    finally
                    {
                        Array.Clear(iv, 0, iv.Length);
                    }
                }

                throw new CryptographicException("Unsupported vault field encryption version.");
            }

            CngAesGcm Gcm()
            {
                Check();
                if (_gcm == null) _gcm = new CngAesGcm(_enc);
                return _gcm;
            }

            static byte[] GcmFieldAssociatedData(byte version = CryLain.GcmFieldVersion)
            {
                byte[] aad = new byte[FieldAadPrefix.Length + 1];
                Buffer.BlockCopy(FieldAadPrefix, 0, aad, 0, FieldAadPrefix.Length);
                aad[aad.Length - 1] = version;
                return aad;
            }

            byte[] DecryptGcm(byte[] blob)
            {
                if (blob.Length < 1 + GcmNonceSize + GcmTagSize || blob[0] != CryLain.GcmFieldVersion)
                    throw new CryptographicException("Malformed vault value.");

                int ciphertextLength = blob.Length - 1 - GcmNonceSize - GcmTagSize;
                byte[] nonce = new byte[GcmNonceSize];
                byte[] ciphertext = new byte[ciphertextLength];
                byte[] tag = new byte[GcmTagSize];
                byte[] aad = GcmFieldAssociatedData(blob[0]);
                Buffer.BlockCopy(blob, 1, nonce, 0, nonce.Length);
                if (ciphertextLength > 0) Buffer.BlockCopy(blob, 1 + GcmNonceSize, ciphertext, 0, ciphertextLength);
                Buffer.BlockCopy(blob, blob.Length - GcmTagSize, tag, 0, tag.Length);
                try { return Gcm().Decrypt(ciphertext, nonce, aad, tag); }
                finally
                {
                    Array.Clear(nonce, 0, nonce.Length);
                    Array.Clear(ciphertext, 0, ciphertext.Length);
                    Array.Clear(tag, 0, tag.Length);
                    Array.Clear(aad, 0, aad.Length);
                }
            }

            void AuthenticateLegacy(byte[] blob)
            {
                int cipherLength = blob.Length - 1 - LegacyIvSize - LegacyMacSize;
                if (cipherLength < 16 || cipherLength % 16 != 0 || blob[0] != CryLain.LegacyFieldVersion)
                    throw new CryptographicException("Malformed legacy vault value.");

                byte[] expected = new byte[LegacyMacSize];
                byte[] actual = null;
                try
                {
                    Buffer.BlockCopy(blob, blob.Length - LegacyMacSize, expected, 0, expected.Length);
                    actual = _hmac.ComputeHash(blob, 0, blob.Length - LegacyMacSize);
                    if (!CryLain.FixedTimeEquals(actual, expected))
                        throw new CryptographicException("Vault value failed authentication.");
                }
                finally
                {
                    Array.Clear(expected, 0, expected.Length);
                    if (actual != null) Array.Clear(actual, 0, actual.Length);
                }
            }

            static byte[] Decode(string cipherText)
            {
                try { return Convert.FromBase64String(cipherText); }
                catch (FormatException) { throw new CryptographicException("Malformed vault value."); }
            }

            void Check()
            {
                if (_disposed) throw new ObjectDisposedException("VaultKey.Cipher");
            }

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                Array.Clear(_enc, 0, _enc.Length);
                Array.Clear(_mac, 0, _mac.Length);
                _legacyAes.Dispose();
                if (_gcm != null) _gcm.Dispose();
                _hmac.Dispose();
                _rng.Dispose();
            }
        }

        public void Dispose()
        {
            lock (_sync)
            {
                if (_disposed) return;
                Array.Clear(_material, 0, _material.Length);
                if (_secret != null) Array.Clear(_secret, 0, _secret.Length);
                _disposed = true;
            }
        }
    }
}
