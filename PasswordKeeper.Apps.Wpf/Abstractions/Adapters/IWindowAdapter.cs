using System.Windows;

namespace PasswordKeeper.Apps.Wpf.Abstractions.Adapters
{
    public interface IWindowAdapter
    {
        WindowState State { get; set; }

        void Hide();

        void Show();

        void Close();

        void Activate();
    }
}