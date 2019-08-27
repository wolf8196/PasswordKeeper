using System.IO;
using System.Threading.Tasks;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public interface IEncryptor : ISerializable
    {
        int KeySize { get; }

        Task DecryptAsync(Stream source, Stream destination, byte[] key);

        Task EncryptAsync(Stream source, Stream destination, byte[] key);
    }
}