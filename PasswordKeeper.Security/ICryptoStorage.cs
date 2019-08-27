using System.IO;
using System.Threading.Tasks;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public interface ICryptoStorage : IStorage, IOpenable, ISerializable, IOriginator
    {
        Stream DataSource { get; set; }

        Task SetupAsync(byte[] key);
    }
}