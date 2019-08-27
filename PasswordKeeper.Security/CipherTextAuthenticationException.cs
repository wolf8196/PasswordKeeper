using System;

namespace PasswordKeeper.Security
{
    public sealed class CipherTextAuthenticationException : Exception
    {
        public CipherTextAuthenticationException()
            : base("Ciphertext authentication failed. Specified key is invalid or content is corrupted")
        {
        }
    }
}