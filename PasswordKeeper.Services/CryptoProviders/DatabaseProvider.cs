using PasswordKeeper.Security;
using PasswordKeeper.Services.Abstractions;

namespace PasswordKeeper.Services.CryptoProviders
{
    public class DatabaseProvider : IDatabaseProvider
    {
        public ICryptoDatabase GetDatabase(string location)
        {
            return new CryptoDatabase(location);
        }
    }
}