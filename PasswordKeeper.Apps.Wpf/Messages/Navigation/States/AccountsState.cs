using PasswordKeeper.Services.Entities;

namespace PasswordKeeper.Apps.Wpf.Messages.Navigation.States
{
    public class AccountsState
    {
        public Account SelectedAccount { get; set; }

        public string SearchText { get; set; }
    }
}