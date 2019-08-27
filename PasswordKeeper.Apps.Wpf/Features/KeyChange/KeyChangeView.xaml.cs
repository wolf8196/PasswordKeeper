using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.KeyChange
{
    /// <summary>
    /// Interaction logic for KeyChangeView.xaml
    /// </summary>
    public partial class KeyChangeView : UserControl
    {
        public KeyChangeView()
        {
            InitializeComponent();
        }

        private void KeyChangeView_Loaded(object sender, RoutedEventArgs e)
        {
            Keyboard.Focus(NewPasswordBox);
        }
    }
}