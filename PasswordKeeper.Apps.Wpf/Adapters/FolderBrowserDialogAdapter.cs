using System.Windows.Forms;
using PasswordKeeper.Apps.Wpf.Abstractions.Adapters;

namespace PasswordKeeper.Apps.Wpf.Adapters
{
    public class FolderBrowserDialogAdapter : IFolderBrowserDialogAdapter
    {
        public string ShowDialog()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
            }
        }
    }
}