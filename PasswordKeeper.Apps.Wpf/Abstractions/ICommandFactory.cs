using System;
using System.Windows.Input;

namespace PasswordKeeper.Apps.Wpf.Abstractions
{
    public interface ICommandFactory
    {
        ICommand Create(Action<object> executeDelegate, Predicate<object> canExecuteDelegate);

        ICommand Create(Action<object> executeDelegate);
    }
}