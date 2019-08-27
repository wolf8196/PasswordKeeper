using PasswordKeeper.Security;

namespace PasswordKeeper.Services.Abstractions
{
    public interface IDatabaseProvider
    {
        ICryptoDatabase GetDatabase(string location);
    }
}