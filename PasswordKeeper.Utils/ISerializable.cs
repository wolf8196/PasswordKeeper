namespace PasswordKeeper.Utils
{
    public interface ISerializable
    {
        ISerializer GetSerializer();
    }
}