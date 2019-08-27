using System.IO;
using System.Threading.Tasks;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security.Persistence
{
    internal static class Backup
    {
        internal static async Task<FileStream> CreateAsync(FileStream originalStream, string resultFileName)
        {
            originalStream.ThrowIfNull(nameof(originalStream));
            resultFileName.ThrowIfNullOrEmpty(nameof(resultFileName));

            var originalPosition = originalStream.Position;
            originalStream.Seek(0, SeekOrigin.Begin);

            var backupStream = new FileStream(resultFileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, true);

            await originalStream.CopyToAsync(backupStream).ConfigureAwait(false);
            await originalStream.FlushAsync().ConfigureAwait(false);

            originalStream.Seek(originalPosition, SeekOrigin.Begin);
            backupStream.Seek(originalPosition, SeekOrigin.Begin);

            return backupStream;
        }

        internal static string GetTempFileName(string fileName, string uniqueIdentified)
        {
            return $"{fileName}.{uniqueIdentified}.temp";
        }

        internal static string GetBackupFileName(string fileName, string uniqueIdentified)
        {
            return $"{fileName}.{uniqueIdentified}.bak";
        }
    }
}