using System.Windows;

namespace PasswordKeeper.Apps.Wpf.Abstractions.Adapters
{
    public interface IWindowStorage<T>
    {
        IWindowAdapter GetWindow(T owner);

        void Register(T owner, Window window);

        void Remove(T owner);
    }
}