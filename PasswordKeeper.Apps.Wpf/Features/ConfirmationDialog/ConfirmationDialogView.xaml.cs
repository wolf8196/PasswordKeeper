using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.ConfirmationDialog
{
    /// <summary>
    /// Interaction logic for ConfirmationDialogView.xaml
    /// </summary>
    public partial class ConfirmationDialogView : UserControl
    {
        public ConfirmationDialogView()
        {
            InitializeComponent();
        }

        private void ConfirmationDialogView_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
        }

        private void ConfirmationDialogView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }
    }
}