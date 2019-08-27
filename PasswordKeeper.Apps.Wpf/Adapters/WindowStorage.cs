using System.Collections.Generic;
using System.Windows;
using PasswordKeeper.Apps.Wpf.Abstractions.Adapters;

namespace PasswordKeeper.Apps.Wpf.Adapters
{
    public class WindowStorage<T> : IWindowStorage<T>
    {
        private readonly Dictionary<T, Window> registeredWindows;

        public WindowStorage()
        {
            registeredWindows = new Dictionary<T, Window>();
        }

        public IWindowAdapter GetWindow(T owner)
        {
            return new WindowAdapter(registeredWindows[owner]);
        }

        public void Register(T owner, Window window)
        {
            registeredWindows.Add(owner, window);
        }

        public void Remove(T owner)
        {
            registeredWindows.Remove(owner);
        }
    }
}