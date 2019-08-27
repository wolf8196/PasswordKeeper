using System.ComponentModel;
using MahApps.Metro.Controls;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Abstractions.Adapters;

namespace PasswordKeeper.Apps.Wpf.Features.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly MainViewModel viewModel;

        public MainWindow(MainViewModel viewModel, IWindowStorage<ViewModelBase> windowStorage)
        {
            InitializeComponent();

            var subscribable = viewModel as ISubscribable;
            subscribable?.Subscribe();

            DataContext = viewModel;

            windowStorage.Register(viewModel, this);

            this.viewModel = viewModel;
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            viewModel.CloseCommand.Execute(e);
        }
    }
}