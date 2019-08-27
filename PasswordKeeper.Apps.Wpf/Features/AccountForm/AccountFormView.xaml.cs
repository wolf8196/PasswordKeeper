using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.AccountForm
{
    /// <summary>
    /// Interaction logic for AccountFormView.xaml
    /// </summary>
    public partial class AccountFormView : UserControl
    {
        public AccountFormView()
        {
            InitializeComponent();
        }

        private void AccountFormView_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(AccountNameTextBox);
            AccountNameTextBox.CaretIndex = AccountNameTextBox.Text.Length;
            LoginTextBox.CaretIndex = LoginTextBox.Text.Length;
            PasswordTextBox.CaretIndex = PasswordTextBox.Text.Length;
            NotesTextBox.CaretIndex = NotesTextBox.Text.Length;
        }
    }
}