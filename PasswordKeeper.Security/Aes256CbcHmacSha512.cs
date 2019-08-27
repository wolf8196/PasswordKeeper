using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PasswordKeeper.Utils;
using StreamIndexingUtils.Extensions;

namespace PasswordKeeper.Security
{
    public sealed partial class Aes256CbcHmacSha512 : IEncryptor
    {
        private const int BlockSize = 16;
        private const int MacSize = 64;

        // AES-256 key size
        private const int EncryptionKeySize = 32;

        // Microsoft recommended key size for HMACSHA512
        private const int MacKeySize = 128;

        public int KeySize => RequiredKeySize;

        private static int RequiredKeySize => EncryptionKeySize + MacKeySize;

        public async Task EncryptAsync(Stream source, Stream destination, byte[] key)
        {
            CheckParameters(source, destination, key);

            var (macKey, encryptionKey) = SplitKeys(key);

            using (var aes = CreateAes())
            using (var encryptor = aes.CreateEncryptor(encryptionKey, aes.IV))
            using (var hmac = CreateHmac(macKey))
            {
                var cipherTextStart = destination.Position;

                // Write initialization vector
                destination.Write(aes.IV, 0, aes.IV.Length);

                using (var aesStream = new CryptoStream(destination, encryptor, CryptoStreamMode.Write, true))
                {
                    // Encrypt plaintext
                    await source.CopyToAsync(aesStream).ConfigureAwait(false);
                    aesStream.FlushFinalBlock();

                    var cipherTextLength = destination.Position - cipherTextStart;

                    // Calculate HMAC over ciphertext
                    using (var hmacStream = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write))
                    {
                        destination.Seek(cipherTextStart, SeekOrigin.Begin);

                        await destination.CopyToAsync(hmacStream, cipherTextLength).ConfigureAwait(false);
                        hmacStream.FlushFinalBlock();

                        // Write authentication tag after ciphertext
                        await destination.WriteAsync(hmac.Hash, 0, hmac.Hash.Length).ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task DecryptAsync(Stream source, Stream destination, byte[] key)
        {
            CheckParameters(source, destination, key);

            var (macKey, encryptionKey) = SplitKeys(key);

            // Find beginning of authentication tag
            source.Seek(-MacSize, SeekOrigin.End);
            var authenticationTag = new byte[MacSize];
            await source.ReadAsync(authenticationTag, 0, MacSize).ConfigureAwait(false);

            using (var hmac = CreateHmac(macKey))
            using (var hmacStream = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write))
            {
                // Calculate HMAC over ciphertext
                source.Seek(0, SeekOrigin.Begin);
                await source.CopyToAsync(hmacStream, source.Length - MacSize).ConfigureAwait(false);
                hmacStream.FlushFinalBlock();

                // Check if calculated HMAC and authentication tag are the same
                if (!authenticationTag.SequenceEqual(hmac.Hash))
                {
                    throw new CipherTextAuthenticationException();
                }
            }

            // Read initialization vector
            source.Seek(0, SeekOrigin.Begin);
            var iv = new byte[BlockSize];
            await source.ReadAsync(iv, 0, iv.Length).ConfigureAwait(false);

            try
            {
                using (var aes = CreateAes())
                using (var decryptor = aes.CreateDecryptor(encryptionKey, iv))
                using (var aesStream = new CryptoStream(destination, decryptor, CryptoStreamMode.Write, true))
                {
                    // Decrypt ciphertext
                    await source.CopyToAsync(aesStream, source.Length - BlockSize - MacSize).ConfigureAwait(false);
                    aesStream.FlushFinalBlock();
                }
            }
            catch (CryptographicException)
            {
                throw new CipherTextAuthenticationException();
            }
        }

        public ISerializer GetSerializer()
        {
            return new Serializer();
        }

        private static (byte[] macKey, byte[] encryptionKey) SplitKeys(byte[] key)
        {
            var macKey = new byte[MacKeySize];
            var encryptionKey = new byte[EncryptionKeySize];
            Buffer.BlockCopy(key, 0, macKey, 0, MacKeySize);
            Buffer.BlockCopy(key, MacKeySize, encryptionKey, 0, EncryptionKeySize);

            return (macKey, encryptionKey);
        }

        private static Aes CreateAes()
        {
            var aesAlg = Aes.Create();
            aesAlg.Padding = PaddingMode.PKCS7;
            aesAlg.Mode = CipherMode.CBC;
            return aesAlg;
        }

        private static HMAC CreateHmac(byte[] key)
        {
            return new HMACSHA512(key);
        }

        private static void CheckParameters(Stream source, Stream destination, byte[] key)
        {
            source.ThrowIfNull(nameof(source));
            destination.ThrowIfNull(nameof(destination));
            key.ThrowIfNull(nameof(key));

            if (key.Length != RequiredKeySize)
            {
                throw new ArgumentException("Invalid key size", nameof(key));
            }
        }
    }
}