using System;
using System.Collections.Generic;
using System.Linq;
using PasswordKeeper.Utils;

namespace PasswordKeeper.Services.Entities
{
    public sealed partial class Account : IOriginator
    {
        private List<Attachment> attachments;

        public Account()
        {
            attachments = new List<Attachment>();
        }

        public Account(Account account)
        {
            Id = account.Id;
            AccountName = account.AccountName;
            Login = account.Login;
            Password = account.Password;
            Notes = account.Notes;
            IsHidden = account.IsHidden;
            attachments = account.Attachments.Select(x => new Attachment(x)).ToList();
        }

        public Guid Id { get; set; }

        public string AccountName { get; set; }

        public string Login { get; set; }

        public string Password { get; set; }

        public string Notes { get; set; }

        public IEnumerable<Attachment> Attachments => attachments;

        public bool IsHidden { get; set; }

        public void Add(Attachment attachment)
        {
            attachments.Add(attachment);
        }

        public void Remove(Attachment attachment)
        {
            attachment.ThrowIfNull(nameof(attachment));

            attachments.Remove(attachment);
        }

        public void Remove(string id)
        {
            id.ThrowIfNull(nameof(id));

            var attachment = Attachments.FirstOrDefault(x => x.Id.ToString() == id);

            if (attachment == null)
            {
                return;
            }

            Remove(attachment);
        }

        public void Remove(Guid id)
        {
            Remove(id.ToString());
        }

        public void RemoveAllAttachments()
        {
            attachments.Clear();
        }

        public IMemento GetMemento()
        {
            return new Memento(this);
        }
    }
}