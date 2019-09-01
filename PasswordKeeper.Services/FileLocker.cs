using System.Collections.Generic;
using System.IO;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services
{
    public sealed class FileLocker : ILocker
    {
        private const string LocksDirectoryName = "locks";

        private readonly Dictionary<string, FileStream> locks;

        public FileLocker()
        {
            locks = new Dictionary<string, FileStream>();
        }

        public bool IsLocked(string fileName)
        {
            fileName.ThrowIfNullOrEmpty(nameof(fileName));
            var file = new FileInfo(fileName);
            return File.Exists(GetLockFileName(file));
        }

        public void Lock(string fileName)
        {
            fileName.ThrowIfNullOrEmpty(nameof(fileName));
            var file = new FileInfo(fileName);
            Directory.CreateDirectory(GetLocksDirectory(file));

            if (locks.ContainsKey(fileName))
            {
                return;
            }

            locks.Add(
                fileName,
                new FileStream(
                    Path.Combine(GetLockFileName(file)),
                    FileMode.OpenOrCreate,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    4096,
                    FileOptions.DeleteOnClose));
        }

        public void Unlock(string fileName)
        {
            fileName.ThrowIfNullOrEmpty(nameof(fileName));
            if (locks.TryGetValue(fileName, out FileStream lockFile))
            {
                lockFile.Dispose();
            }

            locks.Remove(fileName);
        }

        public void Dispose()
        {
            foreach (var item in locks)
            {
                item.Value.Dispose();
            }

            locks.Clear();
        }

        private static string GetLocksDirectory(FileInfo file)
        {
            return Path.Combine(file.DirectoryName, LocksDirectoryName);
        }

        private static string GetLockFileName(FileInfo file)
        {
            return Path.Combine(file.DirectoryName, LocksDirectoryName, $"{file.Name}.lock");
        }
    }
}