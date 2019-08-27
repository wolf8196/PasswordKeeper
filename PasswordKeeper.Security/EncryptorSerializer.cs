using System;
using System.Collections.Generic;
using System.IO;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public static partial class EncryptorSerializer
    {
        private const string FormatExceptionMessage = "Invalid encryptor or encryptor is not supported";

        private static readonly Dictionary<Type, int> TypeToEnumMappings = new Dictionary<Type, int>
        {
            { typeof(Aes256CbcHmacSha512), (int)Encryptors.Aes256CbcHmacSha512 }
        };

        private static readonly Dictionary<int, Func<ISerializable>> EnumToConstuctorMappings = new Dictionary<int, Func<ISerializable>>
        {
            { (int)Encryptors.Aes256CbcHmacSha512, () => new Aes256CbcHmacSha512() }
        };

        internal static IEncryptor Deserialize(Stream source)
        {
            source.ThrowIfNull(nameof(source));
            return (IEncryptor)GenericSerializer.Deserialize(source, EnumToConstuctorMappings, FormatExceptionMessage);
        }

        internal static void Serialize(Stream destination, IEncryptor obj)
        {
            destination.ThrowIfNull(nameof(destination));
            obj.ThrowIfNull(nameof(obj));
            GenericSerializer.Serialize(destination, obj, TypeToEnumMappings, FormatExceptionMessage);
        }
    }
}