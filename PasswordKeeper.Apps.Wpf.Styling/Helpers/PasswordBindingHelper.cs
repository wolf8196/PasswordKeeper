using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace PasswordKeeper.Apps.Wpf.Styling.Helpers
{
    public static class PasswordBindingHelper
    {
        private static readonly DependencyProperty PasswordProperty;
        private static readonly MethodInfo PassswordBoxSelectMethod;

        static PasswordBindingHelper()
        {
            PasswordProperty = DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordBindingHelper),
                new PropertyMetadata(string.Empty, OnPasswordPropertyChanged));

            PassswordBoxSelectMethod = typeof(PasswordBox).GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public static string GetPassword(DependencyObject d)
        {
            return (string)d.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject d, string value)
        {
            d.SetValue(PasswordProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox passwordBox)
            {
                passwordBox.PasswordChanged -= PasswordChanged;

                var password = (string)e.NewValue;
                passwordBox.Password = password;

                var caretPos = string.IsNullOrEmpty(password) ? 0 : password.Length;
                passwordBox.SetCaretPosition(caretPos);

                passwordBox.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                SetPassword(passwordBox, passwordBox.Password);
            }
        }

        private static void SetCaretPosition(this PasswordBox passwordBox, int start, int length = 0)
        {
            PassswordBoxSelectMethod.Invoke(passwordBox, new object[] { start, length });
        }
    }
}