using System.ComponentModel;

namespace PasswordKeeper.Services.Enums
{
    public enum KeyTransformations
    {
        [Description("PBKDF2-SHA512")]
        Pbkdf2 = 1
    }
}