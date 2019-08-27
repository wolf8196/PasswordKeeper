using PasswordKeeper.Security;
using PasswordKeeper.Services.Abstractions;

namespace PasswordKeeper.Services.CryptoProviders
{
    public class DefaultStorageProvider : IStorageProvider
    {
        private const int DecentNumberOfIterations = 100000;

        public ICryptoStorage GetStorage()
        {
            return new CryptoStorage(new Pbkdf2Sha512(8, DecentNumberOfIterations), new Aes256CbcHmacSha512());
        }
    }
}