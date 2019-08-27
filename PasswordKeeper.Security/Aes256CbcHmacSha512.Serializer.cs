using System.IO;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public sealed partial class Aes256CbcHmacSha512
    {
        private sealed class Serializer : ISerializer
        {
            public void Deserialize(Stream source)
            {
            }

            public void Serialize(Stream destination)
            {
            }
        }
    }
}