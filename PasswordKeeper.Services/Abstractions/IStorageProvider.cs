using PasswordKeeper.Security;

namespace PasswordKeeper.Services.Abstractions
{
    public interface IStorageProvider
    {
        ICryptoStorage GetStorage();
    }
}