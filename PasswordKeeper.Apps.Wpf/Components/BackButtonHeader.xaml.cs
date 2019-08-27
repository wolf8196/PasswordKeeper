using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Components
{
    /// <summary>
    /// Interaction logic for BackButtonHeader.xaml
    /// </summary>
    public partial class BackButtonHeader : UserControl
    {
        static BackButtonHeader()
        {
            TextProperty = DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(BackButtonHeader),
                new PropertyMetadata(null));

            CommandProperty = DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(BackButtonHeader),
                new PropertyMetadata(null));
        }

        public BackButtonHeader()
        {
            InitializeComponent();
        }

        public static DependencyProperty CommandProperty { get; set; }

        public static DependencyProperty TextProperty { get; set; }

        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
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