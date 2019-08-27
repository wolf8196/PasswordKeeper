using System.Threading.Tasks;
using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Services.Abstractions
{
    public interface IAttachmentService
    {
        string GetExtractPath(string path);

        Task GetAsync(Attachment attachment, string destination);

        Task AddAsync(Account account, string file);

        Task ReplaceAsync(Account account, string file);

        Task RemoveAsync(Account account, Attachment attachment);

        Task RemoveAllAsync(Account account);
    }
}