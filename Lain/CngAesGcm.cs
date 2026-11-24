using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Lain
{
    // AES-256-GCM adapter over Windows CNG for the .NET Framework 4.8 target.
    // The key object and algorithm provider live for the lifetime of this instance.
    internal sealed class CngAesGcm : IDisposable
    {
        const string AlgorithmAes = "AES";
        const string PropertyChainingMode = "ChainingMode";
        const string ChainingModeGcm = "ChainingModeGCM";
        const string PropertyObjectLength = "ObjectLength";
        const string PropertyAuthTagLength = "AuthTagLength";

        const int NonceSize = 12;
        const int TagSize = 16;

        IntPtr _algorithm;
        IntPtr _key;
        IntPtr _keyObject;
        int _keyObjectLength;
        bool _disposed;

        [StructLayout(LayoutKind.Sequential)]
        struct AuthTagLengths
        {
            internal uint Minimum;
            internal uint Maximum;
            internal uint Increment;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        struct AuthenticatedCipherModeInfo
        {
            internal uint cbSize;
            internal uint dwInfoVersion;
            internal IntPtr pbNonce;
            internal uint cbNonce;
            internal IntPtr pbAuthData;
            internal uint cbAuthData;
            internal IntPtr pbTag;
            internal uint cbTag;
            internal IntPtr pbMacContext;
            internal uint cbMacContext;
            internal uint cbAAD;
            internal ulong cbData;
            internal uint dwFlags;
        }

        [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
        static extern int BCryptOpenAlgorithmProvider(out IntPtr phAlgorithm, string pszAlgId, string pszImplementation, int dwFlags);

        [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
        static extern int BCryptSetProperty(IntPtr hObject, string pszProperty, IntPtr pbInput, int cbInput, int dwFlags);

        [DllImport("bcrypt.dll", CharSet = CharSet.Unicode)]
        static extern int BCryptGetProperty(IntPtr hObject, string pszProperty, IntPtr pbOutput, int cbOutput, out int pcbResult, int dwFlags);

        [DllImport("bcrypt.dll")]
        static extern int BCryptGenerateSymmetricKey(IntPtr hAlgorithm, out IntPtr phKey, IntPtr pbKeyObject, int cbKeyObject, IntPtr pbSecret, int cbSecret, int dwFlags);

        [DllImport("bcrypt.dll")]
        static extern int BCryptEncrypt(IntPtr hKey, IntPtr pbInput, int cbInput, IntPtr pPaddingInfo, IntPtr pbIV, int cbIV,
            IntPtr pbOutput, int cbOutput, out int pcbResult, int dwFlags);

        [DllImport("bcrypt.dll")]
        static extern int BCryptDecrypt(IntPtr hKey, IntPtr pbInput, int cbInput, IntPtr pPaddingInfo, IntPtr pbIV, int cbIV,
            IntPtr pbOutput, int cbOutput, out int pcbResult, int dwFlags);

        [DllImport("bcrypt.dll")]
        static extern int BCryptDestroyKey(IntPtr hKey);

        [DllImport("bcrypt.dll")]
        static extern int BCryptCloseAlgorithmProvider(IntPtr hAlgorithm, int dwFlags);

        internal CngAesGcm(byte[] key)
        {
            if (key == null) throw new ArgumentNullException("key");
            if (key.Length != 32) throw new ArgumentException("AES-256 requires a 32-byte key.", "key");

            try
            {
                Check(BCryptOpenAlgorithmProvider(out _algorithm, AlgorithmAes, null, 0), "opening the AES provider");

                IntPtr mode = Marshal.StringToHGlobalUni(ChainingModeGcm);
                try
                {
                    int modeLength = Encoding.Unicode.GetByteCount(ChainingModeGcm) + 2;
                    Check(BCryptSetProperty(_algorithm, PropertyChainingMode, mode, modeLength, 0), "configuring AES-GCM");
                }
                finally
                {
                    Marshal.FreeHGlobal(mode);
                }

                IntPtr tagLengthBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(AuthTagLengths)));
                try
                {
                    int resultLength;
                    Check(BCryptGetProperty(_algorithm, PropertyAuthTagLength, tagLengthBuffer,
                        Marshal.SizeOf(typeof(AuthTagLengths)), out resultLength, 0), "reading supported AES-GCM tag lengths");
                    if (resultLength != Marshal.SizeOf(typeof(AuthTagLengths)))
                        throw new CryptographicException("Windows CNG returned invalid AES-GCM tag-length information.");
                    AuthTagLengths lengths = (AuthTagLengths)Marshal.PtrToStructure(tagLengthBuffer, typeof(AuthTagLengths));
                    if (lengths.Minimum > TagSize || lengths.Maximum < TagSize || lengths.Increment == 0 ||
                        (TagSize - lengths.Minimum) % lengths.Increment != 0)
                        throw new CryptographicException("The Windows AES-GCM provider does not support a 128-bit authentication tag.");
                }
                finally
                {
                    Marshal.FreeHGlobal(tagLengthBuffer);
                }

                IntPtr objectLengthBuffer = Marshal.AllocHGlobal(sizeof(int));
                try
                {
                    int resultLength;
                    Check(BCryptGetProperty(_algorithm, PropertyObjectLength, objectLengthBuffer, sizeof(int), out resultLength, 0),
                        "reading the AES key-object size");
                    if (resultLength != sizeof(int)) throw new CryptographicException("Windows CNG returned an invalid AES key-object size.");
                    _keyObjectLength = Marshal.ReadInt32(objectLengthBuffer);
                }
                finally
                {
                    Marshal.FreeHGlobal(objectLengthBuffer);
                }

                if (_keyObjectLength <= 0 || _keyObjectLength > 1024 * 1024)
                    throw new CryptographicException("Windows CNG returned an unsupported AES key-object size.");

                _keyObject = Marshal.AllocHGlobal(_keyObjectLength);
                GCHandle secretHandle = GCHandle.Alloc(key, GCHandleType.Pinned);
                try
                {
                    Check(BCryptGenerateSymmetricKey(_algorithm, out _key, _keyObject, _keyObjectLength,
                        secretHandle.AddrOfPinnedObject(), key.Length, 0), "creating the AES-256-GCM key");
                }
                finally
                {
                    secretHandle.Free();
                }
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        internal byte[] Encrypt(byte[] plaintext, byte[] nonce, byte[] associatedData, out byte[] tag)
        {
            CheckNotDisposed();
            ValidateInputs(plaintext, nonce, associatedData);

            tag = null;
            byte[] generatedTag = new byte[TagSize];
            byte[] nonceBuffer = (byte[])nonce.Clone();
            byte[] input = plaintext.Length == 0 ? new byte[1] : plaintext;
            byte[] output = new byte[Math.Max(1, plaintext.Length)];
            int written = 0;
            try
            {
                using (PinnedBuffers buffers = new PinnedBuffers(nonceBuffer, associatedData, generatedTag, input, output))
                {
                    IntPtr info = CreateInfo(buffers, nonceBuffer.Length, associatedData.Length, TagSize);
                    try
                    {
                        Check(BCryptEncrypt(_key, buffers.Input, plaintext.Length, info, IntPtr.Zero, 0,
                            buffers.Output, output.Length, out written, 0), "encrypting with AES-256-GCM");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(info);
                    }
                }

                if (written != plaintext.Length)
                    throw new CryptographicException("Windows CNG returned an unexpected AES-GCM ciphertext length.");

                byte[] ciphertext = new byte[written];
                if (written > 0) Buffer.BlockCopy(output, 0, ciphertext, 0, written);
                tag = generatedTag;
                generatedTag = null;
                return ciphertext;
            }
            finally
            {
                if (input != plaintext) Array.Clear(input, 0, input.Length);
                Array.Clear(nonceBuffer, 0, nonceBuffer.Length);
                Array.Clear(output, 0, output.Length);
                if (generatedTag != null) Array.Clear(generatedTag, 0, generatedTag.Length);
            }
        }

        internal byte[] Decrypt(byte[] ciphertext, byte[] nonce, byte[] associatedData, byte[] tag)
        {
            CheckNotDisposed();
            ValidateInputs(ciphertext, nonce, associatedData);
            if (tag == null || tag.Length != TagSize) throw new CryptographicException("Malformed AES-GCM authentication tag.");

            byte[] nonceBuffer = (byte[])nonce.Clone();
            byte[] input = ciphertext.Length == 0 ? new byte[1] : ciphertext;
            byte[] output = new byte[Math.Max(1, ciphertext.Length)];
            int written = 0;

            try
            {
                using (PinnedBuffers buffers = new PinnedBuffers(nonceBuffer, associatedData, tag, input, output))
                {
                    IntPtr info = CreateInfo(buffers, nonceBuffer.Length, associatedData.Length, TagSize);
                    try
                    {
                        Check(BCryptDecrypt(_key, buffers.Input, ciphertext.Length, info, IntPtr.Zero, 0,
                            buffers.Output, output.Length, out written, 0), "authenticating AES-256-GCM ciphertext");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(info);
                    }
                }

                if (written != ciphertext.Length)
                    throw new CryptographicException("Windows CNG returned an unexpected AES-GCM plaintext length.");

                byte[] plaintext = new byte[written];
                if (written > 0) Buffer.BlockCopy(output, 0, plaintext, 0, written);
                return plaintext;
            }
            catch (CryptographicException)
            {
                Array.Clear(output, 0, output.Length);
                throw new CryptographicException("Vault value failed authentication.");
            }
            finally
            {
                if (input != ciphertext) Array.Clear(input, 0, input.Length);
                Array.Clear(nonceBuffer, 0, nonceBuffer.Length);
                Array.Clear(output, 0, output.Length);
            }
        }

        static void ValidateInputs(byte[] data, byte[] nonce, byte[] associatedData)
        {
            if (data == null) throw new ArgumentNullException("data");
            if (nonce == null || nonce.Length != NonceSize) throw new ArgumentException("AES-GCM requires a 12-byte nonce.", "nonce");
            if (associatedData == null) throw new ArgumentNullException("associatedData");
        }

        static IntPtr CreateInfo(PinnedBuffers buffers, int nonceLength, int aadLength, int tagLength)
        {
            AuthenticatedCipherModeInfo info = new AuthenticatedCipherModeInfo
            {
                cbSize = (uint)Marshal.SizeOf(typeof(AuthenticatedCipherModeInfo)),
                dwInfoVersion = 1,
                pbNonce = buffers.Nonce,
                cbNonce = (uint)nonceLength,
                pbAuthData = aadLength == 0 ? IntPtr.Zero : buffers.AssociatedData,
                cbAuthData = (uint)aadLength,
                pbTag = buffers.Tag,
                cbTag = (uint)tagLength,
                pbMacContext = IntPtr.Zero,
                cbMacContext = 0,
                cbAAD = 0,
                cbData = 0,
                dwFlags = 0
            };

            IntPtr memory = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(AuthenticatedCipherModeInfo)));
            Marshal.StructureToPtr(info, memory, false);
            return memory;
        }

        static void Check(int status, string operation)
        {
            if (status < 0)
                throw new CryptographicException("Windows CNG failed while " + operation + " (NTSTATUS 0x" + unchecked((uint)status).ToString("X8") + ").");
        }

        void CheckNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException("CngAesGcm");
            if (_key == IntPtr.Zero) throw new CryptographicException("The AES-GCM key is unavailable.");
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (_key != IntPtr.Zero)
            {
                BCryptDestroyKey(_key);
                _key = IntPtr.Zero;
            }

            if (_keyObject != IntPtr.Zero)
            {
                byte[] zeroes = new byte[_keyObjectLength];
                Marshal.Copy(zeroes, 0, _keyObject, zeroes.Length);
                Array.Clear(zeroes, 0, zeroes.Length);
                Marshal.FreeHGlobal(_keyObject);
                _keyObject = IntPtr.Zero;
            }

            if (_algorithm != IntPtr.Zero)
            {
                BCryptCloseAlgorithmProvider(_algorithm, 0);
                _algorithm = IntPtr.Zero;
            }
        }

        sealed class PinnedBuffers : IDisposable
        {
            GCHandle _nonceHandle;
            GCHandle _aadHandle;
            GCHandle _tagHandle;
            GCHandle _inputHandle;
            GCHandle _outputHandle;

            internal IntPtr Nonce { get { return _nonceHandle.AddrOfPinnedObject(); } }
            internal IntPtr AssociatedData { get { return _aadHandle.AddrOfPinnedObject(); } }
            internal IntPtr Tag { get { return _tagHandle.AddrOfPinnedObject(); } }
            internal IntPtr Input { get { return _inputHandle.AddrOfPinnedObject(); } }
            internal IntPtr Output { get { return _outputHandle.AddrOfPinnedObject(); } }

            internal PinnedBuffers(byte[] nonce, byte[] associatedData, byte[] tag, byte[] input, byte[] output)
            {
                try
                {
                    _nonceHandle = GCHandle.Alloc(nonce, GCHandleType.Pinned);
                    _aadHandle = GCHandle.Alloc(associatedData, GCHandleType.Pinned);
                    _tagHandle = GCHandle.Alloc(tag, GCHandleType.Pinned);
                    _inputHandle = GCHandle.Alloc(input, GCHandleType.Pinned);
                    _outputHandle = GCHandle.Alloc(output, GCHandleType.Pinned);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public void Dispose()
            {
                if (_outputHandle.IsAllocated) _outputHandle.Free();
                if (_inputHandle.IsAllocated) _inputHandle.Free();
                if (_tagHandle.IsAllocated) _tagHandle.Free();
                if (_aadHandle.IsAllocated) _aadHandle.Free();
                if (_nonceHandle.IsAllocated) _nonceHandle.Free();
            }
        }
    }
}
