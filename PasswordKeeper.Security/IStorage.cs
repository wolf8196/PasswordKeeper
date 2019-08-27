using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PasswordKeeper.Security
{
    public interface IStorage
    {
        IEnumerable<string> Content { get; }

        Task GetAsync(string id, Stream destination);

        Task AddAsync(string id, Stream source);

        Task RemoveAsync(string id);

        Task SaveAsync();
    }
}