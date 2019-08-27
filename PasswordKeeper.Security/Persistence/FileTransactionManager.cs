using System;
using System.IO;
using System.Threading.Tasks;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security.Persistence
{
    internal sealed class FileTransactionManager : TransactionManagerBase, IDisposable
    {
        private readonly FileInfo file;
        private FileStream originalStream;
        private FileStream backupStream;
        private Guid? transactionId;

        internal FileTransactionManager(string fileName, FileStream stream)
        {
            file = new FileInfo(fileName.ThrowIfNullOrEmpty(nameof(fileName)));
            originalStream = stream.ThrowIfNull(nameof(stream));
            CurrentStream = originalStream;
        }

        internal FileStream CurrentStream { get; private set; }

        private string TempFileName => Backup.GetTempFileName(file.FullName, transactionId.ToString());

        private string BackupName => Backup.GetBackupFileName(file.FullName, transactionId.ToString());

        private string FileName { get => file.FullName; }

        public void Dispose()
        {
            if (CurrentStream != backupStream)
            {
                backupStream?.Dispose();
            }

            if (CurrentStream != originalStream)
            {
                originalStream?.Dispose();
            }
        }

        protected override async Task BeginAsync()
        {
            transactionId = Guid.NewGuid();
            backupStream = await Backup.CreateAsync(originalStream, TempFileName).ConfigureAwait(false);
            CurrentStream = backupStream;

            await base.BeginAsync().ConfigureAwait(false);
        }

        protected override Task CommitAsync()
        {
            originalStream.Dispose();
            backupStream.Dispose();

            File.Replace(TempFileName, FileName, BackupName, true);
            File.Delete(BackupName);

            originalStream = new FileStream(FileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, true);
            CurrentStream = originalStream;
            transactionId = null;

            return base.CommitAsync();
        }

        protected override Task RollbackAsync()
        {
            backupStream?.Dispose();
            File.Delete(TempFileName);
            File.Delete(BackupName);
            CurrentStream = originalStream;
            transactionId = null;

            return base.RollbackAsync();
        }
    }
}