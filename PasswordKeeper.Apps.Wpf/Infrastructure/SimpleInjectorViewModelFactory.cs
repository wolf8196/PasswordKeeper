using System;
using PasswordKeeper.Apps.Wpf.Abstractions;
using PasswordKeeper.Apps.Wpf.Features;
using SimpleInjector;

namespace PasswordKeeper.Apps.Wpf.Infrastructure
{
    public class SimpleInjectorViewModelFactory : IViewModelFactory, IDisposable
    {
        private readonly Container container;

        public SimpleInjectorViewModelFactory(Container container)
        {
            this.container = container;
        }

        public T Create<T>() where T : ViewModelBase
        {
            return container.GetInstance<T>();
        }

        public ViewModelBase Create(Type type)
        {
            return (ViewModelBase)container.GetInstance(type);
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}