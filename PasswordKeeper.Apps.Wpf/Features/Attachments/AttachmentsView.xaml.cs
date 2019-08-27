using System.Windows.Controls;

namespace PasswordKeeper.Apps.Wpf.Features.Attachments
{
    /// <summary>
    /// Interaction logic for AttachmentsView.xaml
    /// </summary>
    public partial class AttachmentsView : UserControl
    {
        public AttachmentsView()
        {
            InitializeComponent();
        }

        private void AttachmentsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Focus();
        }
    }
}