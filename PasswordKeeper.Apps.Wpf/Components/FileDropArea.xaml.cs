using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Components
{
    /// <summary>
    /// Interaction logic for FileDropArea.xaml
    /// </summary>
    public partial class FileDropArea : UserControl
    {
        static FileDropArea()
        {
            DropCommandProperty = DependencyProperty.Register(
                nameof(DropCommand),
                typeof(ICommand),
                typeof(FileDropArea),
                new PropertyMetadata(null));
        }

        public FileDropArea()
        {
            InitializeComponent();
        }

        public static DependencyProperty DropCommandProperty { get; set; }

        public ICommand DropCommand
        {
            get
            {
                return (ICommand)GetValue(DropCommandProperty);
            }

            set
            {
                SetValue(DropCommandProperty, value);
            }
        }

        private void FileDropArea_Drop(object sender, DragEventArgs e)
        {
            if (DropCommand != null)
            {
                var filesOrDirectories = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (DropCommand.CanExecute(filesOrDirectories))
                {
                    DropCommand.Execute(filesOrDirectories);
                }
            }
        }
    }
}