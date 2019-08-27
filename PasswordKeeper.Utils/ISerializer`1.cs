using System.IO;

namespace PasswordKeeper.Utils
{
    public interface ISerializer<T>
    {
        void Serialize(Stream destination, T obj);

        T Deserialize(Stream source);
    }
}