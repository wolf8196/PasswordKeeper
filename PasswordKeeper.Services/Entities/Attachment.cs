using System;

namespace PasswordKeeper.Services.Entities
{
    public sealed class Attachment
    {
        public Attachment()
        {
        }

        public Attachment(Attachment attachment)
        {
            Id = attachment.Id;
            Name = attachment.Name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}