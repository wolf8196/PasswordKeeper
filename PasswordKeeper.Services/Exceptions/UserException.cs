using System;

namespace PasswordKeeper.Services.Exceptions
{
    public class UserException : Exception
    {
        public UserException(string message, string userName)
            : base(message)
        {
            UserName = userName;
        }

        public string UserName { get; set; }
    }
}