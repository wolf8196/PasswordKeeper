using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Components
{
    /// <summary>
    /// Interaction logic for BackButton.xaml
    /// </summary>
    public partial class BackButton : UserControl
    {
        static BackButton()
        {
            CommandProperty = DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(BackButton),
                new PropertyMetadata(null));
        }

        public BackButton()
        {
            InitializeComponent();
        }

        public static DependencyProperty CommandProperty { get; set; }

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
    }
}