namespace PasswordKeeper.Services.Exceptions
{
    public class UserNotFoundException : UserException
    {
        public UserNotFoundException(string userName)
            : base("User not found", userName)
        {
        }
    }
}