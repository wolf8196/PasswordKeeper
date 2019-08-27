using System.Windows;
using System.Windows.Interactivity;

namespace PasswordKeeper.Apps.Wpf.Styling.Helpers
{
    public class TopmostBindingHelper : Behavior<Window>
    {
        public static readonly DependencyProperty ValueProperty;

        static TopmostBindingHelper()
        {
            ValueProperty = DependencyProperty.RegisterAttached(
                nameof(Value),
                typeof(bool),
                typeof(TopmostBindingHelper),
                new PropertyMetadata(false, OnValueChanged));
        }

        public bool Value
        {
            get
            {
                return (bool)AssociatedObject.GetValue(ValueProperty);
            }

            set
            {
                AssociatedObject.SetValue(ValueProperty, value);
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = (d as TopmostBindingHelper).AssociatedObject;
            if (window != null && e.NewValue != e.OldValue)
            {
                window.Topmost = (bool)e.NewValue;
            }
        }
    }
}