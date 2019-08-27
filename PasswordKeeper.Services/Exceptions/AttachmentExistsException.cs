using System;

namespace PasswordKeeper.Services.Exceptions
{
    public class AttachmentExistsException : Exception
    {
        public AttachmentExistsException(string attachmentName)
            : base("Attachment already exists")
        {
            AttachmentName = attachmentName;
        }

        public string AttachmentName { get; set; }
    }
}