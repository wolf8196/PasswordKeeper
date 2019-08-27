using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PasswordKeeper.Apps.Wpf.Styling.Converters
{
    public class BrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;

            if (brush != null)
            {
                return brush.Color;
            }
            else
            {
                return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;

            if (color != null)
            {
                return new SolidColorBrush(color.Value);
            }
            else
            {
                return Binding.DoNothing;
            }
        }
    }
}