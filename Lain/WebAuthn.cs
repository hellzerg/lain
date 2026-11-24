using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Lain
{
    // Minimal wrapper over the Windows WebAuthn API (webauthn.dll) for FIDO2 security keys with the hmac-secret (PRF)
    // extension. Credentials are non-discoverable: the credential IDs are kept in the vault's key file.
    internal static class WebAuthn
    {
        internal const string RpId = "lain.local";
        const string Origin = "https://" + RpId;
        const int MinApiVersion = 4;
        const int TimeoutMs = 120000;
        internal const int SecretSize = 32;

        internal const int CoseAlgEs256 = -7;
        internal const int CoseKeyTypeEc2 = 2;
        internal const int CoseCurveP256 = 1;
        internal const int AuthenticatorAttachmentCrossPlatform = 2;

        internal sealed class Credential
        {
            internal byte[] CredentialId;
            internal byte[] Salt;
            internal int Algorithm;
            internal int KeyType;
            internal int Curve;
            internal byte[] PublicKeyX;
            internal byte[] PublicKeyY;

            internal bool HasVerifiablePublicKey
            {
                get
                {
                    return Algorithm == CoseAlgEs256 && KeyType == CoseKeyTypeEc2 && Curve == CoseCurveP256
                        && PublicKeyX != null && PublicKeyX.Length == 32
                        && PublicKeyY != null && PublicKeyY.Length == 32;
                }
            }
        }

        internal sealed class Assertion
        {
            internal byte[] CredentialId;
            internal byte[] Secret;
            internal bool SignatureVerified;
            // Used only during the initial enrollment transaction to test the new slot with the PRF value
            // returned by the just-completed WebAuthn registration ceremony.
            internal bool IsEnrollmentProof;
        }

        internal sealed class WebAuthnException : Exception
        {
            internal readonly bool Cancelled;

            internal WebAuthnException(string message, bool cancelled = false) : base(message)
            {
                Cancelled = cancelled;
            }
        }

        static int? _apiVersion;

        internal static int ApiVersion
        {
            get
            {
                if (_apiVersion == null)
                {
                    try { _apiVersion = (int)WebAuthNGetApiVersionNumber(); }
                    catch (Exception ex) when (ex is DllNotFoundException || ex is EntryPointNotFoundException) { _apiVersion = 0; }
                }
                return _apiVersion.Value;
            }
        }

        internal static bool IsAvailable { get { return ApiVersion >= MinApiVersion; } }

        internal static string UnavailableReason
        {
            get
            {
                return ApiVersion == 0
                    ? "Security keys need the Windows WebAuthn API, which isn't available on this version of Windows."
                    : "Security keys need a newer Windows WebAuthn API (Windows 10 22H2 or Windows 11).";
            }
        }

        // WebAuthn API 8 can evaluate the PRF while the credential is created (CTAP 2.2 hmac-secret-mc), saving a second prompt.
        internal static bool CanReadSecretOnRegister { get { return ApiVersion >= PrfOnCreateApiVersion; } }
        const int PrfOnCreateApiVersion = 8;

        // Registers a new ES256/P-256 credential (touch + user verification) and returns its validated COSE public key.
        // When Windows and the key can evaluate the PRF at creation, 'secret' receives the 32-byte secret for 'salt'
        // (identical to what GetSecret returns later); otherwise it's null and the caller asks with GetSecret.
        internal static Credential Register(IntPtr hwnd, string displayName, IList<byte[]> exclude, byte[] salt, out byte[] secret)
        {
            Require();
            secret = null;
            bool prfOnCreate = salt != null && CanReadSecretOnRegister;
            using (Native n = new Native())
            {
                RpInfo rp = new RpInfo { dwVersion = 1, pwszId = n.Str(RpId), pwszName = n.Str("Lain") };
                byte[] userId = RandomBytes(16);
                UserInfo user = new UserInfo
                {
                    dwVersion = 1, cbId = userId.Length, pbId = n.Bytes(userId),
                    pwszName = n.Str(displayName), pwszDisplayName = n.Str(displayName)
                };

                // Registration policy: ES256 (ECDSA P-256 / SHA-256) only.
                CoseParam[] algs =
                {
                    new CoseParam { dwVersion = 1, pwszCredentialType = n.Str("public-key"), lAlg = CoseAlgEs256 },
                };
                CoseParams cose = new CoseParams { cCredentialParameters = algs.Length, pCredentialParameters = n.Array(algs) };

                ClientData cd = n.ClientData("webauthn.create");

                MakeOptions o = new MakeOptions
                {
                    dwVersion = 4,
                    dwTimeoutMilliseconds = TimeoutMs,
                    CredentialList = n.Credentials(exclude),
                    dwAuthenticatorAttachment = AuthenticatorAttachmentCrossPlatform,
                    bRequireResidentKey = 0,
                    dwUserVerificationRequirement = 1,
                    dwAttestationConveyancePreference = 1,
                };
                if (prfOnCreate)
                {
                    // PRF input semantics (no HMAC_SECRET_VALUES_FLAG), the same as GetSecret uses
                    o.dwVersion = 8;
                    o.bEnablePrf = 1;
                    o.pPRFGlobalEval = n.Struct(new SecretSalt { cbFirst = salt.Length, pbFirst = n.Bytes(salt) });
                }
                else
                {
                    IntPtr enable = n.Bytes(BitConverter.GetBytes(1));
                    Extension[] ext = { new Extension { pwszExtensionIdentifier = n.Str("hmac-secret"), cbExtension = 4, pvExtension = enable } };
                    o.Extensions = new Extensions { cExtensions = 1, pExtensions = n.Array(ext) };
                }

                IntPtr result;
                Check(WebAuthNAuthenticatorMakeCredential(hwnd, ref rp, ref user, ref cose, ref cd, ref o, out result));
                if (result == IntPtr.Zero) throw new WebAuthnException("Windows returned an empty security-key registration response.");
                try
                {
                    Attestation a = (Attestation)Marshal.PtrToStructure(result, typeof(Attestation));
                    if (a.cbCredentialId <= 0 || a.cbCredentialId > 1023 || a.pbCredentialId == IntPtr.Zero)
                        throw new WebAuthnException("The security key didn't return a valid credential ID.");

                    bool hmac = false;
                    if (a.dwVersion >= 2)
                    {
                        if (a.Extensions.cExtensions < 0 || a.Extensions.cExtensions > 64
                            || (a.Extensions.cExtensions > 0 && a.Extensions.pExtensions == IntPtr.Zero))
                            throw new WebAuthnException("The security key returned invalid extension data.");
                        int size = Marshal.SizeOf(typeof(Extension));
                        for (int i = 0; i < a.Extensions.cExtensions; i++)
                        {
                            Extension e = (Extension)Marshal.PtrToStructure(a.Extensions.pExtensions + i * size, typeof(Extension));
                            if (Marshal.PtrToStringUni(e.pwszExtensionIdentifier) == "hmac-secret" && e.cbExtension >= 4 && e.pvExtension != IntPtr.Zero)
                                hmac = Marshal.ReadInt32(e.pvExtension) != 0;
                        }
                    }
                    if (a.dwVersion >= 5 && Marshal.ReadInt32(result, AttestationOffset("bPrfEnabled")) != 0) hmac = true;
                    if (!hmac) throw new WebAuthnException("This authenticator doesn't support the hmac-secret extension, so it can't protect the vault. Most FIDO2 security keys (YubiKey 5, SoloKey, Nitrokey 3, Feitian...) do.");

                    if (a.cbAuthenticatorData < 55 || a.cbAuthenticatorData > MaxAuthenticatorDataSize || a.pbAuthenticatorData == IntPtr.Zero)
                        throw new WebAuthnException("The security key returned invalid authenticator data.");

                    byte[] authenticatorData = new byte[a.cbAuthenticatorData];
                    Marshal.Copy(a.pbAuthenticatorData, authenticatorData, 0, authenticatorData.Length);
                    byte[] id = new byte[a.cbCredentialId];
                    Marshal.Copy(a.pbCredentialId, id, 0, id.Length);

                    Credential credential = ParseEs256Credential(authenticatorData, id, salt);
                    ValidateEs256PublicKey(credential.PublicKeyX, credential.PublicKeyY);

                    if (prfOnCreate && a.dwVersion >= 7)
                    {
                        IntPtr p = Marshal.ReadIntPtr(result, AttestationOffset("pHmacSecret"));
                        if (p != IntPtr.Zero)
                        {
                            SecretSalt s = (SecretSalt)Marshal.PtrToStructure(p, typeof(SecretSalt));
                            if (s.cbFirst == SecretSize && s.pbFirst != IntPtr.Zero)
                            {
                                secret = new byte[SecretSize];
                                Marshal.Copy(s.pbFirst, secret, 0, SecretSize);
                            }
                        }
                    }
                    return credential;
                }
                finally
                {
                    WebAuthNFreeCredentialAttestation(result);
                }
            }
        }

        // Asks for any of the given credentials (touch + PIN) and returns which one answered with its 32-byte secret
        // for that credential's salt. Blocks until the Windows prompt closes.
        internal static Assertion GetSecret(IntPtr hwnd, IList<Credential> credentials)
        {
            Require();
            if (credentials == null || credentials.Count == 0 || credentials.Count > 32)
                throw new ArgumentException("No security-key credentials were supplied.", "credentials");

            List<byte[]> credentialIds = new List<byte[]>(credentials.Count);
            SecretSalt[] saltStructs;
            foreach (Credential credential in credentials)
            {
                ValidateCredentialForAssertion(credential);
                credentialIds.Add(credential.CredentialId);
            }

            using (Native n = new Native())
            {
                byte[] clientDataJson;
                ClientData cd = n.ClientData("webauthn.get", out clientDataJson);

                saltStructs = new SecretSalt[credentials.Count];
                for (int i = 0; i < credentials.Count; i++)
                    saltStructs[i] = new SecretSalt { cbFirst = credentials[i].Salt.Length, pbFirst = n.Bytes(credentials[i].Salt) };

                CredSalt[] credSalts = new CredSalt[credentials.Count];
                for (int i = 0; i < credentials.Count; i++)
                {
                    credSalts[i] = new CredSalt
                    {
                        cbCredID = credentials[i].CredentialId.Length, pbCredID = n.Bytes(credentials[i].CredentialId),
                        pHmacSecretSalt = n.Struct(saltStructs[i])
                    };
                }
                SaltValues values = new SaltValues { cCredWithHmacSecretSaltList = credSalts.Length, pCredWithHmacSecretSaltList = n.Array(credSalts) };

                GetOptions o = new GetOptions
                {
                    dwVersion = 6,
                    dwTimeoutMilliseconds = TimeoutMs,
                    CredentialList = n.Credentials(credentialIds),
                    dwAuthenticatorAttachment = AuthenticatorAttachmentCrossPlatform,
                    dwUserVerificationRequirement = 1,
                    pHmacSecretSaltValues = n.Struct(values),
                };

                IntPtr result;
                Check(WebAuthNAuthenticatorGetAssertion(hwnd, RpId, ref cd, ref o, out result));
                if (result == IntPtr.Zero) throw new WebAuthnException("Windows returned an empty security-key assertion.");
                try
                {
                    AssertionNative a = (AssertionNative)Marshal.PtrToStructure(result, typeof(AssertionNative));
                    if (a.dwVersion < 3 || a.pHmacSecret == IntPtr.Zero)
                        throw new WebAuthnException("The security key didn't return its secret. It may not support the hmac-secret extension.");
                    if (a.cbAuthenticatorData < 37 || a.cbAuthenticatorData > MaxAuthenticatorDataSize || a.pbAuthenticatorData == IntPtr.Zero)
                        throw new WebAuthnException("The security key returned invalid authenticator data.");
                    // ES256 signatures are DER-encoded ECDSA signatures and are bounded before parsing.
                    if (a.cbSignature < 1 || a.cbSignature > MaxEs256SignatureSize || a.pbSignature == IntPtr.Zero)
                        throw new WebAuthnException("The security key returned an invalid assertion signature.");

                    SecretSalt s = (SecretSalt)Marshal.PtrToStructure(a.pHmacSecret, typeof(SecretSalt));
                    if (s.cbFirst != SecretSize || s.pbFirst == IntPtr.Zero)
                        throw new WebAuthnException("The security key returned an unexpected secret.");
                    if (a.Credential.cbId <= 0 || a.Credential.cbId > 1023 || a.Credential.pbId == IntPtr.Zero
                        || Marshal.PtrToStringUni(a.Credential.pwszCredentialType) != "public-key")
                        throw new WebAuthnException("The security key didn't identify a valid public-key credential.");

                    byte[] returnedId = new byte[a.Credential.cbId];
                    byte[] authenticatorData = new byte[a.cbAuthenticatorData];
                    byte[] signature = new byte[a.cbSignature];
                    byte[] secret = new byte[SecretSize];
                    try
                    {
                        Marshal.Copy(a.Credential.pbId, returnedId, 0, returnedId.Length);
                        Marshal.Copy(a.pbAuthenticatorData, authenticatorData, 0, authenticatorData.Length);
                        Marshal.Copy(a.pbSignature, signature, 0, signature.Length);
                        Marshal.Copy(s.pbFirst, secret, 0, secret.Length);

                        Credential selected = FindCredential(credentials, returnedId);
                        if (selected == null) throw new WebAuthnException("The security key returned a credential that wasn't requested.");
                        if (!selected.HasVerifiablePublicKey)
                            throw new WebAuthnException("This security-key credential has no locally verifiable ES256 public key. Remove it and register it again.");

                        ValidateAssertionAuthenticatorData(authenticatorData);
                        VerifyAssertionSignature(authenticatorData, clientDataJson, signature, selected);

                        return new Assertion
                        {
                            CredentialId = returnedId,
                            Secret = secret,
                            SignatureVerified = true
                        };
                    }
                    catch
                    {
                        Array.Clear(secret, 0, secret.Length);
                        throw;
                    }
                    finally
                    {
                        Array.Clear(signature, 0, signature.Length);
                        Array.Clear(authenticatorData, 0, authenticatorData.Length);
                    }
                }
                finally
                {
                    WebAuthNFreeAssertion(result);
                    Array.Clear(clientDataJson, 0, clientDataJson.Length);
                }
            }
        }

        const int MaxAuthenticatorDataSize = 8192;
        const int MaxEs256SignatureSize = 72; // Maximum DER-encoded ECDSA P-256 signature length.
        const byte FlagUserPresent = 0x01;
        const byte FlagUserVerified = 0x04;
        const byte FlagBackupEligible = 0x08;
        const byte FlagBackupState = 0x10;
        const byte FlagAttestedCredentialData = 0x40;
        const byte FlagExtensionData = 0x80;
        const byte FlagReservedBits = 0x22; // RFU1 (bit 1) and RFU2 (bit 5).

        internal static Credential FindCredential(IList<Credential> credentials, byte[] credentialId)
        {
            foreach (Credential credential in credentials)
                if (CryLain.FixedTimeEquals(credential.CredentialId, credentialId)) return credential;
            return null;
        }

        internal static void ValidateCredentialForAssertion(Credential credential)
        {
            if (credential == null || credential.CredentialId == null || credential.CredentialId.Length < 1
                || credential.CredentialId.Length > 1023 || credential.Salt == null || credential.Salt.Length != SecretSize)
                throw new WebAuthnException("A security-key credential or PRF salt is invalid.");

            if (!credential.HasVerifiablePublicKey)
                throw new WebAuthnException("This security-key credential has no locally verifiable ES256 public key. Remove it and register it again.");
        }

        internal static Credential ParseEs256Credential(byte[] authenticatorData, byte[] credentialId, byte[] salt)
        {
            ValidateAuthenticatorDataHeader(authenticatorData, requireAttested: true);
            int offset = 37 + 16;
            if (offset + 2 > authenticatorData.Length) throw new WebAuthnException("The security key returned truncated attested credential data.");

            int idLength = (authenticatorData[offset] << 8) | authenticatorData[offset + 1];
            offset += 2;
            if (idLength < 1 || idLength > 1023 || offset + idLength > authenticatorData.Length)
                throw new WebAuthnException("The security key returned an invalid credential ID.");

            byte[] embeddedId = new byte[idLength];
            Buffer.BlockCopy(authenticatorData, offset, embeddedId, 0, idLength);
            offset += idLength;
            if (!CryLain.FixedTimeEquals(embeddedId, credentialId))
                throw new WebAuthnException("The credential ID did not match the attested authenticator data.");

            int mapMajor;
            ulong mapCount;
            ReadCborHead(authenticatorData, ref offset, out mapMajor, out mapCount);
            if (mapMajor != 5 || mapCount != 5)
                throw new WebAuthnException("The security key returned a credential public key outside the ES256 policy.");

            int[] expectedLabels = { 1, 3, -1, -2, -3 };
            int kty = 0, alg = 0, curve = 0;
            byte[] x = null, y = null;
            for (int i = 0; i < expectedLabels.Length; i++)
            {
                int label = ReadCborInteger(authenticatorData, ref offset);
                if (label != expectedLabels[i])
                    throw new WebAuthnException("The credential public key contains unsupported or non-canonical COSE parameters.");
                switch (label)
                {
                    case 1:
                        kty = ReadCborInteger(authenticatorData, ref offset);
                        break;
                    case 3:
                        alg = ReadCborInteger(authenticatorData, ref offset);
                        break;
                    case -1:
                        curve = ReadCborInteger(authenticatorData, ref offset);
                        break;
                    case -2:
                        x = ReadCborByteString(authenticatorData, ref offset, 32);
                        break;
                    case -3:
                        y = ReadCborByteString(authenticatorData, ref offset, 32);
                        break;
                    default:
                        throw new WebAuthnException("The credential public key contains an unsupported COSE parameter.");
                }
            }

            if (kty != CoseKeyTypeEc2 || alg != CoseAlgEs256 || curve != CoseCurveP256)
                throw new WebAuthnException("The security key did not create an ES256 (P-256) credential.");

            ValidateAuthenticatorDataExtensions(authenticatorData, offset);
            return new Credential
            {
                CredentialId = (byte[])credentialId.Clone(),
                Salt = salt == null ? null : (byte[])salt.Clone(),
                Algorithm = alg,
                KeyType = kty,
                Curve = curve,
                PublicKeyX = x,
                PublicKeyY = y
            };
        }

        static void ValidateAuthenticatorDataHeader(byte[] authenticatorData, bool requireAttested)
        {
            if (authenticatorData == null || authenticatorData.Length < 37 || authenticatorData.Length > MaxAuthenticatorDataSize)
                throw new WebAuthnException("The security key returned invalid authenticator data.");

            byte[] expectedRpIdHash;
            using (SHA256 sha = SHA256.Create()) expectedRpIdHash = sha.ComputeHash(Encoding.UTF8.GetBytes(RpId));
            byte[] actualRpIdHash = new byte[32];
            Buffer.BlockCopy(authenticatorData, 0, actualRpIdHash, 0, actualRpIdHash.Length);
            bool rpMatches = CryLain.FixedTimeEquals(expectedRpIdHash, actualRpIdHash);
            Array.Clear(expectedRpIdHash, 0, expectedRpIdHash.Length);
            Array.Clear(actualRpIdHash, 0, actualRpIdHash.Length);
            if (!rpMatches) throw new WebAuthnException("The security key response belongs to a different relying party.");

            byte flags = authenticatorData[32];
            if ((flags & FlagReservedBits) != 0)
                throw new WebAuthnException("The security key returned authenticator data with reserved flag bits set.");
            if ((flags & FlagBackupState) != 0 && (flags & FlagBackupEligible) == 0)
                throw new WebAuthnException("The security key returned an invalid backup-state flag combination.");
            if ((flags & (FlagUserPresent | FlagUserVerified)) != (FlagUserPresent | FlagUserVerified))
                throw new WebAuthnException("The security key did not confirm user presence and verification.");
            if (requireAttested && (flags & FlagAttestedCredentialData) == 0)
                throw new WebAuthnException("The security key did not return attested credential data.");
            if (!requireAttested && (flags & FlagAttestedCredentialData) != 0)
                throw new WebAuthnException("The security key returned unexpected attested data during authentication.");
        }

        static void ValidateAuthenticatorDataExtensions(byte[] authenticatorData, int offset)
        {
            bool hasExtensions = (authenticatorData[32] & FlagExtensionData) != 0;
            if (!hasExtensions)
            {
                if (offset != authenticatorData.Length)
                    throw new WebAuthnException("The security key returned unexpected trailing authenticator data.");
                return;
            }

            SkipCborValue(authenticatorData, ref offset, 0);
            if (offset != authenticatorData.Length)
                throw new WebAuthnException("The security key returned malformed authenticator extensions.");
        }

        internal static void ValidateAssertionAuthenticatorData(byte[] authenticatorData)
        {
            ValidateAuthenticatorDataHeader(authenticatorData, requireAttested: false);
            int offset = 37;
            if ((authenticatorData[32] & FlagExtensionData) != 0)
            {
                SkipCborValue(authenticatorData, ref offset, 0);
                if (offset != authenticatorData.Length)
                    throw new WebAuthnException("The security key returned malformed authenticator extensions.");
            }
            else if (offset != authenticatorData.Length)
            {
                throw new WebAuthnException("The security key returned unexpected trailing authenticator data.");
            }
        }

        internal static void ValidateEs256PublicKey(byte[] x, byte[] y)
        {
            byte[] blob = CreateEccPublicBlob(x, y);
            try
            {
                using (CngKey key = CngKey.Import(blob, CngKeyBlobFormat.EccPublicBlob))
                using (ECDsaCng ecdsa = new ECDsaCng(key))
                {
                    // Importing the key asks CNG to validate the P-256 public point.
                }
            }
            catch (CryptographicException ex)
            {
                throw new WebAuthnException("The security key returned an invalid P-256 public key: " + ex.Message);
            }
            finally
            {
                Array.Clear(blob, 0, blob.Length);
            }
        }

        internal static void VerifyAssertionSignature(byte[] authenticatorData, byte[] clientDataJson, byte[] derSignature, Credential credential)
        {
            byte[] clientDataHash = null;
            byte[] signedData = null;
            byte[] signedHash = null;
            byte[] signature = null;
            byte[] blob = null;
            try
            {
                ValidateEs256PublicKey(credential.PublicKeyX, credential.PublicKeyY);
                using (SHA256 sha = SHA256.Create()) clientDataHash = sha.ComputeHash(clientDataJson);

                signedData = new byte[authenticatorData.Length + clientDataHash.Length];
                Buffer.BlockCopy(authenticatorData, 0, signedData, 0, authenticatorData.Length);
                Buffer.BlockCopy(clientDataHash, 0, signedData, authenticatorData.Length, clientDataHash.Length);
                using (SHA256 sha = SHA256.Create()) signedHash = sha.ComputeHash(signedData);

                signature = DerEcdsaToP1363(derSignature);
                blob = CreateEccPublicBlob(credential.PublicKeyX, credential.PublicKeyY);
                using (CngKey key = CngKey.Import(blob, CngKeyBlobFormat.EccPublicBlob))
                using (ECDsaCng ecdsa = new ECDsaCng(key))
                {
                    if (!ecdsa.VerifyHash(signedHash, signature))
                        throw new WebAuthnException("The security key assertion signature is invalid.");
                }
            }
            catch (CryptographicException ex)
            {
                throw new WebAuthnException("The security key assertion could not be verified: " + ex.Message);
            }
            finally
            {
                ClearBytes(clientDataHash);
                ClearBytes(signedData);
                ClearBytes(signedHash);
                ClearBytes(signature);
                ClearBytes(blob);
            }
        }

        static void ClearBytes(byte[] value)
        {
            if (value != null) Array.Clear(value, 0, value.Length);
        }

        static byte[] CreateEccPublicBlob(byte[] x, byte[] y)
        {
            if (x == null || y == null || x.Length != 32 || y.Length != 32)
                throw new WebAuthnException("The stored ES256 public key is invalid.");
            byte[] blob = new byte[8 + 32 + 32];
            Buffer.BlockCopy(BitConverter.GetBytes(unchecked((int)0x31534345)), 0, blob, 0, 4); // BCRYPT_ECDSA_PUBLIC_P256_MAGIC
            Buffer.BlockCopy(BitConverter.GetBytes(32), 0, blob, 4, 4);
            Buffer.BlockCopy(x, 0, blob, 8, 32);
            Buffer.BlockCopy(y, 0, blob, 40, 32);
            return blob;
        }

        static byte[] DerEcdsaToP1363(byte[] der)
        {
            if (der == null || der.Length < 8 || der.Length > 72 || der[0] != 0x30)
                throw new WebAuthnException("The security key returned a malformed ES256 signature.");

            int offset = 1;
            int sequenceLength = ReadDerLength(der, ref offset);
            if (sequenceLength != der.Length - offset) throw new WebAuthnException("The security key returned a malformed ES256 signature.");
            byte[] result = new byte[64];
            ReadDerInteger(der, ref offset, result, 0);
            ReadDerInteger(der, ref offset, result, 32);
            if (offset != der.Length) throw new WebAuthnException("The security key returned a malformed ES256 signature.");
            return result;
        }

        static int ReadDerLength(byte[] data, ref int offset)
        {
            if (offset >= data.Length) throw new WebAuthnException("The security key returned a malformed ES256 signature.");
            int first = data[offset++];
            if (first < 0x80) return first;
            int count = first & 0x7f;
            if (count != 1 || offset >= data.Length) throw new WebAuthnException("The security key returned a malformed ES256 signature.");
            int length = data[offset++];
            if (length < 0x80) throw new WebAuthnException("The security key returned a non-canonical ES256 signature.");
            return length;
        }

        static void ReadDerInteger(byte[] data, ref int offset, byte[] destination, int destinationOffset)
        {
            if (offset + 2 > data.Length || data[offset++] != 0x02)
                throw new WebAuthnException("The security key returned a malformed ES256 signature.");
            int length = ReadDerLength(data, ref offset);
            if (length < 1 || length > 33 || offset + length > data.Length)
                throw new WebAuthnException("The security key returned a malformed ES256 signature.");
            if ((data[offset] & 0x80) != 0)
                throw new WebAuthnException("The security key returned a negative ES256 signature integer.");
            if (length > 1 && data[offset] == 0 && (data[offset + 1] & 0x80) == 0)
                throw new WebAuthnException("The security key returned a non-canonical ES256 signature integer.");
            if (length == 33)
            {
                if (data[offset] != 0) throw new WebAuthnException("The security key returned an oversized ES256 signature integer.");
                offset++;
                length--;
            }
            Buffer.BlockCopy(data, offset, destination, destinationOffset + 32 - length, length);
            offset += length;
        }

        static int ReadCborInteger(byte[] data, ref int offset)
        {
            int major;
            ulong value;
            ReadCborHead(data, ref offset, out major, out value);
            if (value > int.MaxValue) throw new WebAuthnException("The security key returned an unsupported COSE integer.");
            if (major == 0) return (int)value;
            if (major == 1) return -1 - (int)value;
            throw new WebAuthnException("The security key returned an invalid COSE parameter.");
        }

        static byte[] ReadCborByteString(byte[] data, ref int offset, int expectedLength)
        {
            int major;
            ulong length;
            ReadCborHead(data, ref offset, out major, out length);
            if (major != 2 || length != (ulong)expectedLength || length > (ulong)(data.Length - offset))
                throw new WebAuthnException("The security key returned an invalid ES256 coordinate.");
            byte[] value = new byte[(int)length];
            Buffer.BlockCopy(data, offset, value, 0, value.Length);
            offset += value.Length;
            return value;
        }

        static void ReadCborHead(byte[] data, ref int offset, out int major, out ulong value)
        {
            if (offset >= data.Length) throw new WebAuthnException("The security key returned truncated CBOR data.");
            byte initial = data[offset++];
            major = initial >> 5;
            int additional = initial & 0x1f;
            if (additional < 24) { value = (ulong)additional; return; }
            int bytes = additional == 24 ? 1 : additional == 25 ? 2 : additional == 26 ? 4 : additional == 27 ? 8 : 0;
            if (bytes == 0 || offset + bytes > data.Length)
                throw new WebAuthnException("The security key returned unsupported or truncated CBOR data.");
            value = 0;
            for (int i = 0; i < bytes; i++) value = (value << 8) | data[offset++];
            if ((bytes == 1 && value < 24) || (bytes == 2 && value <= byte.MaxValue)
                || (bytes == 4 && value <= ushort.MaxValue) || (bytes == 8 && value <= uint.MaxValue))
                throw new WebAuthnException("The security key returned non-canonical CBOR data.");
        }

        static void SkipCborValue(byte[] data, ref int offset, int depth)
        {
            if (depth > 8) throw new WebAuthnException("The security key returned excessively nested CBOR data.");
            int major;
            ulong value;
            ReadCborHead(data, ref offset, out major, out value);
            if (major == 2 || major == 3)
            {
                if (value > (ulong)(data.Length - offset)) throw new WebAuthnException("The security key returned truncated CBOR data.");
                offset += (int)value;
                return;
            }
            if (major == 4 || major == 5)
            {
                if (major == 5 && value > 128)
                    throw new WebAuthnException("The security key returned oversized CBOR data.");
                ulong children = major == 5 ? value * 2 : value;
                if (children > 256) throw new WebAuthnException("The security key returned oversized CBOR data.");
                for (ulong i = 0; i < children; i++) SkipCborValue(data, ref offset, depth + 1);
                return;
            }
            if (major == 6)
            {
                SkipCborValue(data, ref offset, depth + 1);
                return;
            }
            if (major > 7) throw new WebAuthnException("The security key returned invalid CBOR data.");
        }

        static void Require()
        {
            if (!IsAvailable) throw new WebAuthnException(UnavailableReason);
        }

        const int NTE_USER_CANCELLED = unchecked((int)0x80090036);
        const int NTE_NOT_FOUND = unchecked((int)0x80090011);
        const int NTE_EXISTS = unchecked((int)0x8009000F);

        static void Check(int hr)
        {
            if (hr == 0) return;
            if (hr == NTE_USER_CANCELLED) throw new WebAuthnException("The security key prompt was cancelled or timed out.", true);
            if (hr == NTE_EXISTS) throw new WebAuthnException("This security key is already registered.");
            if (hr == NTE_NOT_FOUND) throw new WebAuthnException("This security key isn't registered for this vault.");

            string name = null;
            try { name = Marshal.PtrToStringUni(WebAuthNGetErrorName(hr)); }
            catch { }
            throw new WebAuthnException("The security key operation failed (" + (string.IsNullOrEmpty(name) ? "0x" + hr.ToString("X8") : name) + ").");
        }

        static byte[] RandomBytes(int count)
        {
            return CryLain.RandomBytes(count);
        }

        static string Base64Url(byte[] b)
        {
            return Convert.ToBase64String(b).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        // Tracks unmanaged allocations for one call.
        sealed class Native : IDisposable
        {
            readonly List<IntPtr> _blocks = new List<IntPtr>();

            internal IntPtr Str(string s)
            {
                IntPtr p = Marshal.StringToHGlobalUni(s);
                _blocks.Add(p);
                return p;
            }

            internal IntPtr Bytes(byte[] b)
            {
                IntPtr p = Marshal.AllocHGlobal(Math.Max(1, b.Length));
                _blocks.Add(p);
                Marshal.Copy(b, 0, p, b.Length);
                return p;
            }

            internal IntPtr Struct<T>(T value) where T : struct
            {
                IntPtr p = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
                _blocks.Add(p);
                Marshal.StructureToPtr(value, p, false);
                return p;
            }

            internal IntPtr Array<T>(T[] items) where T : struct
            {
                int size = Marshal.SizeOf(typeof(T));
                IntPtr p = Marshal.AllocHGlobal(Math.Max(1, size * items.Length));
                _blocks.Add(p);
                for (int i = 0; i < items.Length; i++) Marshal.StructureToPtr(items[i], p + i * size, false);
                return p;
            }

            internal Credentials Credentials(IList<byte[]> ids)
            {
                if (ids == null || ids.Count == 0) return new Credentials();
                CredentialNative[] list = new CredentialNative[ids.Count];
                IntPtr type = Str("public-key");
                for (int i = 0; i < ids.Count; i++) list[i] = new CredentialNative { dwVersion = 1, cbId = ids[i].Length, pbId = Bytes(ids[i]), pwszCredentialType = type };
                return new Credentials { cCredentials = list.Length, pCredentials = Array(list) };
            }

            internal ClientData ClientData(string type)
            {
                byte[] ignored;
                return ClientData(type, out ignored);
            }

            internal ClientData ClientData(string type, out byte[] jsonBytes)
            {
                string json = "{\"type\":\"" + type + "\",\"challenge\":\"" + Base64Url(RandomBytes(32)) + "\",\"origin\":\"" + Origin + "\",\"crossOrigin\":false}";
                jsonBytes = Encoding.UTF8.GetBytes(json);
                return new ClientData { dwVersion = 1, cbClientDataJSON = jsonBytes.Length, pbClientDataJSON = Bytes(jsonBytes), pwszHashAlgId = Str("SHA-256") };
            }

            public void Dispose()
            {
                foreach (IntPtr p in _blocks) Marshal.FreeHGlobal(p);
                _blocks.Clear();
            }
        }

        #region Native

        [StructLayout(LayoutKind.Sequential)]
        struct RpInfo { public int dwVersion; public IntPtr pwszId, pwszName, pwszIcon; }

        [StructLayout(LayoutKind.Sequential)]
        struct UserInfo { public int dwVersion, cbId; public IntPtr pbId, pwszName, pwszIcon, pwszDisplayName; }

        [StructLayout(LayoutKind.Sequential)]
        struct ClientData { public int dwVersion, cbClientDataJSON; public IntPtr pbClientDataJSON, pwszHashAlgId; }

        [StructLayout(LayoutKind.Sequential)]
        struct CoseParam { public int dwVersion; public IntPtr pwszCredentialType; public int lAlg; }

        [StructLayout(LayoutKind.Sequential)]
        struct CoseParams { public int cCredentialParameters; public IntPtr pCredentialParameters; }

        [StructLayout(LayoutKind.Sequential)]
        struct CredentialNative { public int dwVersion, cbId; public IntPtr pbId, pwszCredentialType; }

        [StructLayout(LayoutKind.Sequential)]
        struct Credentials { public int cCredentials; public IntPtr pCredentials; }

        [StructLayout(LayoutKind.Sequential)]
        struct Extension { public IntPtr pwszExtensionIdentifier; public int cbExtension; public IntPtr pvExtension; }

        [StructLayout(LayoutKind.Sequential)]
        struct Extensions { public int cExtensions; public IntPtr pExtensions; }

        [StructLayout(LayoutKind.Sequential)]
        struct SecretSalt { public int cbFirst; public IntPtr pbFirst; public int cbSecond; public IntPtr pbSecond; }

        [StructLayout(LayoutKind.Sequential)]
        struct CredSalt { public int cbCredID; public IntPtr pbCredID, pHmacSecretSalt; }

        [StructLayout(LayoutKind.Sequential)]
        struct SaltValues { public IntPtr pGlobalHmacSalt; public int cCredWithHmacSecretSaltList; public IntPtr pCredWithHmacSecretSaltList; }

        // WEBAUTHN_AUTHENTICATOR_MAKE_CREDENTIAL_OPTIONS up to version 8 (Windows reads only the fields of dwVersion)
        [StructLayout(LayoutKind.Sequential)]
        struct MakeOptions
        {
            public int dwVersion, dwTimeoutMilliseconds;
            public Credentials CredentialList;
            public Extensions Extensions;
            public int dwAuthenticatorAttachment, bRequireResidentKey, dwUserVerificationRequirement, dwAttestationConveyancePreference, dwFlags;
            public IntPtr pCancellationId, pExcludeCredentialList;
            public int dwEnterpriseAttestation, dwLargeBlobSupport, bPreferResidentKey;
            public int bBrowserInPrivateMode, bEnablePrf;
            public IntPtr pLinkedDevice;
            public int cbJsonExt;
            public IntPtr pbJsonExt, pPRFGlobalEval;
            public int cCredentialHints;
            public IntPtr ppwszCredentialHints;
            public int bThirdPartyPayment;
        }

        // WEBAUTHN_AUTHENTICATOR_GET_ASSERTION_OPTIONS up to version 6
        [StructLayout(LayoutKind.Sequential)]
        struct GetOptions
        {
            public int dwVersion, dwTimeoutMilliseconds;
            public Credentials CredentialList;
            public Extensions Extensions;
            public int dwAuthenticatorAttachment, dwUserVerificationRequirement, dwFlags;
            public IntPtr pwszU2fAppId, pbU2fAppId, pCancellationId, pAllowCredentialList;
            public int dwCredLargeBlobOperation, cbCredLargeBlob;
            public IntPtr pbCredLargeBlob, pHmacSecretSaltValues;
            public int bBrowserInPrivateMode;
        }

        // WEBAUTHN_CREDENTIAL_ATTESTATION up to version 2
        [StructLayout(LayoutKind.Sequential)]
        struct Attestation
        {
            public int dwVersion;
            public IntPtr pwszFormatType;
            public int cbAuthenticatorData;
            public IntPtr pbAuthenticatorData;
            public int cbAttestation;
            public IntPtr pbAttestation;
            public int dwAttestationDecodeType;
            public IntPtr pvAttestationDecode;
            public int cbAttestationObject;
            public IntPtr pbAttestationObject;
            public int cbCredentialId;
            public IntPtr pbCredentialId;
            public Extensions Extensions;
        }

        // WEBAUTHN_CREDENTIAL_ATTESTATION up to version 7. Only used for field offsets: Windows may return a shorter
        // (older) struct, so fields are read individually after checking dwVersion.
        [StructLayout(LayoutKind.Sequential)]
        struct AttestationV7
        {
            public Attestation Base;
            public int dwUsedTransport, bEpAtt, bLargeBlobSupported, bResidentKey, bPrfEnabled, cbUnsignedExtensionOutputs;
            public IntPtr pbUnsignedExtensionOutputs, pHmacSecret;
            public int bThirdPartyPayment;
        }

        static int AttestationOffset(string field)
        {
            return (int)Marshal.OffsetOf(typeof(AttestationV7), field);
        }

        // WEBAUTHN_ASSERTION up to version 3
        [StructLayout(LayoutKind.Sequential)]
        struct AssertionNative
        {
            public int dwVersion, cbAuthenticatorData;
            public IntPtr pbAuthenticatorData;
            public int cbSignature;
            public IntPtr pbSignature;
            public CredentialNative Credential;
            public int cbUserId;
            public IntPtr pbUserId;
            public Extensions Extensions;
            public int cbCredLargeBlob;
            public IntPtr pbCredLargeBlob;
            public int dwCredLargeBlobStatus;
            public IntPtr pHmacSecret;
        }

        [DllImport("webauthn.dll")]
        static extern uint WebAuthNGetApiVersionNumber();

        [DllImport("webauthn.dll")]
        static extern int WebAuthNAuthenticatorMakeCredential(IntPtr hWnd, ref RpInfo rp, ref UserInfo user, ref CoseParams pubKeyCredParams,
            ref ClientData clientData, ref MakeOptions options, out IntPtr attestation);

        [DllImport("webauthn.dll", CharSet = CharSet.Unicode)]
        static extern int WebAuthNAuthenticatorGetAssertion(IntPtr hWnd, string rpId, ref ClientData clientData, ref GetOptions options, out IntPtr assertion);

        [DllImport("webauthn.dll")]
        static extern void WebAuthNFreeCredentialAttestation(IntPtr attestation);

        [DllImport("webauthn.dll")]
        static extern void WebAuthNFreeAssertion(IntPtr assertion);

        [DllImport("webauthn.dll")]
        static extern IntPtr WebAuthNGetErrorName(int hr);

        #endregion
    }
}
