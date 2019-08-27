using System.Threading.Tasks;

namespace PasswordKeeper.Security
{
    public interface ICryptoDatabase : IStorage, IOpenable
    {
        string FileName { get; }

        Task SetupAsync(ICryptoStorage cryptoStorage, byte[] key);

        Task ResetupAsync(ICryptoStorage cryptoStorage, byte[] key);

        Task GetAsync(string id, string destinationFile);

        Task AddAsync(string id, string sourceFile);
    }
}