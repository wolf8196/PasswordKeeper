using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.Registration
{
    /// <summary>
    /// Interaction logic for RegistrationUserControl.xaml
    /// </summary>
    public partial class RegistrationView : UserControl
    {
        public RegistrationView()
        {
            InitializeComponent();
        }

        private void RegistrationView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(LoginTextBox);
            LoginTextBox.CaretIndex = LoginTextBox.Text.Length;
        }
    }
}