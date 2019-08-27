using PasswordKeeper.Utils;
using StreamIndexingUtils.Models;

namespace PasswordKeeper.Security
{
    public sealed partial class CryptoStorage
    {
        private sealed class Memento : IMemento
        {
            private readonly CryptoStorage storage;
            private readonly ContentIndex content;

            internal Memento(CryptoStorage storage)
            {
                this.storage = storage;
                content = new ContentIndex(storage.ContentIndex);
            }

            public void Restore()
            {
                storage.ContentIndex = content;
            }
        }
    }
}