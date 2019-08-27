using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.LoadingIndicator
{
    /// <summary>
    /// Interaction logic for LoadingIndicatorView.xaml
    /// </summary>
    public partial class LoadingIndicatorView : UserControl
    {
        public LoadingIndicatorView()
        {
            InitializeComponent();
        }

        private void LoadingIndicatorView_Loaded(object sender, RoutedEventArgs e)
        {
            Focus();
        }

        private void LoadingIndicatorView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }
    }
}