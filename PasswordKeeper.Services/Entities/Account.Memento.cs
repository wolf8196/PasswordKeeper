using System.Linq;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services.Entities
{
    public partial class Account
    {
        private class Memento : IMemento
        {
            private readonly Account account;
            private readonly Account backup;

            public Memento(Account account)
            {
                this.account = account;
                backup = new Account(account);
            }

            public void Restore()
            {
                account.Id = backup.Id;
                account.AccountName = backup.AccountName;
                account.Login = backup.Login;
                account.Password = backup.Password;
                account.Notes = backup.Notes;
                account.IsHidden = backup.IsHidden;
                account.attachments = backup.Attachments.Select(x => new Attachment(x)).ToList();
            }
        }
    }
}