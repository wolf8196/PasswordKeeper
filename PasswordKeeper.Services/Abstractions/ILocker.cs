using System;

namespace PasswordKeeper.Services.Abstractions
{
    public interface ILocker : IDisposable
    {
        bool IsLocked(string itemName);

        void Lock(string itemName);

        void Unlock(string itemName);
    }
}