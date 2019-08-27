using System;
using System.Collections.Generic;
using System.IO;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Security
{
    internal static class GenericSerializer
    {
        internal static ISerializable Deserialize(
            Stream source,
            Dictionary<int, Func<ISerializable>> codeToConstuctorMapping,
            string formatExceptionMessage)
        {
            source.ThrowIfNull(nameof(source));
            codeToConstuctorMapping.ThrowIfNull(nameof(codeToConstuctorMapping));
            formatExceptionMessage.ThrowIfNull(nameof(formatExceptionMessage));

            var buffer = new byte[sizeof(int)];
            source.Read(buffer, 0, buffer.Length);
            var objectKey = BitConverter.ToInt32(buffer, 0);

            if (!codeToConstuctorMapping.TryGetValue(objectKey, out Func<ISerializable> objConstructor))
            {
                throw new FormatException(formatExceptionMessage);
            }

            ISerializable obj = objConstructor();

            obj.GetSerializer().Deserialize(source);

            return obj;
        }

        internal static void Serialize(
            Stream destination,
            ISerializable obj,
            Dictionary<Type, int> typeToCodeMapping,
            string formatExceptionMessage)
        {
            destination.ThrowIfNull(nameof(destination));
            obj.ThrowIfNull(nameof(obj));
            typeToCodeMapping.ThrowIfNull(nameof(typeToCodeMapping));
            formatExceptionMessage.ThrowIfNull(nameof(formatExceptionMessage));

            if (!typeToCodeMapping.TryGetValue(obj.GetType(), out int value))
            {
                throw new FormatException(formatExceptionMessage);
            }

            var bytes = BitConverter.GetBytes(value);
            destination.Write(bytes, 0, bytes.Length);

            obj.GetSerializer().Serialize(destination);
        }
    }
}