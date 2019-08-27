using System.IO;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public sealed partial class CryptoStorage
    {
        private sealed partial class Serializer : ISerializer
        {
            private readonly CryptoStorage storage;

            internal Serializer(CryptoStorage storage)
            {
                this.storage = storage;
            }

            public void Deserialize(Stream source)
            {
                storage.kdf = KeyDerivationFunctionSerializer.Deserialize(source);
                storage.encryptor = EncryptorSerializer.Deserialize(source);
            }

            public void Serialize(Stream destination)
            {
                KeyDerivationFunctionSerializer.Serialize(destination, storage.kdf);
                EncryptorSerializer.Serialize(destination, storage.encryptor);
            }
        }
    }
}