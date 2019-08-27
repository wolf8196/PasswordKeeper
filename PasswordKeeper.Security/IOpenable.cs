using System;
using System.Threading.Tasks;

namespace PasswordKeeper.Security
{
    public interface IOpenable : IDisposable
    {
        bool IsOpened { get; }

        Task<bool> OpenAsync(byte[] key);

        void Close();
    }
}