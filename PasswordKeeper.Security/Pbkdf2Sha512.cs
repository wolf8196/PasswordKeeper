using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public sealed partial class Pbkdf2Sha512 : IKeyDerivationFunction
    {
        public const int DefaultSaltSize = 8;
        public const int DefaultIterations = 500000;

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

            // Changed to BouncyCastle's implementation of PBKDF2
            // Because .Net Standard 2.0 does not allow to use SHA512
            var generator = new Pkcs5S2ParametersGenerator(new Sha512Digest());
            generator.Init(key, salt, iterations);
            var keyParameter = (KeyParameter)generator.GenerateDerivedMacParameters(keySize * 8);
            return keyParameter.GetKey();
        }

        public ISerializer GetSerializer()
        {
            return new Serializer(this);
        }
    }
}