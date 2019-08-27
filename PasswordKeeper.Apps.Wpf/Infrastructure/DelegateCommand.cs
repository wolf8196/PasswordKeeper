using System;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Infrastructure
{
    public class DelegateCommand : ICommand
    {
        private readonly Predicate<object> canExecuteDelegate;
        private readonly Action<object> executeDelegate;

        public DelegateCommand(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
        {
            this.executeDelegate = executeDelegate;
            this.canExecuteDelegate = canExecuteDelegate;
        }

        public DelegateCommand(Action<object> executeDelegate)
            : this(executeDelegate, null)
        {
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public bool CanExecute(object parameter = null)
        {
            return canExecuteDelegate?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter = null)
        {
            executeDelegate?.Invoke(parameter);
        }
    }
}