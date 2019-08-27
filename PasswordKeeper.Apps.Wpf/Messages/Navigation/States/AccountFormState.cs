using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Messages.Navigation.States
{
    public class AccountFormState
    {
        public AccountsState AccountsState { get; set; }

        public Account Account { get; set; }
    }
}