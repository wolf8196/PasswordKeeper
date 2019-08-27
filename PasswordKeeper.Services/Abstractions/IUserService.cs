using System.Threading.Tasks;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Services.Abstractions
{
    public interface IUserService
    {
        User User { get; }

        Task CreateAsync(IStorageProvider storageProvider, string userName, string password);

        Task LogInAsync(string userName, string password);

        Task LogOutAsync();

        Task SaveAsync();

        Task ChangeKey(IStorageProvider storageProvider, string password);

        Task BeginSaveAsync();

        Task EndSaveAsync();
    }
}