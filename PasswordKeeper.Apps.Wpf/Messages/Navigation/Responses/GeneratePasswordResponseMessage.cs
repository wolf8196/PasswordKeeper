using PasswordKeeper.Apps.Wpf.Messages.Navigation.States;

namespace PasswordKeeper.Apps.Wpf.Messages.Navigation.Responses
{
    public class GeneratePasswordResponseMessage<T> : StatefullMessage<T>
    {
        public string GeneratedPassword { get; set; }
    }
}