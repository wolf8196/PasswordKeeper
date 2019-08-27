using System;

namespace PasswordKeeper.Services.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException()
            : base("Authenticatication failed")
        {
        }
    }
}