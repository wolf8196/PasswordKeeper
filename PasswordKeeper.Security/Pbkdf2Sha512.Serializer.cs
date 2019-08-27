using System;
using System.IO;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public sealed partial class Pbkdf2Sha512
    {
        private sealed class Serializer : ISerializer
        {
            private readonly Pbkdf2Sha512 kdf;

            internal Serializer(Pbkdf2Sha512 kdf)
            {
                this.kdf = kdf;
            }

            public void Serialize(Stream destination)
            {
                destination.ThrowIfNull(nameof(destination));

                var saltSize = BitConverter.GetBytes(kdf.salt.Length);
                var iterations = BitConverter.GetBytes(kdf.iterations);

                destination.Write(saltSize, 0, saltSize.Length);
                destination.Write(kdf.salt, 0, kdf.salt.Length);
                destination.Write(iterations, 0, iterations.Length);
            }

            public void Deserialize(Stream source)
            {
                var saltSize = new byte[sizeof(int)];
                source.Read(saltSize, 0, saltSize.Length);

                var salt = new byte[BitConverter.ToInt32(saltSize, 0)];
                var iterations = new byte[sizeof(int)];

                source.Read(salt, 0, salt.Length);
                source.Read(iterations, 0, iterations.Length);

                kdf.salt = salt;
                kdf.iterations = BitConverter.ToInt32(iterations, 0);
            }
        }
    }
}