using System;
using System.Collections.Generic;
using System.Linq;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services.Entities
{
    public sealed class User
    {
        private List<Account> accounts;

        public User()
        {
            accounts = new List<Account>();
            Settings = new Settings();
        }

        public IEnumerable<Account> Accounts { get => accounts; }

        public Settings Settings { get; set; }

        public void AddAccount(Account account)
        {
            accounts.Add(account);
            accounts = accounts.OrderBy(x => x.AccountName).ThenBy(x => x.Id).Select(x => x).ToList();
        }

        public void RemoveAccount(Account account)
        {
            account.ThrowIfNull(nameof(account));

            accounts.Remove(account);
        }

        public void RemoveAccount(string accountId)
        {
            accountId.ThrowIfNull(nameof(accountId));

            var account = Accounts.FirstOrDefault(x => x.Id.ToString() == accountId);

            if (account == null)
            {
                return;
            }

            RemoveAccount(account);
        }

        public void RemoveAccount(Guid accountId)
        {
            RemoveAccount(accountId.ToString());
        }
    }
}