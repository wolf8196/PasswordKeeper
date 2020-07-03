using PasswordKeeper.Security;
using PasswordKeeper.Services.Abstractions;

namespace PasswordKeeper.Services.CryptoProviders
{
    public class DefaultStorageProvider : IStorageProvider
    {
        public ICryptoStorage GetStorage()
        {
            return new CryptoStorage(new Pbkdf2Sha512(Pbkdf2Sha512.DefaultSaltSize, Pbkdf2Sha512.DefaultIterations), new Aes256CbcHmacSha512());
        }
    }
}