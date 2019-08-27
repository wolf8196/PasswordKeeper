using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Features.AccountForm
{
    /// <summary>
    /// Interaction logic for PasswordGenerationView.xaml
    /// </summary>
    public partial class PasswordGenerationView : UserControl
    {
        public PasswordGenerationView()
        {
            InitializeComponent();
        }

        private void PasswordGenerationView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Keyboard.Focus(LengthComboBox);
        }
    }
}