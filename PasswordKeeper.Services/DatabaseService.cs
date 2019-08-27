using System.Threading.Tasks;
using PasswordKeeper.Security;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IDatabaseProvider databaseProvider;

        public DatabaseService(IDatabaseProvider databaseProvider)
        {
            this.databaseProvider = databaseProvider.ThrowIfNull(nameof(databaseProvider));
        }

        public ICryptoDatabase CurrentDatabase { get; private set; }

        public async Task SetupAsync(string location, ICryptoStorage cryptoStorage, byte[] key)
        {
            location.ThrowIfNullOrEmpty(nameof(location));
            cryptoStorage.ThrowIfNull(nameof(cryptoStorage));
            key.ThrowIfNull(nameof(key));

            Close();
            try
            {
                CurrentDatabase = databaseProvider.GetDatabase(location);
                await CurrentDatabase.SetupAsync(cryptoStorage, key).ConfigureAwait(false);
            }
            catch
            {
                CurrentDatabase = null;
                throw;
            }
        }

        public async Task<bool> OpenAsync(string location, byte[] key)
        {
            location.ThrowIfNullOrEmpty(nameof(location));
            key.ThrowIfNull(nameof(key));

            Close();
            try
            {
                CurrentDatabase = databaseProvider.GetDatabase(location);
                var opened = await CurrentDatabase.OpenAsync(key).ConfigureAwait(false);
                if (!opened)
                {
                    Dispose();
                }

                return opened;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            CurrentDatabase?.Dispose();
            CurrentDatabase = null;
        }
    }
}