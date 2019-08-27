using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.Accounts
{
    /// <summary>
    /// Interaction logic for AccountsView.xaml
    /// </summary>
    public partial class AccountsView : UserControl
    {
        public AccountsView()
        {
            InitializeComponent();
        }

        private void AccountsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(SearchTextBox);
            SearchTextBox.CaretIndex = SearchTextBox.Text.Length;
        }

        private void AccountsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AccountsListBox.ScrollIntoView(AccountsListBox.SelectedItem);
        }
    }
}