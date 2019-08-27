using System.IO;

namespace PasswordKeeper.Utils
{
    public interface ISerializer
    {
        void Serialize(Stream destination);

        void Deserialize(Stream source);
    }
}