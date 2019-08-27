using System.Windows.Controls;

namespace PasswordKeeper.Apps.Wpf.Features.SecuritySettings
{
    /// <summary>
    /// Interaction logic for SecuritySettingsView.xaml
    /// </summary>
    public partial class SecuritySettingsView : UserControl
    {
        public SecuritySettingsView()
        {
            InitializeComponent();
        }

        private void SecuritySettingsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Focus();
        }
    }
}