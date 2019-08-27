using System;
using System.Globalization;
using System.Windows.Data;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Apps.Wpf.Styling.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as Enum)?.GetDescription(value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}