using System;
using System.Windows;
using System.Windows.Threading;
using PasswordKeeper.Apps.Wpf.Features.Main;
using PasswordKeeper.Apps.Wpf.Infrastructure;

namespace PasswordKeeper.Apps.Wpf
{
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var app = new App();
            var container = Bootstrapper.RegisterBindings();
            var mainWindow = container.GetInstance<MainWindow>();

            app.DispatcherUnhandledException += App_DispatcherUnhandledException;
            app.InitializeComponent();
            app.Run(mainWindow);
        }

        private static void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("Unhandled exception occured" + Environment.NewLine + e.Exception.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            Application.Current.Shutdown();
        }
    }
}