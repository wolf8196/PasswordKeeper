using System;
using System.Collections.Generic;
using System.IO;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public static partial class KeyDerivationFunctionSerializer
    {
        private const string FormatExceptionMessage = "Invalid key derivation function or key derivation function is not supported";

        private static readonly Dictionary<Type, int> TypeToEnumMappings = new Dictionary<Type, int>
        {
            { typeof(Pbkdf2Sha512), (int)KeyDerivationFunctions.Pbkdf2Sha512 }
        };

        private static readonly Dictionary<int, Func<ISerializable>> EnumToConstuctorMappings = new Dictionary<int, Func<ISerializable>>
        {
            { (int)KeyDerivationFunctions.Pbkdf2Sha512, () => new Pbkdf2Sha512() }
        };

        internal static IKeyDerivationFunction Deserialize(Stream source)
        {
            source.ThrowIfNull(nameof(source));
            return (IKeyDerivationFunction)GenericSerializer.Deserialize(source, EnumToConstuctorMappings, FormatExceptionMessage);
        }

        internal static void Serialize(Stream destination, IKeyDerivationFunction obj)
        {
            destination.ThrowIfNull(nameof(destination));
            obj.ThrowIfNull(nameof(obj));
            GenericSerializer.Serialize(destination, obj, TypeToEnumMappings, FormatExceptionMessage);
        }
    }
}