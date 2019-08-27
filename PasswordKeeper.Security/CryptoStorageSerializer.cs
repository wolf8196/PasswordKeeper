using System;
using System.Collections.Generic;
using System.IO;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    public static partial class CryptoStorageSerializer
    {
        private const string FormatExceptionMessage = "Invalid crypto storage or crypto storage is not supported";

        private static readonly Dictionary<Type, int> TypeToEnumMappings = new Dictionary<Type, int>
        {
            { typeof(CryptoStorage), (int)CryptoStorageVersions.V1 }
        };

        private static readonly Dictionary<int, Func<ISerializable>> EnumToConstuctorMappings = new Dictionary<int, Func<ISerializable>>
        {
            { (int)CryptoStorageVersions.V1, () => new CryptoStorage() }
        };

        internal static ICryptoStorage Deserialize(Stream source)
        {
            source.ThrowIfNull(nameof(source));
            return (ICryptoStorage)GenericSerializer.Deserialize(source, EnumToConstuctorMappings, FormatExceptionMessage);
        }

        internal static void Serialize(Stream destination, ICryptoStorage obj)
        {
            destination.ThrowIfNull(nameof(destination));
            obj.ThrowIfNull(nameof(obj));
            GenericSerializer.Serialize(destination, obj, TypeToEnumMappings, FormatExceptionMessage);
        }
    }
}