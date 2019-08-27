namespace PasswordKeeper.Services.Exceptions
{
    public class UserExistsException : UserException
    {
        public UserExistsException(string userName)
            : base("User already exists", userName)
        {
        }
    }
}