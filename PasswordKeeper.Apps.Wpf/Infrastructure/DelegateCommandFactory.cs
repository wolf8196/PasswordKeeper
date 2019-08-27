using System;
using System.Windows.Input;
using PasswordKeeper.Apps.Wpf.Abstractions;

namespace PasswordKeeper.Apps.Wpf.Infrastructure
{
    public class DelegateCommandFactory : ICommandFactory
    {
        public ICommand Create(Action<object> executeDelegate, Predicate<object> canExecuteDelegate)
        {
            return new DelegateCommand(executeDelegate, canExecuteDelegate);
        }

        public ICommand Create(Action<object> executeDelegate)
        {
            return new DelegateCommand(executeDelegate);
        }
    }
}