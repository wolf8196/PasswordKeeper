namespace PasswordKeeper.Services.Abstractions
{
    public interface IUserManager
    {
        bool UserExists(string userName);

        string GetUserLocation(string userName);

        bool IsUserLoggedIn(string userName);

        void MarkUserLoggedIn(string userName);

        void MarkUserLoggedOut(string userName);

        string GetUserNameByLocation(string location);
    }
}