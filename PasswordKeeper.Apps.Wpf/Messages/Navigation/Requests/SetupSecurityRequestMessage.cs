using System;
using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;

namespace PasswordKeeper.Apps.Wpf.Messages.Navigation.Requests
{
    public class SetupSecurityRequestMessage<T> : StatefullMessage<T>
    {
        public Type ReturnToViewModel { get; set; }
    }
}