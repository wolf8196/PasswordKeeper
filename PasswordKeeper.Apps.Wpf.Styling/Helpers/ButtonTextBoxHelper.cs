using System.Windows;
using System.Windows.Controls;

namespace PasswordKeeper.Apps.Wpf.Styling.Helpers
{
    public static class ButtonTextBoxHelper
    {
        public static readonly DependencyProperty IsButtonEnabledProperty;

        static ButtonTextBoxHelper()
        {
            IsButtonEnabledProperty = DependencyProperty.RegisterAttached(
                "IsButtonEnabled",
                typeof(bool),
                typeof(ButtonTextBoxHelper),
                new PropertyMetadata(false));
        }

        public static bool GetIsButtonEnabled(TextBox element)
        {
            return (bool)element.GetValue(IsButtonEnabledProperty);
        }

        public static void SetIsButtonEnabled(TextBox element, bool value)
        {
            element.SetValue(IsButtonEnabledProperty, value);
        }
    }
}