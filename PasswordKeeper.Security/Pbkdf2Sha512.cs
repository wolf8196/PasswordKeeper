using System.Security.Cryptography;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public sealed partial class Pbkdf2Sha512 : IKeyDerivationFunction
    {
        private byte[] salt;
        private int iterations;

        public Pbkdf2Sha512(int saltSize, int iterations)
        {
            saltSize.ThrowIfDefault(nameof(saltSize));
            iterations.ThrowIfDefault(nameof(iterations));

            salt = new byte[saltSize];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(salt);
            }

            this.iterations = iterations;
        }

        public Pbkdf2Sha512(byte[] salt, int iterations)
        {
            salt.ThrowIfNull(nameof(salt));
            iterations.ThrowIfDefault(nameof(iterations));

            this.salt = salt;
            this.iterations = iterations;
        }

        internal Pbkdf2Sha512()
        {
        }

        public byte[] Generate(byte[] key, int keySize)
        {
            key.ThrowIfNull(nameof(key));
            keySize.ThrowIfDefault(nameof(keySize));

            using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(key, salt, iterations, HashAlgorithmName.SHA512))
            {
                return bytes.GetBytes(keySize);
            }
        }

        public ISerializer GetSerializer()
        {
            return new Serializer(this);
        }
    }
}