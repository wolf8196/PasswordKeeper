using System;
using System.Threading.Tasks;
using PasswordKeeper.Security;

namespace PasswordKeeper.Services.Abstractions
{
    public interface IDatabaseService : IDisposable
    {
        ICryptoDatabase CurrentDatabase { get; }

        Task<bool> OpenAsync(string location, byte[] key);

        Task SetupAsync(string location, ICryptoStorage cryptoStorage, byte[] key);

        void Close();
    }
}