using System.IO;
using System.Text;
using Newtonsoft.Json;
using PasswordKeeper.Services.Entities;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services
{
    public sealed class UserSerializer : ISerializer<User>
    {
        public User Deserialize(Stream source)
        {
            source.ThrowIfNull(nameof(source));

            using (var streamReader = new StreamReader(source))
            {
                return (User)JsonSerializer.CreateDefault().Deserialize(streamReader, typeof(User));
            }
        }

        public void Serialize(Stream destination, User obj)
        {
            destination.ThrowIfNull(nameof(destination));
            obj.ThrowIfNull(nameof(obj));

            using (var streamWriter = new StreamWriter(destination, Encoding.UTF8, 1024, true))
            {
                JsonSerializer.CreateDefault().Serialize(streamWriter, obj);
            }
        }
    }
}