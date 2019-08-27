using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public interface IKeyDerivationFunction : ISerializable
    {
        byte[] Generate(byte[] key, int keySize);
    }
}