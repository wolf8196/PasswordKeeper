using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.Authentication
{
    /// <summary>
    /// Interaction logic for AuthenticationView.xaml
    /// </summary>
    public partial class AuthenticationView : UserControl
    {
        public AuthenticationView()
        {
            InitializeComponent();
        }

        private void AuthenticationView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(LoginTextBox);
            LoginTextBox.CaretIndex = LoginTextBox.Text.Length;
        }
    }
}