using System.Windows;
using PasswordKeeper.Apps.Wpf.Abstractions.Adapters;

namespace PasswordKeeper.Apps.Wpf.Adapters
{
    public class WindowAdapter : IWindowAdapter
    {
        private readonly Window window;

        public WindowAdapter(Window window)
        {
            this.window = window;
        }

        public WindowState State
        {
            get => window.WindowState;
            set => window.WindowState = value;
        }

        public void Show()
        {
            window.Show();
        }

        public void Hide()
        {
            window.Hide();
        }

        public void Close()
        {
            window.Close();
        }

        public void Activate()
        {
            window.Activate();
        }
    }
}