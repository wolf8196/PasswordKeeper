using System.ComponentModel;

namespace PasswordKeeper.Services.Enums
{
    public enum EncryptionAlgorithms
    {
        [Description("AES-256-CBC-PKCS7-HMACSHA-512")]
        Aes256CbcHmacSha512 = 1
    }
}