using System;
using PasswordKeeper.Security;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Enums;

namespace PasswordKeeper.Services.CryptoProviders
{
    public class ConfigurableStorageProvider : IStorageProvider
    {
        public EncryptionAlgorithms EncryptionAlgorithm { get; set; }

        public IKdfProvider KdfProvider { get; set; }

        public ICryptoStorage GetStorage()
        {
            return new CryptoStorage(KdfProvider.GetKeyDerivationFunction(), GetEncryptor());
        }

        private IEncryptor GetEncryptor()
        {
            switch (EncryptionAlgorithm)
            {
                case EncryptionAlgorithms.Aes256CbcHmacSha512:
                    return new Aes256CbcHmacSha512();

                default:
                    throw new ArgumentException($"Invalid encryption alogithm: {EncryptionAlgorithm}");
            }
        }
    }
}