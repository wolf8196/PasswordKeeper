using System;
using PasswordKeeper.Apps.Wpf.Features;

namespace PasswordKeeper.Apps.Wpf.Abstractions
{
    public interface IViewModelFactory
    {
        T Create<T>() where T : ViewModelBase;

        ViewModelBase Create(Type type);
    }
}