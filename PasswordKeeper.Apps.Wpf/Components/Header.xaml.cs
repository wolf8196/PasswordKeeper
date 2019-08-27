using System.Windows;
using System.Windows.Controls;

namespace PasswordKeeper.Apps.Wpf.Components
{
    /// <summary>
    /// Interaction logic for Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        static Header()
        {
            IconProperty = DependencyProperty.Register(
                nameof(Icon),
                typeof(UIElement),
                typeof(Header),
                new PropertyMetadata(null));

            TextProperty = DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(Header),
                new PropertyMetadata(null));
        }

        public Header()
        {
            InitializeComponent();
        }

        public static DependencyProperty IconProperty { get; set; }

        public static DependencyProperty TextProperty { get; set; }

        public UIElement Icon
        {
            get
            {
                return (UIElement)GetValue(IconProperty);
            }

            set
            {
                SetValue(IconProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }

            set
            {
                SetValue(TextProperty, value);
            }
        }
    }
}