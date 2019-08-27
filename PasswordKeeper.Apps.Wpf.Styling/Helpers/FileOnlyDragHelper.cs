using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace PasswordKeeper.Apps.Wpf.Styling.Helpers
{
    public class FileOnlyDragHelper : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.DragEnter += Handler;
            AssociatedObject.DragOver += Handler;

            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.DragEnter -= Handler;
            AssociatedObject.DragOver -= Handler;

            base.OnDetaching();
        }

        private static void Handler(object sender, DragEventArgs args)
        {
            bool dropEnabled = true;
            if (args.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var filesOrDirectories = args.Data.GetData(DataFormats.FileDrop) as string[];

                if (!filesOrDirectories.Select(x => File.Exists(x)).All(x => x))
                {
                    dropEnabled = false;
                }
            }
            else
            {
                dropEnabled = false;
            }

            if (!dropEnabled)
            {
                args.Effects = DragDropEffects.None;
                args.Handled = true;
            }
        }
    }
}