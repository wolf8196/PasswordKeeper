using System;
using System.ComponentModel;
using System.Linq;

namespace PasswordKeeper.Utils
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T value) where T : Enum
        {
            var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(typeof(T).GetMember(value.ToString()).First(), typeof(DescriptionAttribute));

            return attr != null ? attr.Description : value.ToString();
        }

        public static string GetDescription<T>(this T value, Type type) where T : Enum
        {
            var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(type.GetMember(value.ToString()).First(), typeof(DescriptionAttribute));

            return attr != null ? attr.Description : value.ToString();
        }
    }
}