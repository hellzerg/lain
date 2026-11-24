using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lain
{
    /*
     * Lain.vault - one file for the whole vault
     *
     *  "LAINBOX1" | int dataLength | data section | int keysLength | keys section
     *
     *  The data section is the current Lain.data format ("LAINVLT4", HMAC'd with the vault key) and the keys section
     *  is the current Lain.keys format ("LAINKEY2", HMAC'd with the vault key), or empty when security keys are off.
     *  The container adds no cryptography of its own: both sections keep their own integrity checks.
     *
     *  Vaults stored as separate files (Lain.serial + Lain.data [+ Lain.keys]) are packed into Lain.vault the first
     *  time they unlock; the old files are kept as backups for a few unlocks and then wiped.
     */
    internal static partial class CryLain
    {
        static readonly byte[] BoxMagic = Encoding.ASCII.GetBytes("LAINBOX1");
        const int MaxVaultSize = 512 * 1024 * 1024;
        const string SplitTag = ".split-";

        sealed class VaultFiles
        {
            internal byte[] Data;   // null only for a split vault whose data file is missing (an empty vault)
            internal byte[] Keys;   // null when security keys are off
            internal bool Merged;   // true when read from Lain.vault
        }

        // True when there is a vault (of any version) to unlock.
        internal static bool VaultExists()
        {
            return File.Exists(Required.LainVault) || File.Exists(Required.LainSerial);
        }

        static bool HasKeyFile()
        {
            return LoadFiles().Keys != null;
        }

        static VaultFiles LoadFiles()
        {
            if (File.Exists(Required.LainVault))
            {
                FileInfo info = new FileInfo(Required.LainVault);
                if (info.Length > MaxVaultSize) throw new InvalidDataException("The vault file (Lain.vault) is too large to be a Lain vault.");
                return ParseBox(File.ReadAllBytes(Required.LainVault));
            }

            return new VaultFiles
            {
                Data = File.Exists(Required.LainData) ? File.ReadAllBytes(Required.LainData) : null,
                Keys = File.Exists(Required.LainKeys) ? File.ReadAllBytes(Required.LainKeys) : null
            };
        }

        static VaultHeader HeaderOf(VaultFiles files)
        {
            return files.Data != null ? ReadDataHeader(files.Data) : ParseSerial(File.ReadAllText(Required.LainSerial));
        }

        static byte[] BuildBox(byte[] data, byte[] keys)
        {
            if (data == null) throw new ArgumentNullException("data");
            int keysLength = keys == null ? 0 : keys.Length;

            byte[] box = new byte[BoxMagic.Length + 4 + data.Length + 4 + keysLength];
            int at = 0;
            Buffer.BlockCopy(BoxMagic, 0, box, at, BoxMagic.Length); at += BoxMagic.Length;
            WriteInt(box, at, data.Length); at += 4;
            Buffer.BlockCopy(data, 0, box, at, data.Length); at += data.Length;
            WriteInt(box, at, keysLength); at += 4;
            if (keysLength > 0) Buffer.BlockCopy(keys, 0, box, at, keysLength);
            return box;
        }

        static VaultFiles ParseBox(byte[] box)
        {
            const string damaged = "The vault file (Lain.vault) is damaged.";

            int header = BoxMagic.Length + 4;
            if (box.Length < header + 4) throw new InvalidDataException(damaged);
            byte[] magic = new byte[BoxMagic.Length];
            Buffer.BlockCopy(box, 0, magic, 0, magic.Length);
            if (!FixedTimeEquals(magic, BoxMagic)) throw new InvalidDataException("The vault file (Lain.vault) is not in a recognized format.");

            int dataLength = ReadInt(box, BoxMagic.Length);
            if (dataLength <= 0 || dataLength > box.Length - header - 4) throw new InvalidDataException(damaged);
            int keysAt = header + dataLength;
            int keysLength = ReadInt(box, keysAt);
            if (keysLength < 0 || keysLength != box.Length - keysAt - 4) throw new InvalidDataException(damaged);

            VaultFiles files = new VaultFiles { Data = new byte[dataLength], Merged = true };
            Buffer.BlockCopy(box, header, files.Data, 0, dataLength);
            if (keysLength > 0)
            {
                files.Keys = new byte[keysLength];
                Buffer.BlockCopy(box, keysAt + 4, files.Keys, 0, keysLength);
            }
            return files;
        }

        static void WriteInt(byte[] b, int at, int v)
        {
            b[at] = (byte)v; b[at + 1] = (byte)(v >> 8); b[at + 2] = (byte)(v >> 16); b[at + 3] = (byte)(v >> 24);
        }

        static int ReadInt(byte[] b, int at)
        {
            return b[at] | (b[at + 1] << 8) | (b[at + 2] << 16) | (b[at + 3] << 24);
        }

        // Writes Lain.vault atomically and verifies it in bounded memory.
        static void WriteVault(byte[] data, byte[] keys)
        {
            byte[] box = BuildBox(data, keys);
            WriteAtomic(Required.LainVault, box);
            if (!FileMatches(Required.LainVault, box)) throw new IOException("the written vault file does not match");
        }

        static bool FileMatches(string path, byte[] expected)
        {
            using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                if (stream.Length != expected.LongLength) return false;

                byte[] buffer = new byte[64 * 1024];
                int offset = 0;
                int difference = 0;
                try
                {
                    while (offset < expected.Length)
                    {
                        int wanted = Math.Min(buffer.Length, expected.Length - offset);
                        int read = stream.Read(buffer, 0, wanted);
                        if (read <= 0) return false;

                        for (int i = 0; i < read; i++) difference |= buffer[i] ^ expected[offset + i];
                        offset += read;
                    }

                    return difference == 0;
                }
                finally
                {
                    Array.Clear(buffer, 0, buffer.Length);
                }
            }
        }

        // Called after a successful unlock. It upgrades password-derived protection to at least the current
        // PBKDF2 baseline and migrates older data formats to v4. All replacement data is verified before it
        // becomes authoritative. A failed best-effort upgrade leaves the original vault usable.
        static VaultKey CompleteUnlock(VaultKey key, VaultHeader header, VaultFiles files, ref List<LainAccount> accounts, UnlockFactor factor)
        {
            bool hasKeyFile = files.Keys != null;
            bool needsFieldUpgrade = header.FormatVersion != CurrentDataVersion;
            bool passwordAvailable = factor != null && !string.IsNullOrEmpty(factor.Password);

            // In a password-only vault, the data key itself is PBKDF2-derived. Raising the work factor
            // therefore requires a new salt and a new data key, followed by a full account re-encryption.
            // Rekey performs an on-disk backup, validates the new vault by reopening it, and restores the
            // original file if any part of the replacement fails.
            if (!hasKeyFile && ShouldUpgradePasswordKdf(header.Iterations, passwordAvailable))
            {
                VaultKey replacementKey = null;
                try
                {
                    VaultHeader upgradedHeader;
                    replacementKey = NewKey(factor.Password, TargetKdfIterations(header.Iterations), out upgradedHeader);
                    List<LainAccount> rekeyedAccounts;
                    VaultKey upgradedKey = Rekey(key, accounts, replacementKey, upgradedHeader, null,
                        () => OpenKey(factor.Password, HeaderOf(LoadFiles())),
                        reason => new InvalidDataException("The password-based vault KDF could not be upgraded (" + reason + "). The original vault remains available."),
                        out rekeyedAccounts);
                    replacementKey = null; // Rekey transferred ownership to the returned key.
                    VaultKey previousKey = key;
                    key = upgradedKey;
                    accounts = rekeyedAccounts;
                    previousKey.Dispose();
                    NoteSuccessfulUnlock();
                    return key;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is CryptographicException || ex is InvalidDataException)
                {
                    if (replacementKey != null) replacementKey.Dispose();
                    NoteSuccessfulUnlock();
                    return key;
                }
            }

            // For keyed vaults the data key is random; the password KDF lives in LAINKEY2. The data-header
            // iteration field is metadata in this mode, so it can be brought up to the same baseline without
            // changing the data key. The actual password slot is upgraded below only when the password was
            // supplied. In Either mode a security-key-only login defers that part until a password login.
            bool needsHeaderUpgrade = hasKeyFile && header.Iterations < Iterations;
            bool needsDataRewrite = needsFieldUpgrade || needsHeaderUpgrade;
            bool hadVault = File.Exists(Required.LainVault);
            VaultHeader writeHeader = needsDataRewrite ? CreateCurrentHeader(key, header, hasKeyFile) : header;
            List<LainAccount> upgradedAccounts = null;
            byte[] upgradedKeys = files.Keys;
            string backup = null;

            try
            {
                if (hasKeyFile)
                    upgradedKeys = UpgradeLegacyKeyEnvelopes(files.Keys, key, factor, writeHeader);

                bool needsKeyUpgrade = hasKeyFile && !object.ReferenceEquals(upgradedKeys, files.Keys);
                bool needsWrite = !files.Merged || needsDataRewrite || needsKeyUpgrade;
                if (needsWrite)
                {
                    byte[] data;
                    if (needsDataRewrite)
                    {
                        // Only older data versions need their account fields re-encrypted. A v4 header-only
                        // work-factor metadata update leaves the already-authenticated GCM fields intact.
                        upgradedAccounts = needsFieldUpgrade ? ReencryptAccounts(key, accounts) : accounts;
                        data = BuildData(key, writeHeader, upgradedAccounts);
                        ReadData(key, data);
                    }
                    else
                    {
                        data = files.Data ?? BuildData(key, header, accounts);
                    }

                    backup = BackupVaultFile(reason => new InvalidDataException("The vault upgrade could not be backed up (" + reason + ")."));
                    WriteVault(data, upgradedKeys);
                    if (!object.ReferenceEquals(upgradedAccounts, accounts) && upgradedAccounts != null)
                        ReplaceAccounts(accounts, upgradedAccounts);
                    if (backup != null)
                    {
                        WipeFile(backup);
                        backup = null;
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is CryptographicException || ex is InvalidDataException)
            {
                RestoreVaultFile(backup, hadVault);
                if (!hadVault)
                {
                    try { File.Delete(Required.LainVault); } catch { }
                }
                NoteSuccessfulUnlock();
                return key;
            }

            RetireSplitFiles();
            NoteSuccessfulUnlock();
            return key;
        }

        // Moves the pre-Lain.vault files aside as backups (removed by the usual retention) once Lain.vault exists.
        static void RetireSplitFiles()
        {
            if (!File.Exists(Required.LainVault)) return;

            string stamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            bool moved = false;
            foreach (string file in new[] { Required.LainData, Required.LainSerial, Required.LainKeys })
            {
                if (!File.Exists(file)) continue;
                try
                {
                    File.Move(file, file + SplitTag + stamp + ".bak");
                    moved = true;
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { }
            }
            DeleteSaltFile();

            if (moved)
            {
                Options.CurrentOptions.LegacyBackupUnlocks = 0;
                TrySaveSettings();
            }
        }

        // Lain.salt only ever applied to v1 vaults.
        static void DeleteSaltFile()
        {
            if (!File.Exists(Required.LainSalt)) return;
            try { File.Delete(Required.LainSalt); }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException) { }
        }

        // After the vault was re-encrypted the old split files (and their backups) open with the old factors; wipe them.
        static void WipeSplitLeftovers()
        {
            foreach (string file in new[] { Required.LainData, Required.LainSerial, Required.LainKeys })
            {
                if (File.Exists(file)) WipeFile(file);
            }
            foreach (string b in FindBackups(SplitTag)) WipeFile(b);
        }
    }
}
