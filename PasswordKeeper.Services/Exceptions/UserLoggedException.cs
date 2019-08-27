namespace PasswordKeeper.Services.Exceptions
{
    public class UserLoggedException : UserException
    {
        public UserLoggedException(string userName)
            : base("User already logged", userName)
        {
        }
    }
}