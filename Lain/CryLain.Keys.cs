using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lain
{
    internal enum KeyMode
    {
        Off = 0,
        // the master password or any security key unlocks the vault
        Either = 1,
        // the master password and a security key (or the recovery code) are both needed
        Both = 2
    }

    internal sealed class UnlockFactor
    {
        internal string Password;
        internal WebAuthn.Assertion Key;
        internal string RecoveryCode;
    }

    internal sealed class SecurityKeyInfo
    {
        internal string Name;
        internal byte[] CredentialId;
        internal byte[] Salt;
        internal int Algorithm;
        internal int KeyType;
        internal int Curve;
        internal byte[] PublicKeyX;
        internal byte[] PublicKeyY;
        internal DateTime Added;
    }

    /*
     * Vault format v4 (security keys)
     *
     *  The vault is encrypted with a random 64-byte data key K (Lain.data / Lain.serial as in v4, verifier = HMAC(K)).
     *  A random 32-byte secret S is shared by all security keys. Lain.keys holds the wrapped keys:
     *
     *    "LAINKEY2" | mode | pwIterations | pwSalt | passwordSlot | keyPathSlot | secretSlot
     *               | recIterations | recSalt | recoverySlot | count | keys... | HMAC_K(file)
     *    key        : name | idLength | credentialId | prfSalt[32] | addedTicks | wrappedSecret
     *               | COSE algorithm | key type | curve | xLength | x | yLength | y
     *    Only LAINKEY2 is accepted. LAINKEY1 is rejected because it has no credential public keys to verify.
     *
     *  pwKey        = PBKDF2-SHA256(password, pwSalt, pwIterations)
     *  Either mode  : passwordSlot = Wrap(K, pwKey), keyPathSlot = Wrap(K, HKDF(S, "lain-key-path"))
     *  Both mode    : passwordSlot = Wrap(K, HKDF(pwKey | S, "lain-password-and-key")),
     *                 recoverySlot = Wrap(S, PBKDF2(recovery code, recSalt, recIterations))
     *  secretSlot   = Wrap(S, K) so that S is known after any unlock (to add keys or change the password).
     *  Wrap(x, kek) = a versioned authenticated field envelope under the 64-byte kek; new writes use AES-256-GCM, while v2 CBC+HMAC envelopes remain readable for migration.
     *
     *  prf is the hmac-secret (WebAuthn PRF) output of the authenticator for prfSalt; it never leaves memory.
     *  The mode is bound cryptographically: changing it in the file makes every slot fail to open.
     */
    internal static partial class CryLain
    {
        static readonly byte[] KeysMagicV1 = Encoding.ASCII.GetBytes("LAINKEY1");
        static readonly byte[] KeysMagicV2 = Encoding.ASCII.GetBytes("LAINKEY2");
        const int SecretSize = 32;
        const int PrfSaltSize = 32;
        const int RecoveryIterations = 200000;
        internal const int MaxSecurityKeys = 32;
        const int MaxKeyNameLength = 200;
        const int MaxKeyNameUtf8Length = MaxKeyNameLength * 4;
        const int MaxCredentialIdLength = 1023;
        const int MaxWrappedSlotLength = 256;
        // The maximum valid serialized file is below 68 KiB even with 32 maximum-size names and credential IDs.
        const int MaxKeyFileSize = 128 * 1024;
        static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);
        const int MinIterations = 1000, MaxIterations = 10000000;
        const string RecoveryAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
        const int RecoveryLength = 30;

        sealed class KeyFile
        {
            internal KeyMode Mode;
            internal int PwIterations;
            internal byte[] PwSalt;
            internal string PasswordSlot = string.Empty;
            internal string KeyPathSlot = string.Empty;
            internal string SecretSlot = string.Empty;
            internal int RecIterations;
            internal byte[] RecSalt = new byte[SaltSize];
            internal string RecoverySlot = string.Empty;
            internal List<KeySlot> Keys = new List<KeySlot>();
            internal byte[] Body, Mac;
        }

        sealed class KeySlot
        {
            internal string Name;
            internal byte[] CredentialId;
            internal byte[] Salt;
            internal long Added;
            internal string Wrapped;
            internal int Algorithm;
            internal int KeyType;
            internal int Curve;
            internal byte[] PublicKeyX;
            internal byte[] PublicKeyY;
        }

        #region Public API

        // Unauthenticated read, for choosing what to ask at login; the unlock itself verifies everything.
        internal static KeyMode GetKeyMode()
        {
            byte[] keys = VaultExists() ? LoadFiles().Keys : null;
            return keys == null ? KeyMode.Off : ReadKeyFile(keys).Mode;
        }

        internal static int GetMasterPasswordMinimumLength(KeyMode mode)
        {
            return mode == KeyMode.Both
                ? PasswordStrength.MinimumMfaMasterPasswordLength
                : PasswordStrength.MinimumMasterPasswordLength;
        }

        internal static int GetRequiredMasterPasswordLength()
        {
            return GetMasterPasswordMinimumLength(GetKeyMode());
        }

        // Unauthenticated read, for display and for the credential list sent to the authenticator.
        // Invalid or unsupported key files must surface an error; silently treating them as "keys off" could
        // mislead the user about the configured unlock policy.
        internal static List<SecurityKeyInfo> GetSecurityKeys()
        {
            List<SecurityKeyInfo> list = new List<SecurityKeyInfo>();
            byte[] keys = VaultExists() ? LoadFiles().Keys : null;
            if (keys == null) return list;

            foreach (KeySlot s in ReadKeyFile(keys).Keys)
            {
                list.Add(new SecurityKeyInfo
                {
                    Name = s.Name,
                    CredentialId = (byte[])s.CredentialId.Clone(),
                    Salt = (byte[])s.Salt.Clone(),
                    Algorithm = s.Algorithm,
                    KeyType = s.KeyType,
                    Curve = s.Curve,
                    PublicKeyX = (byte[])s.PublicKeyX.Clone(),
                    PublicKeyY = (byte[])s.PublicKeyY.Clone(),
                    Added = new DateTime(s.Added, DateTimeKind.Utc)
                });
            }
            return list;
        }

        // True when the factor opens this vault (and, with a session, it is the same vault key).
        internal static bool Verify(UnlockFactor factor, VaultKey session)
        {
            VaultFiles files = LoadFiles();
            if (files.Keys == null) return false;
            KeyFile f = ReadKeyFile(files.Keys);
            using (VaultKey k = OpenSlots(f, factor, session, HeaderOf(files)))
            {
                return k != null && (session == null || SameKey(k, session));
            }
        }

        // Generates a fresh recovery code in the display format XXXXX-XXXXX-...
        internal static string NewRecoveryCode()
        {
            byte[] r = RandomBytes(RecoveryLength);
            StringBuilder sb = new StringBuilder(RecoveryLength + RecoveryLength / 5);
            for (int i = 0; i < RecoveryLength; i++)
            {
                if (i > 0 && i % 5 == 0) sb.Append('-');
                sb.Append(RecoveryAlphabet[r[i] & 31]);
            }
            Array.Clear(r, 0, r.Length);
            return sb.ToString();
        }

        // Upper-cases, drops separators and fixes look-alike digits; null when it can't be a recovery code.
        internal static string NormalizeRecoveryCode(string code)
        {
            if (code == null) return null;
            StringBuilder sb = new StringBuilder(RecoveryLength);
            foreach (char ch in code.ToUpperInvariant())
            {
                if (ch == '-' || ch == ' ' || char.IsWhiteSpace(ch)) continue;
                char c = ch == '0' ? 'O' : ch == '1' ? 'I' : ch == '8' ? 'B' : ch;
                if (RecoveryAlphabet.IndexOf(c) < 0) return null;
                sb.Append(c);
            }
            return sb.Length == RecoveryLength ? sb.ToString() : null;
        }

        // Turns security keys on: the vault is re-encrypted under a random data key and Lain.keys is written.
        // credential / prf / prfSalt come from the validated ES256 WebAuthn registration (+ GetSecret when the key
        // cannot evaluate the PRF at registration).
        // recoveryCode is set in Both mode and must be shown to the user once.
        internal static VaultKey EnableSecurityKeys(VaultKey oldKey, List<LainAccount> current, string password, KeyMode mode,
            string name, WebAuthn.Credential credential, byte[] prf, byte[] prfSalt, out string recoveryCode, out List<LainAccount> accounts)
        {
            accounts = null;
            recoveryCode = null;
            if (mode != KeyMode.Either && mode != KeyMode.Both) throw new ArgumentException("Choose a valid security key mode.");
            EnsureAcceptableMasterPassword(password, GetMasterPasswordMinimumLength(mode));
            if (HasKeyFile()) throw new InvalidOperationException("Security keys are already turned on.");
            if (!VerifyPassword(password, oldKey)) throw new InvalidOperationException("The master password is incorrect.");
            CheckEnrollment(name, credential, prf, prfSalt);

            Func<string, Exception> failed = reason => new InvalidDataException("Security keys could not be turned on (" + reason + "). Your vault is unchanged.");
            byte[] k = RandomBytes(VaultKey.MaterialSize);
            byte[] s = RandomBytes(SecretSize);
            VaultKey key = null;
            try
            {
                int passwordIterations = TargetKdfIterations(HeaderOf(LoadFiles()).Iterations);
                VaultHeader header;
                key = NewDataKey(k, out header);
                header.Iterations = passwordIterations;
                key.SetSecret(s);

                KeyFile f = new KeyFile { Mode = mode, PwIterations = passwordIterations, PwSalt = RandomBytes(SaltSize) };
                SetPasswordSlot(f, password, k, s);
                SetModeSlots(f, k, s);
                string code = mode == KeyMode.Both ? SetRecovery(f, s) : null;
                f.Keys.Add(NewSlot(name, credential, prf, prfSalt, s));

                WebAuthn.Assertion assertion = new WebAuthn.Assertion
                {
                    CredentialId = credential.CredentialId,
                    Secret = prf,
                    IsEnrollmentProof = true
                };
                VaultKey result = Rekey(oldKey, current, key, header, nk => BuildKeyFile(f, nk), () =>
                {
                    VaultFiles files = LoadFiles();
                    if (files.Keys == null) return null;
                    KeyFile nf = ReadKeyFile(files.Keys);
                    VaultHeader h = HeaderOf(files);
                    if (mode == KeyMode.Either)
                    {
                        using (VaultKey viaKey = OpenSlots(nf, new UnlockFactor { Key = assertion }, null, h)) if (viaKey == null) return null;
                        return OpenSlots(nf, new UnlockFactor { Password = password }, null, h);
                    }
                    using (VaultKey viaCode = OpenSlots(nf, new UnlockFactor { Password = password, RecoveryCode = code }, null, h)) if (viaCode == null) return null;
                    return OpenSlots(nf, new UnlockFactor { Password = password, Key = assertion }, null, h);
                }, failed, out accounts);
                key = null;
                recoveryCode = code;
                return result;
            }
            finally
            {
                if (key != null) key.Dispose();
                Array.Clear(k, 0, k.Length);
                Array.Clear(s, 0, s.Length);
            }
        }

        // Turns security keys off: the vault goes back to a key derived from the master password only.
        internal static VaultKey DisableSecurityKeys(VaultKey oldKey, List<LainAccount> current, string password, out List<LainAccount> accounts)
        {
            accounts = null;
            EnsureAcceptableMasterPassword(password);
            KeyFile currentKeyFile = AuthenticateKeyFile(oldKey);
            if (!VerifyPassword(password, oldKey)) throw new InvalidOperationException("The master password is incorrect.");
            VaultHeader header;
            VaultKey key = NewKey(password, TargetKdfIterations(currentKeyFile.PwIterations), out header);
            return Rekey(oldKey, current, key, header, null, () => LoadFiles().Keys != null ? null : OpenKey(password, HeaderOf(LoadFiles())),
                reason => new InvalidDataException("Security keys could not be turned off (" + reason + "). Your vault is unchanged."), out accounts);
        }

        internal static void AddSecurityKey(VaultKey session, string password, string name, WebAuthn.Credential credential, byte[] prf, byte[] prfSalt)
        {
            CheckEnrollment(name, credential, prf, prfSalt);
            KeyFile f = ConfirmChange(session, password);
            if (f.Keys.Count >= MaxSecurityKeys) throw new InvalidOperationException("No more than " + MaxSecurityKeys + " security keys can be added.");
            foreach (KeySlot slot in f.Keys)
            {
                if (FixedTimeEquals(slot.CredentialId, credential.CredentialId)) throw new InvalidOperationException("This security key is already added.");
            }

            byte[] s = SessionSecret(session);
            try
            {
                KeySlot added = NewSlot(name, credential, prf, prfSalt, s);
                byte[] check = Unwrap(added.Wrapped, SlotKek(prf, prfSalt));
                bool ok = check != null && FixedTimeEquals(check, s);
                if (check != null) Array.Clear(check, 0, check.Length);
                if (!ok) throw new InvalidDataException("The security key could not be added (it could not be verified).");
                f.Keys.Add(added);
            }
            finally
            {
                Array.Clear(s, 0, s.Length);
            }
            ReplaceKeyFile(f, session, "The security key could not be added");
        }

        internal static void RemoveSecurityKey(VaultKey session, string password, byte[] credentialId)
        {
            KeyFile f = ConfirmChange(session, password);
            int index = f.Keys.FindIndex(slot => FixedTimeEquals(slot.CredentialId, credentialId));
            if (index < 0) throw new InvalidOperationException("This security key is not part of the vault.");
            if (f.Keys.Count == 1) throw new InvalidOperationException("The last security key can't be removed. Turn security keys off instead.");
            f.Keys.RemoveAt(index);
            ReplaceKeyFile(f, session, "The security key could not be removed");
        }

        internal static void RenameSecurityKey(VaultKey session, byte[] credentialId, string name)
        {
            name = NormalizeSecurityKeyName(name);
            KeyFile f = AuthenticateKeyFile(session);
            KeySlot slot = f.Keys.Find(x => FixedTimeEquals(x.CredentialId, credentialId));
            if (slot == null) throw new InvalidOperationException("This security key is not part of the vault.");
            slot.Name = name;
            ReplaceKeyFile(f, session, "The security key could not be renamed");
        }

        // Switches between Either and Both. Returns the new recovery code when switching to Both, otherwise null.
        internal static string SetKeyMode(VaultKey session, string password, KeyMode mode)
        {
            if (mode != KeyMode.Either && mode != KeyMode.Both) throw new ArgumentException("Use DisableSecurityKeys to turn security keys off.");
            KeyFile f = ConfirmChange(session, password);
            if (f.Mode == mode) return null;
            EnsureAcceptableMasterPassword(password, GetMasterPasswordMinimumLength(mode));

            byte[] k = session.Material();
            byte[] s = SessionSecret(session);
            try
            {
                f.Mode = mode;
                SetPasswordSlot(f, password, k, s);
                SetModeSlots(f, k, s);
                string code = mode == KeyMode.Both ? SetRecovery(f, s) : null;
                if (mode == KeyMode.Either) ClearRecovery(f);
                ReplaceKeyFile(f, session, "The security key mode could not be changed", password);
                return code;
            }
            finally
            {
                Array.Clear(k, 0, k.Length);
                Array.Clear(s, 0, s.Length);
            }
        }

        internal static string RegenerateRecoveryCode(VaultKey session, string password)
        {
            KeyFile f = ConfirmChange(session, password);
            if (f.Mode != KeyMode.Both) throw new InvalidOperationException("A recovery code is only used when both the password and a security key are required.");
            byte[] s = SessionSecret(session);
            try
            {
                string code = SetRecovery(f, s);
                ReplaceKeyFile(f, session, "A new recovery code could not be made");
                return code;
            }
            finally
            {
                Array.Clear(s, 0, s.Length);
            }
        }

        #endregion

        #region Unlock

        static VaultKey UnlockKeyed(UnlockFactor factor, VaultFiles files, out List<LainAccount> accounts)
        {
            accounts = null;
            KeyFile f = ReadKeyFile(files.Keys);
            VaultHeader header = HeaderOf(files);

            VaultKey key = OpenSlots(f, factor, null, header);
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

        // Opens the data key with the given factors (S may also come from an unlocked session). Null when the factors
        // are wrong; throws InvalidDataException when the files don't belong together or were modified.
        static VaultKey OpenSlots(KeyFile f, UnlockFactor factor, VaultKey session, VaultHeader header)
        {
            if (factor == null) throw new ArgumentNullException("factor");
            if (factor.Key != null && (factor.Key.Secret == null || factor.Key.CredentialId == null
                || (!factor.Key.SignatureVerified && !factor.Key.IsEnrollmentProof)))
                throw new InvalidDataException("The security-key assertion was not locally verified.");

            byte[] s = null, k = null, kek = null;
            try
            {
                if (factor.Key != null)
                {
                    KeySlot slot = f.Keys.Find(x => FixedTimeEquals(x.CredentialId, factor.Key.CredentialId));
                    if (slot == null) return null;
                    kek = SlotKek(factor.Key.Secret, slot.Salt);
                    s = Unwrap(slot.Wrapped, kek);
                    if (s == null) return null;
                }
                else if (!string.IsNullOrEmpty(factor.RecoveryCode))
                {
                    string code = NormalizeRecoveryCode(factor.RecoveryCode);
                    if (code == null || f.RecoverySlot.Length == 0) return null;
                    kek = DeriveKey(code, f.RecSalt, f.RecIterations);
                    s = Unwrap(f.RecoverySlot, kek);
                    if (s == null) return null;
                }
                else if (session != null)
                {
                    s = session.Secret();
                }

                bool hasPassword = !string.IsNullOrEmpty(factor.Password);
                if (kek != null) Array.Clear(kek, 0, kek.Length);
                if (f.Mode == KeyMode.Either)
                {
                    if (hasPassword) kek = PasswordKek(f, factor.Password, null);
                    else if (s != null && (factor.Key != null || !string.IsNullOrEmpty(factor.RecoveryCode))) kek = KeyPathKek(s);
                    else return null;
                    k = Unwrap(hasPassword ? f.PasswordSlot : f.KeyPathSlot, kek);
                }
                else
                {
                    if (!hasPassword || s == null) return null;
                    kek = PasswordKek(f, factor.Password, s);
                    k = Unwrap(f.PasswordSlot, kek);
                }
                if (k == null) return null;
                if (k.Length != VaultKey.MaterialSize) throw new InvalidDataException("The security key file (Lain.keys) is damaged.");

                VaultKey key = new VaultKey(k);
                try
                {
                    if (!FixedTimeEquals(key.Mac(VerifierLabelFor(header)), header.Verifier))
                        throw new InvalidDataException("The security key file (Lain.keys) does not belong to this vault.");
                    if (!FixedTimeEquals(key.Mac(f.Body), f.Mac))
                        throw new InvalidDataException("The security key file (Lain.keys) failed its integrity check. It may have been tampered with.");

                    if (s == null)
                    {
                        byte[] material = key.Material();
                        try { s = Unwrap(f.SecretSlot, material); }
                        finally { Array.Clear(material, 0, material.Length); }
                        if (s == null || s.Length != SecretSize) throw new InvalidDataException("The security key file (Lain.keys) is damaged.");
                    }
                    key.SetSecret(s);
                    return key;
                }
                catch
                {
                    key.Dispose();
                    throw;
                }
            }
            finally
            {
                if (s != null) Array.Clear(s, 0, s.Length);
                if (k != null) Array.Clear(k, 0, k.Length);
                if (kek != null) Array.Clear(kek, 0, kek.Length);
            }
        }

        // Rewraps legacy CBC+HMAC security-key envelopes as AES-GCM when the corresponding factor is
        // available during unlock. A hardware-key PRF output is deliberately never persisted, so other
        // enrolled authenticators can only be migrated when each one is used successfully.
        static byte[] UpgradeLegacyKeyEnvelopes(byte[] serializedKeyFile, VaultKey session, UnlockFactor factor, VaultHeader header)
        {
            if (serializedKeyFile == null) return null;
            if (factor == null) throw new ArgumentNullException("factor");

            KeyFile f = ReadKeyFile(serializedKeyFile);
            if (!FixedTimeEquals(session.Mac(f.Body), f.Mac))
                throw new InvalidDataException("The security key file (Lain.keys) failed its integrity check. It may have been tampered with.");

            byte[] dataKey = session.Material();
            byte[] secret = session.Secret();
            if (secret == null || secret.Length != SecretSize)
            {
                Array.Clear(dataKey, 0, dataKey.Length);
                if (secret != null) Array.Clear(secret, 0, secret.Length);
                throw new InvalidDataException("The security-key secret is unavailable.");
            }

            bool changed = false;
            try
            {
                // This slot is always recoverable from the already-unlocked data key and security-key secret.
                UpgradeEnvelope(ref f.SecretSlot, secret, dataKey, ref changed);

                // The password-derived wrapping key can only be upgraded when this unlock supplied the
                // master password. In Either mode, a key-only login deliberately defers this until the
                // next password login; Both mode always supplies the password (including recovery login).
                if (ShouldUpgradePasswordKdf(f.PwIterations, !string.IsNullOrEmpty(factor.Password)))
                {
                    f.PwIterations = TargetKdfIterations(f.PwIterations);
                    f.PwSalt = RandomBytes(SaltSize);
                    SetPasswordSlot(f, factor.Password, dataKey, f.Mode == KeyMode.Both ? secret : null);
                    changed = true;
                }

                if (f.Mode == KeyMode.Either)
                {
                    if (!string.IsNullOrEmpty(factor.Password))
                    {
                        byte[] kek = PasswordKek(f, factor.Password, null);
                        try { UpgradeEnvelope(ref f.PasswordSlot, dataKey, kek, ref changed); }
                        finally { Array.Clear(kek, 0, kek.Length); }
                    }

                    byte[] keyPathKek = KeyPathKek(secret);
                    try { UpgradeEnvelope(ref f.KeyPathSlot, dataKey, keyPathKek, ref changed); }
                    finally { Array.Clear(keyPathKek, 0, keyPathKek.Length); }
                }
                else if (!string.IsNullOrEmpty(factor.Password))
                {
                    byte[] passwordKek = PasswordKek(f, factor.Password, secret);
                    try { UpgradeEnvelope(ref f.PasswordSlot, dataKey, passwordKek, ref changed); }
                    finally { Array.Clear(passwordKek, 0, passwordKek.Length); }
                }

                if (!string.IsNullOrEmpty(factor.RecoveryCode) && f.RecoverySlot.Length > 0)
                {
                    string normalizedCode = NormalizeRecoveryCode(factor.RecoveryCode);
                    if (normalizedCode != null)
                    {
                        byte[] recoveryKek = DeriveKey(normalizedCode, f.RecSalt, f.RecIterations);
                        try { UpgradeEnvelope(ref f.RecoverySlot, secret, recoveryKek, ref changed); }
                        finally { Array.Clear(recoveryKek, 0, recoveryKek.Length); }
                    }
                }

                if (factor.Key != null && factor.Key.Secret != null && factor.Key.CredentialId != null)
                {
                    KeySlot slot = f.Keys.Find(x => FixedTimeEquals(x.CredentialId, factor.Key.CredentialId));
                    if (slot != null)
                    {
                        byte[] slotKek = SlotKek(factor.Key.Secret, slot.Salt);
                        try { UpgradeEnvelope(ref slot.Wrapped, secret, slotKek, ref changed); }
                        finally { Array.Clear(slotKek, 0, slotKek.Length); }
                    }
                }

                if (!changed) return serializedKeyFile;

                byte[] upgraded = BuildKeyFile(f, session);
                KeyFile parsed = ReadKeyFile(upgraded);
                if (!FixedTimeEquals(session.Mac(parsed.Body), parsed.Mac))
                    throw new InvalidDataException("The upgraded security-key file could not be authenticated.");

                using (VaultKey reopened = OpenSlots(parsed, factor, null, header))
                {
                    if (reopened == null || !SameKey(reopened, session))
                        throw new InvalidDataException("The upgraded security-key file could not be reopened with the current unlock factor.");
                }

                return upgraded;
            }
            finally
            {
                Array.Clear(dataKey, 0, dataKey.Length);
                Array.Clear(secret, 0, secret.Length);
            }
        }

        static void UpgradeEnvelope(ref string envelope, byte[] value, byte[] kek, ref bool changed)
        {
            if (string.IsNullOrEmpty(envelope)) return;

            byte[] blob;
            try { blob = Convert.FromBase64String(envelope); }
            catch (FormatException) { return; }

            bool isLegacy = blob.Length > 0 && blob[0] == LegacyFieldVersion;
            Array.Clear(blob, 0, blob.Length);
            if (!isLegacy) return;

            envelope = WrapVerified(value, kek);
            changed = true;
        }

        static string WrapVerified(byte[] value, byte[] kek)
        {
            string wrapped = Wrap(value, kek);
            byte[] check = Unwrap(wrapped, kek);
            try
            {
                if (check == null || !FixedTimeEquals(check, value))
                    throw new InvalidDataException("A security-key envelope failed its encryption verification.");
                return wrapped;
            }
            finally
            {
                if (check != null) Array.Clear(check, 0, check.Length);
            }
        }

        #endregion

        #region Password change (v4)

        // Only the password slot changes; the data key and the data file stay the same.
        static VaultKey ChangeKeyedPassword(VaultKey oldKey, List<LainAccount> current, string newPassword, out List<LainAccount> accounts)
        {
            accounts = null;
            KeyFile f = AuthenticateKeyFile(oldKey);
            byte[] k = oldKey.Material();
            byte[] s = SessionSecret(oldKey);
            try
            {
                f.PwIterations = TargetKdfIterations(f.PwIterations);
                f.PwSalt = RandomBytes(SaltSize);
                SetPasswordSlot(f, newPassword, k, s);
                ReplaceKeyFile(f, oldKey, "The master password could not be changed", newPassword);
            }
            finally
            {
                Array.Clear(k, 0, k.Length);
                Array.Clear(s, 0, s.Length);
            }
            accounts = current;
            return oldKey.Clone();
        }

        #endregion

        #region Slots

        static VaultKey NewDataKey(byte[] k, out VaultHeader header)
        {
            VaultKey key = new VaultKey((byte[])k.Clone());
            header = NewCurrentHeader();
            header.Verifier = key.Mac(VerifierLabelFor(header));
            return key;
        }

        static VaultHeader CurrentHeader()
        {
            return HeaderOf(LoadFiles());
        }

        static byte[] PasswordKek(KeyFile f, string password, byte[] s)
        {
            byte[] pwKey = DeriveKey(password, f.PwSalt, f.PwIterations);
            if (s == null) return pwKey;

            byte[] ikm = new byte[pwKey.Length + s.Length];
            Buffer.BlockCopy(pwKey, 0, ikm, 0, pwKey.Length);
            Buffer.BlockCopy(s, 0, ikm, pwKey.Length, s.Length);
            Array.Clear(pwKey, 0, pwKey.Length);
            try { return Hkdf(ikm, null, "lain-password-and-key"); }
            finally { Array.Clear(ikm, 0, ikm.Length); }
        }

        static byte[] KeyPathKek(byte[] s)
        {
            return Hkdf(s, null, "lain-key-path");
        }

        static byte[] SlotKek(byte[] prf, byte[] prfSalt)
        {
            return Hkdf(prf, prfSalt, "lain-key-slot");
        }

        static void SetPasswordSlot(KeyFile f, string password, byte[] k, byte[] s)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentException("The password is empty.");
            byte[] kek = PasswordKek(f, password, f.Mode == KeyMode.Both ? s : null);
            try { f.PasswordSlot = Wrap(k, kek); }
            finally { Array.Clear(kek, 0, kek.Length); }
        }

        static void SetModeSlots(KeyFile f, byte[] k, byte[] s)
        {
            f.SecretSlot = Wrap(s, k);
            if (f.Mode == KeyMode.Either)
            {
                byte[] kek = KeyPathKek(s);
                try { f.KeyPathSlot = Wrap(k, kek); }
                finally { Array.Clear(kek, 0, kek.Length); }
            }
            else
            {
                f.KeyPathSlot = string.Empty;
            }
        }

        static string SetRecovery(KeyFile f, byte[] s)
        {
            string code = NewRecoveryCode();
            f.RecIterations = RecoveryIterations;
            f.RecSalt = RandomBytes(SaltSize);
            byte[] kek = DeriveKey(NormalizeRecoveryCode(code), f.RecSalt, f.RecIterations);
            try { f.RecoverySlot = Wrap(s, kek); }
            finally { Array.Clear(kek, 0, kek.Length); }
            return code;
        }

        static void ClearRecovery(KeyFile f)
        {
            f.RecIterations = 0;
            f.RecSalt = new byte[SaltSize];
            f.RecoverySlot = string.Empty;
        }

        static KeySlot NewSlot(string name, WebAuthn.Credential credential, byte[] prf, byte[] prfSalt, byte[] s)
        {
            byte[] kek = SlotKek(prf, prfSalt);
            try
            {
                return new KeySlot
                {
                    Name = name.Trim(),
                    CredentialId = (byte[])credential.CredentialId.Clone(),
                    Salt = (byte[])prfSalt.Clone(),
                    Added = DateTime.UtcNow.Ticks,
                    Wrapped = Wrap(s, kek),
                    Algorithm = credential.Algorithm,
                    KeyType = credential.KeyType,
                    Curve = credential.Curve,
                    PublicKeyX = (byte[])credential.PublicKeyX.Clone(),
                    PublicKeyY = (byte[])credential.PublicKeyY.Clone()
                };
            }
            finally
            {
                Array.Clear(kek, 0, kek.Length);
            }
        }

        static string NormalizeSecurityKeyName(string name)
        {
            string normalized = (name ?? string.Empty).Trim();
            if (normalized.Length == 0 || normalized.Length > MaxKeyNameLength || ContainsControlCharacters(normalized))
                throw new ArgumentException("Enter a single-line name of up to " + MaxKeyNameLength + " characters for the security key.");

            try
            {
                if (StrictUtf8.GetByteCount(normalized) > MaxKeyNameUtf8Length)
                    throw new ArgumentException("The security-key name is too long when encoded as UTF-8.");
            }
            catch (EncoderFallbackException)
            {
                throw new ArgumentException("The security-key name contains invalid Unicode text.");
            }

            return normalized;
        }

        static void CheckEnrollment(string name, WebAuthn.Credential credential, byte[] prf, byte[] prfSalt)
        {
            NormalizeSecurityKeyName(name);
            if (credential == null || credential.CredentialId == null || credential.CredentialId.Length == 0 || credential.CredentialId.Length > MaxCredentialIdLength)
                throw new ArgumentException("The security key returned an invalid credential.");
            if (!credential.HasVerifiablePublicKey || credential.Algorithm != WebAuthn.CoseAlgEs256)
                throw new ArgumentException("Only ES256 (P-256) security-key credentials are accepted.");
            if (prf == null || prf.Length != WebAuthn.SecretSize) throw new ArgumentException("The security key returned an invalid secret.");
            if (prfSalt == null || prfSalt.Length != PrfSaltSize) throw new ArgumentException("Invalid security key salt.");
            if (credential.Salt == null || !FixedTimeEquals(credential.Salt, prfSalt))
                throw new ArgumentException("The security-key credential and PRF salt do not match.");
        }

        internal static byte[] NewPrfSalt()
        {
            return RandomBytes(PrfSaltSize);
        }

        static byte[] SessionSecret(VaultKey session)
        {
            byte[] s = session.Secret();
            if (s == null || s.Length != SecretSize) throw new InvalidOperationException("Unlock the vault again before changing security keys.");
            return s;
        }

        static bool SameKey(VaultKey a, VaultKey b)
        {
            byte[] x = a.Material(), y = b.Material();
            try { return FixedTimeEquals(x, y); }
            finally
            {
                Array.Clear(x, 0, x.Length);
                Array.Clear(y, 0, y.Length);
            }
        }

        // Reads Lain.keys, checks it belongs to the unlocked vault and confirms the master password.
        static KeyFile ConfirmChange(VaultKey session, string password)
        {
            KeyFile f = AuthenticateKeyFile(session);
            using (VaultKey k = OpenSlots(f, new UnlockFactor { Password = password }, session, CurrentHeader()))
            {
                if (k == null || !SameKey(k, session)) throw new InvalidOperationException("The master password is incorrect.");
            }

            // These management operations have just re-confirmed the master password. Do not leave an
            // Either-mode key-only login as the last opportunity to upgrade: a password-confirmed action
            // is also a safe point to refresh the password slot's work factor.
            if (ShouldUpgradePasswordKdf(f.PwIterations, true))
            {
                byte[] dataKey = session.Material();
                byte[] secret = SessionSecret(session);
                try
                {
                    f.PwIterations = TargetKdfIterations(f.PwIterations);
                    f.PwSalt = RandomBytes(SaltSize);
                    SetPasswordSlot(f, password, dataKey, f.Mode == KeyMode.Both ? secret : null);
                    ReplaceKeyFile(f, session, "The password protection could not be upgraded", password);
                }
                finally
                {
                    Array.Clear(dataKey, 0, dataKey.Length);
                    Array.Clear(secret, 0, secret.Length);
                }
            }

            return f;
        }

        static KeyFile AuthenticateKeyFile(VaultKey session)
        {
            byte[] keys = LoadFiles().Keys;
            if (keys == null) throw new InvalidOperationException("Security keys are not turned on.");
            KeyFile f = ReadKeyFile(keys);
            if (!FixedTimeEquals(session.Mac(f.Body), f.Mac))
                throw new InvalidDataException("The security key file (Lain.keys) failed its integrity check. It may have been tampered with.");
            return f;
        }

        // Builds and checks the new key file in memory, then replaces Lain.keys atomically (restoring it on failure).
        // With a password, the new file must open the same vault key with it.
        static void ReplaceKeyFile(KeyFile f, VaultKey session, string what, string password = null)
        {
            byte[] bytes = BuildKeyFile(f, session);
            KeyFile parsed = ReadKeyFile(bytes);
            if (!FixedTimeEquals(session.Mac(parsed.Body), parsed.Mac)) throw new InvalidDataException(what + " (the new key file could not be verified).");
            if (password != null)
            {
                using (VaultKey k = OpenSlots(parsed, new UnlockFactor { Password = password }, session, CurrentHeader()))
                {
                    if (k == null || !SameKey(k, session)) throw new InvalidDataException(what + " (the new key file could not be verified).");
                }
            }

            VaultFiles files = LoadFiles();
            bool hadVault = File.Exists(Required.LainVault);
            byte[] old = hadVault ? File.ReadAllBytes(Required.LainVault) : null;
            try
            {
                WriteVault(files.Data ?? BuildData(session, CurrentHeader(), new List<LainAccount>()), bytes);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                try
                {
                    if (old != null) WriteAtomic(Required.LainVault, old);
                    else File.Delete(Required.LainVault);
                }
                catch (IOException) { } catch (UnauthorizedAccessException) { }
                throw new InvalidDataException(what + " (" + ex.Message + "). Nothing was changed.");
            }
            WipeSplitLeftovers();
        }

        #endregion

        #region Key file

        static byte[] BuildKeyFile(KeyFile f, VaultKey key)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter w = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    w.Write(KeysMagicV2);
                    w.Write((byte)f.Mode);
                    w.Write(f.PwIterations);
                    w.Write(f.PwSalt);
                    w.Write(f.PasswordSlot ?? string.Empty);
                    w.Write(f.KeyPathSlot ?? string.Empty);
                    w.Write(f.SecretSlot ?? string.Empty);
                    w.Write(f.RecIterations);
                    w.Write(f.RecSalt ?? new byte[SaltSize]);
                    w.Write(f.RecoverySlot ?? string.Empty);
                    w.Write(f.Keys.Count);
                    foreach (KeySlot s in f.Keys)
                    {
                        w.Write(s.Name);
                        w.Write(s.CredentialId.Length);
                        w.Write(s.CredentialId);
                        w.Write(s.Salt);
                        w.Write(s.Added);
                        w.Write(s.Wrapped);
                        w.Write(s.Algorithm);
                        w.Write(s.KeyType);
                        w.Write(s.Curve);
                        w.Write(s.PublicKeyX == null ? 0 : s.PublicKeyX.Length);
                        if (s.PublicKeyX != null) w.Write(s.PublicKeyX);
                        w.Write(s.PublicKeyY == null ? 0 : s.PublicKeyY.Length);
                        if (s.PublicKeyY != null) w.Write(s.PublicKeyY);
                    }
                    w.Flush();
                    w.Write(key.Mac(ms.GetBuffer(), 0, checked((int)ms.Length)));
                }
                return ms.ToArray();
            }
        }

        internal static void ValidateSecurityKeyFileMagic(byte[] magic)
        {
            if (FixedTimeEquals(magic, KeysMagicV2)) return;
            if (FixedTimeEquals(magic, KeysMagicV1))
                throw new InvalidDataException("This Lain.keys file uses the unsupported LAINKEY1 format. This build requires locally verifiable ES256 credentials and does not migrate legacy security-key files. Use the build that created this file to turn security keys off, then configure security keys again with this build. Do not delete Lain.keys manually.");
            throw new InvalidDataException("The security key file (Lain.keys) has an unsupported or damaged format.");
        }

        static KeyFile ReadKeyFile(byte[] file)
        {
            const string damaged = "The security key file (Lain.keys) is damaged.";
            if (file == null || file.Length < KeysMagicV2.Length + 32 || file.Length > MaxKeyFileSize)
                throw new InvalidDataException(damaged);

            KeyFile f = new KeyFile { Body = new byte[file.Length - 32], Mac = new byte[32] };
            Buffer.BlockCopy(file, 0, f.Body, 0, f.Body.Length);
            Buffer.BlockCopy(file, f.Body.Length, f.Mac, 0, f.Mac.Length);

            try
            {
                using (MemoryStream stream = new MemoryStream(f.Body, false))
                using (BinaryReader r = new BinaryReader(stream, StrictUtf8))
                {
                    ValidateSecurityKeyFileMagic(ReadExactBytes(r, KeysMagicV2.Length, KeysMagicV2.Length));

                    byte mode = r.ReadByte();
                    if (mode != (byte)KeyMode.Either && mode != (byte)KeyMode.Both)
                        throw new InvalidDataException(damaged);
                    f.Mode = (KeyMode)mode;

                    f.PwIterations = r.ReadInt32();
                    f.PwSalt = ReadExactBytes(r, SaltSize, SaltSize);
                    f.PasswordSlot = ReadBoundedString(r, MaxWrappedSlotLength);
                    f.KeyPathSlot = ReadBoundedString(r, MaxWrappedSlotLength);
                    f.SecretSlot = ReadBoundedString(r, MaxWrappedSlotLength);
                    f.RecIterations = r.ReadInt32();
                    f.RecSalt = ReadExactBytes(r, SaltSize, SaltSize);
                    f.RecoverySlot = ReadBoundedString(r, MaxWrappedSlotLength);

                    if (f.PwIterations < MinIterations || f.PwIterations > MaxIterations
                        || !IsValidWrappedSlot(f.PasswordSlot, VaultKey.MaterialSize, false)
                        || !IsValidWrappedSlot(f.SecretSlot, SecretSize, false))
                        throw new InvalidDataException(damaged);

                    if (f.Mode == KeyMode.Either)
                    {
                        // Either mode has no recovery-code slot. Reject unexpected data instead of
                        // accepting an unused alternate unlock path in an otherwise valid file.
                        if (!IsValidWrappedSlot(f.KeyPathSlot, VaultKey.MaterialSize, false)
                            || f.RecoverySlot.Length != 0
                            || f.RecIterations != 0)
                            throw new InvalidDataException(damaged);
                    }
                    else
                    {
                        if (f.KeyPathSlot.Length != 0
                            || !IsValidWrappedSlot(f.RecoverySlot, SecretSize, false)
                            || f.RecIterations < MinIterations || f.RecIterations > MaxIterations)
                            throw new InvalidDataException(damaged);
                    }

                    int count = r.ReadInt32();
                    if (count < 1 || count > MaxSecurityKeys)
                        throw new InvalidDataException(damaged);

                    for (int i = 0; i < count; i++)
                    {
                        KeySlot slot = new KeySlot
                        {
                            Name = ReadBoundedString(r, MaxKeyNameUtf8Length)
                        };
                        int idLength = r.ReadInt32();
                        if (slot.Name.Length == 0 || slot.Name.Length > MaxKeyNameLength
                            || string.IsNullOrWhiteSpace(slot.Name) || ContainsControlCharacters(slot.Name)
                            || idLength < 1 || idLength > MaxCredentialIdLength)
                            throw new InvalidDataException(damaged);

                        slot.CredentialId = ReadExactBytes(r, idLength, MaxCredentialIdLength);
                        slot.Salt = ReadExactBytes(r, PrfSaltSize, PrfSaltSize);
                        slot.Added = r.ReadInt64();
                        slot.Wrapped = ReadBoundedString(r, MaxWrappedSlotLength);
                        slot.Algorithm = r.ReadInt32();
                        slot.KeyType = r.ReadInt32();
                        slot.Curve = r.ReadInt32();
                        int xLength = r.ReadInt32();
                        if (xLength != 32) throw new InvalidDataException(damaged);
                        slot.PublicKeyX = ReadExactBytes(r, xLength, 32);
                        int yLength = r.ReadInt32();
                        if (yLength != 32) throw new InvalidDataException(damaged);
                        slot.PublicKeyY = ReadExactBytes(r, yLength, 32);

                        bool es256Metadata = slot.Algorithm == WebAuthn.CoseAlgEs256
                            && slot.KeyType == WebAuthn.CoseKeyTypeEc2
                            && slot.Curve == WebAuthn.CoseCurveP256;
                        if (!es256Metadata)
                            throw new InvalidDataException("The security key file contains a credential without a valid ES256/P-256 public key. This build requires locally verifiable credentials; use the build that created this file to turn security keys off, then configure them again here.");

                        if (!IsValidWrappedSlot(slot.Wrapped, SecretSize, false)
                            || slot.Added <= DateTime.MinValue.Ticks || slot.Added > DateTime.MaxValue.Ticks)
                            throw new InvalidDataException(damaged);

                        WebAuthn.ValidateEs256PublicKey(slot.PublicKeyX, slot.PublicKeyY);

                        if (f.Keys.Exists(existing => FixedTimeEquals(existing.CredentialId, slot.CredentialId)))
                            throw new InvalidDataException(damaged);
                        f.Keys.Add(slot);
                    }

                    if (stream.Position != stream.Length)
                        throw new InvalidDataException(damaged);
                }
                return f;
            }
            catch (EndOfStreamException)
            {
                throw new InvalidDataException(damaged);
            }
            catch (FormatException)
            {
                throw new InvalidDataException(damaged);
            }
            catch (DecoderFallbackException)
            {
                throw new InvalidDataException(damaged);
            }
            catch (WebAuthn.WebAuthnException)
            {
                throw new InvalidDataException(damaged);
            }
        }

        static int ReadBounded7BitEncodedLength(BinaryReader reader, int maximum)
        {
            int value = 0;
            for (int shift = 0; shift < 35; shift += 7)
            {
                byte current = reader.ReadByte();
                int payload = current & 0x7F;
                if (shift == 28 && (payload > 0x07 || (current & 0x80) != 0))
                    throw new InvalidDataException("The security key file (Lain.keys) is damaged.");

                value |= payload << shift;
                if ((current & 0x80) == 0)
                {
                    if ((shift > 0 && payload == 0) || value < 0 || value > maximum)
                        throw new InvalidDataException("The security key file (Lain.keys) is damaged.");
                    return value;
                }
            }
            throw new InvalidDataException("The security key file (Lain.keys) is damaged.");
        }

        static string ReadBoundedString(BinaryReader reader, int maximumBytes)
        {
            int byteCount = ReadBounded7BitEncodedLength(reader, maximumBytes);
            if (byteCount > reader.BaseStream.Length - reader.BaseStream.Position)
                throw new InvalidDataException("The security key file (Lain.keys) is damaged.");

            byte[] bytes = ReadExactBytes(reader, byteCount, maximumBytes);
            try { return StrictUtf8.GetString(bytes); }
            finally { Array.Clear(bytes, 0, bytes.Length); }
        }

        static byte[] ReadExactBytes(BinaryReader reader, int length, int maximum)
        {
            if (length < 0 || length > maximum || length > reader.BaseStream.Length - reader.BaseStream.Position)
                throw new InvalidDataException("The security key file (Lain.keys) is damaged.");
            byte[] bytes = reader.ReadBytes(length);
            if (bytes.Length != length)
                throw new InvalidDataException("The security key file (Lain.keys) is damaged.");
            return bytes;
        }

        static bool ContainsControlCharacters(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsControl(value[i])) return true;
            }
            return false;
        }

        static bool IsValidWrappedSlot(string value, int secretSize, bool allowEmpty)
        {
            if (string.IsNullOrEmpty(value)) return allowEmpty;
            if (value.Length > MaxWrappedSlotLength || (value.Length & 3) != 0
                || (secretSize != SecretSize && secretSize != VaultKey.MaterialSize)) return false;

            byte[] decoded = null;
            try
            {
                decoded = Convert.FromBase64String(value);
                if (!string.Equals(Convert.ToBase64String(decoded), value, StringComparison.Ordinal)) return false;

                // Wrap() encrypts the Base64 representation of the secret. Validate its exact envelope size
                // and version before any KDF or decryption work is attempted.
                int plainLength = ((secretSize + 2) / 3) * 4;
                if (decoded.Length == 1 + 12 + plainLength + 16)
                    return decoded[0] == GcmFieldVersion;

                int paddedLegacyLength = ((plainLength / 16) + 1) * 16;
                int legacyLength = 1 + 16 + paddedLegacyLength + 32;
                return decoded.Length == legacyLength && decoded[0] == LegacyFieldVersion;
            }
            catch (FormatException)
            {
                return false;
            }
            finally
            {
                if (decoded != null) Array.Clear(decoded, 0, decoded.Length);
            }
        }

        // Exposed internally so the Windows regression harness can exercise the real parser with hostile inputs.
        internal static void ValidateSecurityKeyFileStructure(byte[] file)
        {
            ReadKeyFile(file);
        }

        #endregion

        #region Primitives

        // HKDF-SHA256 (RFC 5869) with a 64-byte output.
        static byte[] Hkdf(byte[] ikm, byte[] salt, string info, int length = VaultKey.MaterialSize)
        {
            byte[] prk;
            using (HMACSHA256 h = new HMACSHA256(salt ?? new byte[32])) prk = h.ComputeHash(ikm);

            byte[] infoBytes = Encoding.UTF8.GetBytes(info);
            byte[] okm = new byte[length];
            byte[] t = new byte[0];
            try
            {
                using (HMACSHA256 h = new HMACSHA256(prk))
                {
                    int pos = 0;
                    for (byte i = 1; pos < length; i++)
                    {
                        byte[] input = new byte[t.Length + infoBytes.Length + 1];
                        Buffer.BlockCopy(t, 0, input, 0, t.Length);
                        Buffer.BlockCopy(infoBytes, 0, input, t.Length, infoBytes.Length);
                        input[input.Length - 1] = i;
                        Array.Clear(t, 0, t.Length);
                        t = h.ComputeHash(input);
                        Array.Clear(input, 0, input.Length);
                        int n = Math.Min(t.Length, length - pos);
                        Buffer.BlockCopy(t, 0, okm, pos, n);
                        pos += n;
                    }
                }
                return okm;
            }
            finally
            {
                Array.Clear(prk, 0, prk.Length);
                Array.Clear(t, 0, t.Length);
            }
        }

        static string Wrap(byte[] secret, byte[] kek)
        {
            byte[] enc = new byte[32], mac = new byte[32];
            Buffer.BlockCopy(kek, 0, enc, 0, 32);
            Buffer.BlockCopy(kek, 32, mac, 0, 32);
            using (VaultKey.Cipher c = new VaultKey.Cipher(enc, mac)) return c.Encrypt(Convert.ToBase64String(secret));
        }

        // Null when the slot doesn't open with this key.
        static byte[] Unwrap(string blob, byte[] kek)
        {
            if (string.IsNullOrEmpty(blob)) return null;
            byte[] enc = new byte[32], mac = new byte[32];
            Buffer.BlockCopy(kek, 0, enc, 0, 32);
            Buffer.BlockCopy(kek, 32, mac, 0, 32);
            try
            {
                using (VaultKey.Cipher c = new VaultKey.Cipher(enc, mac)) return Convert.FromBase64String(c.Decrypt(blob));
            }
            catch (CryptographicException) { return null; }
            catch (FormatException) { return null; }
        }

        #endregion
    }
}
