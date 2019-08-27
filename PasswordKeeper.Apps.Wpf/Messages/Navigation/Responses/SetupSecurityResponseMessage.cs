using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;
using PasswordKeeper.Services.Abstractions;

namespace PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses
{
    public class SetupSecurityResponseMessage<T> : StatefullMessage<T>
    {
        public IStorageProvider StorageProvider { get; set; }
    }
}