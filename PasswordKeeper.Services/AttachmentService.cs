using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PasswordKeeper.Services.Abstractions;
using PasswordKeeper.Services.Entities;
using PasswordKeeper.Services.Exceptions;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IUserService userService;
        private readonly IDatabaseService databaseService;

        public AttachmentService(IUserService userService, IDatabaseService databaseService)
        {
            this.userService = userService.ThrowIfNull(nameof(userService));
            this.databaseService = databaseService.ThrowIfNull(nameof(databaseService));
        }

        public string GetExtractPath(string path)
        {
            var newFullPath = path;

            var count = 1;
            var fileName = Path.GetFileNameWithoutExtension(newFullPath);
            var extension = Path.GetExtension(newFullPath);
            var directory = Path.GetDirectoryName(newFullPath);

            while (File.Exists(newFullPath))
            {
                newFullPath = Path.Combine(directory, string.Format($"{fileName} ({++count}){extension}"));
            }

            return newFullPath;
        }

        public async Task GetAsync(Attachment attachment, string destination)
        {
            attachment.ThrowIfNull(nameof(attachment));
            destination.ThrowIfNullOrEmpty(nameof(destination));

            await databaseService.CurrentDatabase.GetAsync(attachment.Id.ToString(), destination).ConfigureAwait(false);
        }

        public async Task AddAsync(Account account, string file)
        {
            account.ThrowIfNull(nameof(account));
            file.ThrowIfNullOrEmpty(nameof(file));

            var newAttachment = Map(file);

            if (account.Attachments.Any(x => x.Name == newAttachment.Name))
            {
                throw new AttachmentExistsException(newAttachment.Name);
            }

            var memento = account.GetMemento();
            try
            {
                await userService.BeginSaveAsync().ConfigureAwait(false);

                account.Add(newAttachment);

                await databaseService.CurrentDatabase.AddAsync(newAttachment.Id.ToString(), file).ConfigureAwait(false);

                await userService.EndSaveAsync().ConfigureAwait(false);
            }
            catch
            {
                memento.Restore();
                throw;
            }
        }

        public async Task ReplaceAsync(Account account, string file)
        {
            account.ThrowIfNull(nameof(account));
            file.ThrowIfNullOrEmpty(nameof(file));

            var newAttachment = Map(file);
            var oldAttachment = account.Attachments.First(x => x.Name == newAttachment.Name);

            var memento = account.GetMemento();
            try
            {
                await userService.BeginSaveAsync().ConfigureAwait(false);

                await databaseService.CurrentDatabase.RemoveAsync(oldAttachment.Id.ToString()).ConfigureAwait(false);
                account.Remove(oldAttachment);

                account.Add(newAttachment);
                await databaseService.CurrentDatabase.AddAsync(newAttachment.Id.ToString(), file).ConfigureAwait(false);

                await userService.EndSaveAsync().ConfigureAwait(false);
            }
            catch
            {
                memento.Restore();
                throw;
            }
        }

        public async Task RemoveAsync(Account account, Attachment attachment)
        {
            account.ThrowIfNull(nameof(account));
            attachment.ThrowIfNull(nameof(attachment));

            var memento = account.GetMemento();
            try
            {
                await userService.BeginSaveAsync().ConfigureAwait(false);

                await databaseService.CurrentDatabase.RemoveAsync(attachment.Id.ToString()).ConfigureAwait(false);
                account.Remove(attachment);

                await userService.EndSaveAsync().ConfigureAwait(false);
            }
            catch
            {
                memento.Restore();
                throw;
            }
        }

        public async Task RemoveAllAsync(Account account)
        {
            account.ThrowIfNull(nameof(account));

            var memento = account.GetMemento();
            try
            {
                await userService.BeginSaveAsync().ConfigureAwait(false);

                foreach (var attachment in account.Attachments)
                {
                    await databaseService.CurrentDatabase.RemoveAsync(attachment.Id.ToString()).ConfigureAwait(false);
                }

                account.RemoveAllAttachments();

                await userService.EndSaveAsync().ConfigureAwait(false);
            }
            catch
            {
                memento.Restore();
                throw;
            }
        }

        private static Attachment Map(string file)
        {
            return new Attachment
            {
                Id = Guid.NewGuid(),
                Name = Path.GetFileName(file)
            };
        }
    }
}