using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Lain;

namespace Lain.SecurityTests
{
    // Dependency-free regression harness. Run on Windows with .NET Framework 4.8:
    // Lain.SecurityTests.exe
    internal static class Program
    {
        static int _passed;
        static int _failed;

        static int Main()
        {
            Run("accept current LAINKEY2 magic", AcceptsCurrentKeyFileMagic);
            Run("reject legacy LAINKEY1 magic", RejectsLegacyKeyFileMagic);
            Run("reject unknown key-file magic", RejectsUnknownKeyFileMagic);
            Run("require cross-platform authenticators", RequiresCrossPlatformAttachment);
            Run("accept valid ES256 credential metadata", AcceptsValidCredentialMetadata);
            Run("reject credential without public key", RejectsMissingPublicKey);
            Run("reject non-ES256 metadata", RejectsUnexpectedCredentialAlgorithm);
            Run("parse valid ES256 COSE public key", ParsesValidEs256Credential);
            Run("reject non-ES256 COSE key", RejectsNonEs256CoseKey);
            Run("reject mismatched credential ID", RejectsCredentialIdMismatch);
            Run("reject incorrect RP ID hash", RejectsIncorrectRpIdHash);
            Run("reject missing user presence", RejectsMissingUserPresence);
            Run("reject missing user verification", RejectsMissingUserVerification);
            Run("reject reserved authenticator flag bits", RejectsReservedAuthenticatorFlags);
            Run("reject invalid backup-state flag combination", RejectsInvalidBackupStateFlags);
            Run("reject malformed authenticator data", RejectsMalformedAuthenticatorData);
            Run("reject invalid P-256 public key", RejectsInvalidP256PublicKey);
            Run("find only registered credential IDs", RejectsUnregisteredCredentialId);
            Run("verify valid ES256 assertion signature", VerifiesValidAssertionSignature);
            Run("reject modified ES256 assertion signature", RejectsModifiedAssertionSignature);
            Run("reject oversized ES256 assertion signatures", RejectsOversizedAssertionSignature);
            Run("require 15 characters for single-factor passwords", RequiresFifteenCharacterSingleFactorPassword);
            Run("accept long passphrases without composition rules", AcceptsLongPassphraseWithoutCompositionRules);
            Run("allow the 12-character MFA minimum in Both mode", AllowsMfaPasswordMinimum);
            Run("select minimum length from key mode", SelectsMinimumLengthFromKeyMode);
            Run("accept a well-formed Either-mode LAINKEY2 file", AcceptsWellFormedKeyFile);
            Run("accept a well-formed Both-mode LAINKEY2 file", AcceptsWellFormedBothModeKeyFile);
            Run("accept legacy encrypted slots inside LAINKEY2", AcceptsLegacyWrappedSlots);
            Run("reject oversized key-file slot strings", RejectsOversizedKeyFileSlotString);
            Run("reject oversized key files", RejectsOversizedKeyFile);
            Run("reject excessive credential counts", RejectsExcessiveCredentialCount);
            Run("reject malformed UTF-8 in key-file strings", RejectsMalformedKeyFileUtf8);
            Run("reject non-canonical 7-bit string lengths", RejectsNonCanonicalKeyFileStringLength);
            Run("reject mode-inconsistent key slots", RejectsModeInconsistentKeySlots);
            Run("reject recovery slot in Either mode", RejectsRecoverySlotInEitherMode);
            Run("reject duplicate credential IDs", RejectsDuplicateCredentialIds);
            Run("reject oversized credential IDs", RejectsOversizedCredentialIds);
            Run("reject oversized key names", RejectsOversizedKeyNames);
            Run("reject invalid stored P-256 points", RejectsInvalidStoredP256Point);
            Run("reject control characters in key names", RejectsControlCharactersInKeyNames);
            Run("reject trailing key-file bytes", RejectsTrailingKeyFileBytes);
            Run("delete v1 migration backups without deleting split backups", DeletesV1MigrationBackupsOnly);

            Console.WriteLine();
            Console.WriteLine("Passed: {0}; Failed: {1}", _passed, _failed);
            return _failed == 0 ? 0 : 1;
        }

        static void Run(string name, Action test)
        {
            try
            {
                test();
                _passed++;
                Console.WriteLine("PASS  " + name);
            }
            catch (Exception ex)
            {
                _failed++;
                Console.WriteLine("FAIL  " + name + " - " + ex.Message);
            }
        }

        static void DeletesV1MigrationBackupsOnly()
        {
            bool createdDataFolder = !Directory.Exists(Required.DataFolder);
            Directory.CreateDirectory(Required.DataFolder);

            string suffix = Guid.NewGuid().ToString("N");
            string v1DataBackup = Required.LainData + ".v1-test-" + suffix + ".bak";
            string v1SerialBackup = Required.LainSerial + ".v1-test-" + suffix + ".bak";
            string splitBackup = Required.LainData + ".split-test-" + suffix + ".bak";

            try
            {
                File.WriteAllText(v1DataBackup, "legacy data backup");
                File.WriteAllText(v1SerialBackup, "legacy serial backup");
                File.WriteAllText(splitBackup, "split backup");

                True(CryLain.DeleteV1Backups(), "v1 backups were not fully deleted");
                True(!File.Exists(v1DataBackup), "v1 data backup still exists");
                True(!File.Exists(v1SerialBackup), "v1 serial backup still exists");
                True(File.Exists(splitBackup), "split backup was incorrectly deleted");
            }
            finally
            {
                if (File.Exists(v1DataBackup)) File.Delete(v1DataBackup);
                if (File.Exists(v1SerialBackup)) File.Delete(v1SerialBackup);
                if (File.Exists(splitBackup)) File.Delete(splitBackup);
                if (createdDataFolder && Directory.Exists(Required.DataFolder) && Directory.GetFileSystemEntries(Required.DataFolder).Length == 0)
                    Directory.Delete(Required.DataFolder);
            }
        }

        static void AcceptsCurrentKeyFileMagic()
        {
            CryLain.ValidateSecurityKeyFileMagic(Encoding.ASCII.GetBytes("LAINKEY2"));
        }

        static void RejectsLegacyKeyFileMagic()
        {
            InvalidDataException ex = Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileMagic(Encoding.ASCII.GetBytes("LAINKEY1")));
            Contains(ex.Message, "does not migrate legacy security-key files");
        }

        static void RejectsUnknownKeyFileMagic()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileMagic(Encoding.ASCII.GetBytes("LAINKEY9")));
        }

        static void RequiresCrossPlatformAttachment()
        {
            Equal(2, WebAuthn.AuthenticatorAttachmentCrossPlatform, "Windows CROSS_PLATFORM enum value changed");
        }

        static void AcceptsValidCredentialMetadata()
        {
            WebAuthn.Credential credential = NewCredential();
            WebAuthn.ValidateCredentialForAssertion(credential);
        }

        static void RejectsMissingPublicKey()
        {
            WebAuthn.Credential credential = NewCredential();
            credential.PublicKeyX = null;
            credential.PublicKeyY = null;
            credential.Algorithm = 0;
            credential.KeyType = 0;
            credential.Curve = 0;
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateCredentialForAssertion(credential));
        }

        static void RejectsUnexpectedCredentialAlgorithm()
        {
            WebAuthn.Credential credential = NewCredential();
            credential.Algorithm = -257;
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateCredentialForAssertion(credential));
        }

        static void ParsesValidEs256Credential()
        {
            byte[] id = { 0x10, 0x20, 0x30, 0x40 };
            byte[] x, y;
            TestPublicPoint(out x, out y);
            WebAuthn.Credential parsed = WebAuthn.ParseEs256Credential(BuildAttestedData(id, x, y, -7), id, new byte[32]);
            Equal(WebAuthn.CoseAlgEs256, parsed.Algorithm, "COSE alg");
            Equal(WebAuthn.CoseKeyTypeEc2, parsed.KeyType, "COSE kty");
            Equal(WebAuthn.CoseCurveP256, parsed.Curve, "COSE curve");
            True(parsed.HasVerifiablePublicKey, "Parsed credential was not marked verifiable");
            WebAuthn.ValidateEs256PublicKey(parsed.PublicKeyX, parsed.PublicKeyY);
        }

        static void RejectsNonEs256CoseKey()
        {
            byte[] id = { 1, 2, 3 };
            byte[] x, y;
            TestPublicPoint(out x, out y);
            Throws<WebAuthn.WebAuthnException>(() =>
                WebAuthn.ParseEs256Credential(BuildAttestedData(id, x, y, -8), id, new byte[32]));
        }

        static void RejectsCredentialIdMismatch()
        {
            byte[] id = { 1, 2, 3 };
            byte[] wrongId = { 1, 2, 4 };
            byte[] x, y;
            TestPublicPoint(out x, out y);
            Throws<WebAuthn.WebAuthnException>(() =>
                WebAuthn.ParseEs256Credential(BuildAttestedData(id, x, y, -7), wrongId, new byte[32]));
        }

        static void RejectsIncorrectRpIdHash()
        {
            byte[] authData = BuildAssertionAuthenticatorData();
            authData[0] ^= 0x01;
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateAssertionAuthenticatorData(authData));
        }

        static void RejectsMissingUserPresence()
        {
            byte[] authData = BuildAssertionAuthenticatorData();
            authData[32] = 0x04; // UV without UP
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateAssertionAuthenticatorData(authData));
        }

        static void RejectsMissingUserVerification()
        {
            byte[] authData = BuildAssertionAuthenticatorData();
            authData[32] = 0x01; // UP without UV
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateAssertionAuthenticatorData(authData));
        }

        static void RejectsReservedAuthenticatorFlags()
        {
            byte[] authData = BuildAssertionAuthenticatorData();
            authData[32] |= 0x02; // Reserved RFU1 bit must be zero.
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateAssertionAuthenticatorData(authData));
        }

        static void RejectsInvalidBackupStateFlags()
        {
            byte[] authData = BuildAssertionAuthenticatorData();
            authData[32] |= 0x10; // BS set without BE is invalid.
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateAssertionAuthenticatorData(authData));
        }

        static void RejectsMalformedAuthenticatorData()
        {
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateAssertionAuthenticatorData(new byte[36]));
            byte[] withMalformedExtensions = BuildAssertionAuthenticatorData();
            withMalformedExtensions[32] |= 0x80; // ED flag set but extension CBOR is absent
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateAssertionAuthenticatorData(withMalformedExtensions));
        }

        static void RejectsInvalidP256PublicKey()
        {
            Throws<WebAuthn.WebAuthnException>(() => WebAuthn.ValidateEs256PublicKey(new byte[32], new byte[32]));
        }

        static void RejectsUnregisteredCredentialId()
        {
            WebAuthn.Credential registered = NewCredential();
            byte[] unregistered = { 0, 1, 2, 4 };
            True(WebAuthn.FindCredential(new[] { registered }, unregistered) == null, "An unknown credential ID was accepted");
            True(object.ReferenceEquals(registered, WebAuthn.FindCredential(new[] { registered }, registered.CredentialId)), "A registered credential was not found");
        }

        static void VerifiesValidAssertionSignature()
        {
            WebAuthn.Credential credential = NewCredential();
            WebAuthn.VerifyAssertionSignature(BuildAssertionAuthenticatorData(), TestClientData(), TestSignature(), credential);
        }

        static void RejectsModifiedAssertionSignature()
        {
            WebAuthn.Credential credential = NewCredential();
            byte[] signature = TestSignature();
            signature[signature.Length - 1] ^= 0x01;
            Throws<WebAuthn.WebAuthnException>(() =>
                WebAuthn.VerifyAssertionSignature(BuildAssertionAuthenticatorData(), TestClientData(), signature, credential));
        }

        static void RejectsOversizedAssertionSignature()
        {
            WebAuthn.Credential credential = NewCredential();
            Throws<WebAuthn.WebAuthnException>(() =>
                WebAuthn.VerifyAssertionSignature(BuildAssertionAuthenticatorData(), TestClientData(), new byte[73], credential));
        }


        static void RaisesOlderKdfToBaseline()
        {
            Equal(600000, CryLain.TargetKdfIterations(1000), "Older KDF work factor was not raised");
        }

        static void PreservesHigherKdfWorkFactor()
        {
            Equal(900000, CryLain.TargetKdfIterations(900000), "A stronger existing work factor was lowered");
        }

        static void LeavesCurrentKdfBaselineUnchanged()
        {
            True(!CryLain.ShouldUpgradePasswordKdf(600000, true), "The current PBKDF2 baseline should not be rewrapped unnecessarily");
        }

        static void DefersKdfUpgradeWithoutPassword()
        {
            True(!CryLain.ShouldUpgradePasswordKdf(100000, false), "A key-only Either login must not modify the password slot");
        }

        static void UpgradesKdfWhenPasswordIsPresent()
        {
            True(CryLain.ShouldUpgradePasswordKdf(100000, true), "A password-based unlock must upgrade the old work factor");
        }

        static void RequiresFifteenCharacterSingleFactorPassword()
        {
            True(!PasswordStrength.IsAcceptableMasterPassword("CorrectHorse12"), "A 14-character password was accepted for single-factor use");
            True(PasswordStrength.IsAcceptableMasterPassword("CorrectHorse123"), "A strong 15-character password was rejected");
        }

        static void AcceptsLongPassphraseWithoutCompositionRules()
        {
            True(PasswordStrength.IsAcceptableMasterPassword("correct horse battery staple"), "A long lowercase passphrase should not require mixed character classes");
        }

        static void AllowsMfaPasswordMinimum()
        {
            True(PasswordStrength.IsAcceptableMasterPassword("CorrectHorse12", PasswordStrength.MinimumMfaMasterPasswordLength),
                "A strong password meeting the MFA minimum was rejected");
        }

        static void SelectsMinimumLengthFromKeyMode()
        {
            Equal(15, CryLain.GetMasterPasswordMinimumLength(KeyMode.Off), "Password-only minimum");
            Equal(15, CryLain.GetMasterPasswordMinimumLength(KeyMode.Either), "Either-mode minimum");
            Equal(12, CryLain.GetMasterPasswordMinimumLength(KeyMode.Both), "Both-mode minimum");
        }

        static void AcceptsWellFormedKeyFile()
        {
            CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture());
        }

        static void AcceptsWellFormedBothModeKeyFile()
        {
            CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, keyMode: KeyMode.Both));
        }

        static void AcceptsLegacyWrappedSlots()
        {
            CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, legacyEnvelopes: true));
        }

        static void RejectsOversizedKeyFileSlotString()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(new string('A', 257))));
        }

        static void RejectsOversizedKeyFile()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(new byte[128 * 1024 + 1]));
        }

        static void RejectsExcessiveCredentialCount()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, keyCount: CryLain.MaxSecurityKeys + 1)));
        }

        static void RejectsMalformedKeyFileUtf8()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, malformedUtf8: true)));
        }

        static void RejectsNonCanonicalKeyFileStringLength()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, nonCanonicalLength: true)));
        }

        static void RejectsModeInconsistentKeySlots()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, missingKeyPathSlot: true)));
        }

        static void RejectsRecoverySlotInEitherMode()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, unexpectedRecoverySlot: true)));
        }

        static void RejectsDuplicateCredentialIds()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, keyCount: 2)));
        }

        static void RejectsOversizedCredentialIds()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, credentialIdLength: 1024)));
        }

        static void RejectsOversizedKeyNames()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(keyName: new string('a', 201))));
        }

        static void RejectsInvalidStoredP256Point()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, invalidPublicKey: true)));
        }

        static void RejectsControlCharactersInKeyNames()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(keyName: "Primary\0key")));
        }

        static void RejectsTrailingKeyFileBytes()
        {
            Throws<InvalidDataException>(() =>
                CryLain.ValidateSecurityKeyFileStructure(BuildKeyFileFixture(null, trailingData: true)));
        }

        static byte[] BuildKeyFileFixture(string passwordSlotOverride = null, bool malformedUtf8 = false,
            bool nonCanonicalLength = false, bool missingKeyPathSlot = false, bool invalidPublicKey = false,
            bool trailingData = false, bool legacyEnvelopes = false, int keyCount = 1,
            KeyMode keyMode = KeyMode.Either, string keyName = "Primary key",
            bool unexpectedRecoverySlot = false, int credentialIdLength = 4)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms, Encoding.UTF8, true))
                {
                    writer.Write(Encoding.ASCII.GetBytes("LAINKEY2"));
                    writer.Write((byte)keyMode);
                    writer.Write(600000);
                    writer.Write(new byte[16]);
                    if (malformedUtf8)
                    {
                        writer.Write((byte)1);
                        writer.Write((byte)0xFF);
                    }
                    else if (nonCanonicalLength)
                    {
                        writer.Write((byte)0x80);
                        writer.Write((byte)0x00);
                    }
                    else
                    {
                        writer.Write(passwordSlotOverride ?? WrappedEnvelope(VaultKey.MaterialSize, legacyEnvelopes));
                    }
                    writer.Write(missingKeyPathSlot || keyMode == KeyMode.Both
                        ? string.Empty : WrappedEnvelope(VaultKey.MaterialSize, legacyEnvelopes));
                    writer.Write(WrappedEnvelope(32, legacyEnvelopes));
                    writer.Write(keyMode == KeyMode.Both ? 200000 : 0);
                    writer.Write(new byte[16]);
                    writer.Write(keyMode == KeyMode.Both || unexpectedRecoverySlot
                        ? WrappedEnvelope(32, legacyEnvelopes) : string.Empty);
                    writer.Write(keyCount);
                    writer.Write(keyName);
                    writer.Write(credentialIdLength);
                    writer.Write(new byte[Math.Max(0, credentialIdLength)]);
                    writer.Write(new byte[32]);
                    writer.Write(DateTime.UtcNow.Ticks);
                    writer.Write(WrappedEnvelope(32, legacyEnvelopes));
                    writer.Write(WebAuthn.CoseAlgEs256);
                    writer.Write(WebAuthn.CoseKeyTypeEc2);
                    writer.Write(WebAuthn.CoseCurveP256);
                    byte[] x, y;
                    TestPublicPoint(out x, out y);
                    if (invalidPublicKey) { x = new byte[32]; y = new byte[32]; }
                    writer.Write(32);
                    writer.Write(x);
                    writer.Write(32);
                    writer.Write(y);
                    if (trailingData) writer.Write((byte)0x7F);
                    writer.Flush();
                }

                byte[] body = ms.ToArray();
                byte[] file = new byte[body.Length + 32];
                Buffer.BlockCopy(body, 0, file, 0, body.Length);
                return file;
            }
        }

        static string WrappedEnvelope(int secretSize, bool legacy = false)
        {
            int plainLength = ((secretSize + 2) / 3) * 4;
            byte[] blob;
            if (legacy)
            {
                int paddedLength = ((plainLength / 16) + 1) * 16;
                blob = new byte[1 + 16 + paddedLength + 32];
                blob[0] = CryLain.LegacyFieldVersion;
            }
            else
            {
                blob = new byte[1 + 12 + plainLength + 16];
                blob[0] = CryLain.GcmFieldVersion;
            }
            return Convert.ToBase64String(blob);
        }

        static WebAuthn.Credential NewCredential()
        {
            byte[] x, y;
            TestPublicPoint(out x, out y);
            return new WebAuthn.Credential
            {
                CredentialId = new byte[] { 0x10, 0x20, 0x30, 0x40 },
                Salt = new byte[32],
                Algorithm = WebAuthn.CoseAlgEs256,
                KeyType = WebAuthn.CoseKeyTypeEc2,
                Curve = WebAuthn.CoseCurveP256,
                PublicKeyX = x,
                PublicKeyY = y
            };
        }

        static void TestPublicPoint(out byte[] x, out byte[] y)
        {
            x = FromHex("6b17d1f2e12c4247f8bce6e563a440f277037d812deb33a0f4a13945d898c296");
            y = FromHex("4fe342e2fe1a7f9b8ee7eb4a7c0f9e162bce33576b315ececbb6406837bf51f5");
        }

        static byte[] BuildAttestedData(byte[] credentialId, byte[] x, byte[] y, int algorithm)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] rpHash;
                using (SHA256 sha = SHA256.Create()) rpHash = sha.ComputeHash(Encoding.UTF8.GetBytes(WebAuthn.RpId));
                ms.Write(rpHash, 0, rpHash.Length);
                ms.WriteByte(0x45); // UP | UV | AT
                ms.Write(new byte[4], 0, 4); // signature counter
                ms.Write(new byte[16], 0, 16); // AAGUID
                ms.WriteByte((byte)(credentialId.Length >> 8));
                ms.WriteByte((byte)credentialId.Length);
                ms.Write(credentialId, 0, credentialId.Length);
                ms.WriteByte(0xA5); // five-entry COSE map
                ms.WriteByte(0x01); ms.WriteByte(0x02); // kty = EC2
                ms.WriteByte(0x03); ms.WriteByte(algorithm == -7 ? (byte)0x26 : (byte)0x27); // alg = -7 or -8
                ms.WriteByte(0x20); ms.WriteByte(0x01); // crv = P-256
                ms.WriteByte(0x21); ms.WriteByte(0x58); ms.WriteByte(0x20); ms.Write(x, 0, x.Length); // x
                ms.WriteByte(0x22); ms.WriteByte(0x58); ms.WriteByte(0x20); ms.Write(y, 0, y.Length); // y
                Array.Clear(rpHash, 0, rpHash.Length);
                return ms.ToArray();
            }
        }

        static byte[] BuildAssertionAuthenticatorData()
        {
            byte[] data = new byte[37];
            using (SHA256 sha = SHA256.Create())
            {
                byte[] rpHash = sha.ComputeHash(Encoding.UTF8.GetBytes(WebAuthn.RpId));
                Buffer.BlockCopy(rpHash, 0, data, 0, 32);
                Array.Clear(rpHash, 0, rpHash.Length);
            }
            data[32] = 0x05; // UP | UV; AT and ED are not set
            return data;
        }

        static byte[] TestClientData()
        {
            return Convert.FromBase64String("eyJ0eXBlIjoid2ViYXV0aG4uZ2V0IiwiY2hhbGxlbmdlIjoiZEdWemRDMWphR0ZzYkdWdVoyVSIsIm9yaWdpbiI6Imh0dHBzOi8vbGFpbi5sb2NhbCIsImNyb3NzT3JpZ2luIjpmYWxzZX0=");
        }

        // Fixed test-only ECDSA signature over BuildAssertionAuthenticatorData() || SHA256(TestClientData()).
        // Its public key is the standard P-256 generator point above; this key must never be used by the app.
        static byte[] TestSignature()
        {
            return Convert.FromBase64String("MEYCIQDapKBTHV1yuwqKd7pmm6QTfwKl5NISzub5LziuhbQDJQIhAPVCCwYrHeukXxZFwDQCDUYt9t62fgFo7kQ+oDfShA4m");
        }

        static byte[] FromHex(string value)
        {
            byte[] bytes = new byte[value.Length / 2];
            for (int i = 0; i < bytes.Length; i++) bytes[i] = Convert.ToByte(value.Substring(i * 2, 2), 16);
            return bytes;
        }

        static T Throws<T>(Action action) where T : Exception
        {
            try { action(); }
            catch (T ex) { return ex; }
            catch (Exception ex) { throw new Exception("Expected " + typeof(T).Name + ", got " + ex.GetType().Name, ex); }
            throw new Exception("Expected " + typeof(T).Name + " but no exception was thrown.");
        }

        static void Contains(string actual, string expected)
        {
            if (actual == null || actual.IndexOf(expected, StringComparison.Ordinal) < 0)
                throw new Exception("Expected message to contain: " + expected);
        }

        static void Equal<T>(T expected, T actual, string name)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new Exception(name + ": expected " + expected + ", got " + actual);
        }

        static void True(bool value, string message)
        {
            if (!value) throw new Exception(message);
        }
    }
}
