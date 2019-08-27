using System.Windows;

namespace PasswordKeeper.Apps.Wpf.Styling.Helpers
{
    public class BindingProxy : Freezable
    {
        public static readonly DependencyProperty DataProperty;

        static BindingProxy()
        {
            DataProperty = DependencyProperty.Register(
                nameof(Data),
                typeof(object),
                typeof(BindingProxy),
                new UIPropertyMetadata(null));
        }

        public object Data
        {
            get
            {
                return GetValue(DataProperty);
            }

            set
            {
                SetValue(DataProperty, value);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }
    }
}