using System.Windows.Controls;

namespace PasswordKeeper.Apps.Wpf.Features.UserSettings
{
    /// <summary>
    /// Interaction logic for SettingsUserControl.xaml
    /// </summary>
    public partial class UserSettingsView : UserControl
    {
        public UserSettingsView()
        {
            InitializeComponent();
        }

        private void UserSettingsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Focus();
        }
    }
}