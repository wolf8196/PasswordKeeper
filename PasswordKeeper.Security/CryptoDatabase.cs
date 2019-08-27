using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using PasswordKeeper.Security.Persistence;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public sealed class CryptoDatabase : ICryptoDatabase
    {
        private ICryptoStorage storage;
        private CompositeTransactionManager compositeTransactionManager;

        public CryptoDatabase(string fileName)
        {
            FileName = fileName.ThrowIfNull(nameof(fileName));
        }

        public string FileName { get; private set; }

        public bool IsOpened
        {
            get => storage?.IsOpened ?? false;
        }

        public IEnumerable<string> Content
        {
            get => IsOpened ? storage.Content : null;
        }

        public async Task SetupAsync(ICryptoStorage cryptoStorage, byte[] key)
        {
            storage = cryptoStorage.ThrowIfNull(nameof(cryptoStorage));

            var storageFileStream = new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, true);
            cryptoStorage.DataSource = storageFileStream;

            CryptoStorageSerializer.Serialize(cryptoStorage.DataSource, cryptoStorage);
            await cryptoStorage.SetupAsync(key).ConfigureAwait(false);
        }

        public async Task ResetupAsync(ICryptoStorage cryptoStorage, byte[] key)
        {
            Authorize();

            var id = Guid.NewGuid();
            var tempFileName = Backup.GetTempFileName(FileName, id.ToString());
            var backupFileName = Backup.GetBackupFileName(FileName, id.ToString());
            var originalFileName = FileName;

            // Setup new database in a temp file
            var newDb = new CryptoDatabase(tempFileName);
            await newDb.SetupAsync(cryptoStorage, key).ConfigureAwait(false);

            // Copy contents to new database
            foreach (var item in Content)
            {
                var buffer = new MemoryStream();
                await storage.GetAsync(item, buffer).ConfigureAwait(false);
                buffer.Seek(0, SeekOrigin.Begin);
                await newDb.storage.AddAsync(item, buffer).ConfigureAwait(false);
            }

            // Save changes & dispose both databases
            await newDb.storage.SaveAsync().ConfigureAwait(false);
            newDb.Dispose();
            Dispose();

            // Replace old database file with new one
            File.Replace(tempFileName, originalFileName, backupFileName, true);
            File.Delete(backupFileName);

            // Open new database
            FileName = originalFileName;
            await OpenAsync(key).ConfigureAwait(false);
        }

        public Task<bool> OpenAsync(byte[] key)
        {
            if (storage == null)
            {
                FileStream storageFileStream = null;
                try
                {
                    storageFileStream = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, true);
                    storage = CryptoStorageSerializer.Deserialize(storageFileStream);
                    storage.DataSource = storageFileStream;
                }
                catch
                {
                    storageFileStream?.Dispose();
                    storage = null;
                }
            }

            return storage.OpenAsync(key);
        }

        public async Task GetAsync(string id, string destinationFile)
        {
            using (var destination = new FileStream(destinationFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None, 4096, true))
            {
                await GetAsync(id, destination).ConfigureAwait(false);
            }
        }

        public async Task GetAsync(string id, Stream destination)
        {
            Authorize();

            await storage.GetAsync(id, destination).ConfigureAwait(false);
        }

        public async Task AddAsync(string id, string sourceFile)
        {
            await AddAsyncInternal(
                async () =>
                {
                    using (var src = new FileStream(sourceFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None, 4096, true))
                    {
                        await storage.AddAsync(id, src).ConfigureAwait(false);
                    }
                });
        }

        public async Task AddAsync(string id, Stream source)
        {
            await AddAsyncInternal(() => storage.AddAsync(id, source)).ConfigureAwait(false);
        }

        public async Task RemoveAsync(string id)
        {
            Authorize();

            try
            {
                await BeginTransactionAsync().ConfigureAwait(false);

                await storage.RemoveAsync(id).ConfigureAwait(false);
            }
            catch
            {
                await RollbackTransactionAsync().ConfigureAwait(false);
                throw;
            }
        }

        public async Task SaveAsync()
        {
            Authorize();

            try
            {
                await BeginTransactionAsync().ConfigureAwait(false);

                await storage.SaveAsync().ConfigureAwait(false);

                await CommitTransactionAsync().ConfigureAwait(false);
            }
            catch
            {
                await RollbackTransactionAsync().ConfigureAwait(false);
                throw;
            }
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            compositeTransactionManager?.Dispose();
            compositeTransactionManager = null;
            storage?.DataSource.Dispose();
            storage?.Dispose();
            storage = null;
            FileName = null;
        }

        private async Task AddAsyncInternal(Func<Task> func)
        {
            Authorize();

            try
            {
                await BeginTransactionAsync().ConfigureAwait(false);

                await func().ConfigureAwait(false);
            }
            catch
            {
                await RollbackTransactionAsync().ConfigureAwait(false);
                throw;
            }
        }

        private void InitializeTransactionManager()
        {
            var fileTransactionManager = new FileTransactionManager(FileName, (FileStream)storage.DataSource);
            var stateTransactionManager = new StateTransactionManager(storage);

            compositeTransactionManager = new CompositeTransactionManager(
                new List<TransactionManagerBase> { fileTransactionManager, stateTransactionManager });

            void handler() => storage.DataSource = fileTransactionManager.CurrentStream;
            compositeTransactionManager.OnBegan += handler;
            compositeTransactionManager.OnCommmited += handler;
            compositeTransactionManager.OnRollbacked += handler;
        }

        private async Task BeginTransactionAsync()
        {
            if (compositeTransactionManager == null)
            {
                InitializeTransactionManager();
            }

            await compositeTransactionManager.BeginTransactionAsync().ConfigureAwait(false);
        }

        private async Task CommitTransactionAsync()
        {
            await compositeTransactionManager.CommitTransactionAsync().ConfigureAwait(false);
        }

        private async Task RollbackTransactionAsync()
        {
            await compositeTransactionManager.RollbackTransactionAsync().ConfigureAwait(false);
        }

        private void Authorize()
        {
            if (!IsOpened)
            {
                throw new InvalidOperationException("Database is closed. Call OpenAsync first");
            }
        }
    }
}