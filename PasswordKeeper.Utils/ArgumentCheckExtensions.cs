using System;

namespace PasswordKeeper.Utils
{
    public static class ArgumentCheckExtensions
    {
        public static T ThrowIfNull<T>(this T obj, string paramName) where T : class
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return obj;
        }

        public static T ThrowIfDefault<T>(this T obj, string paramName) where T : struct
        {
            if (obj.Equals(default(T)))
            {
                throw new ArgumentException("Parameter cannot be set to its default value", paramName);
            }

            return obj;
        }

        public static string ThrowIfNullOrEmpty(this string obj, string paramName)
        {
            if (string.IsNullOrWhiteSpace(obj))
            {
                throw new ArgumentException("Parameter cannot be set to its null or empty string", paramName);
            }

            return obj;
        }
    }
}