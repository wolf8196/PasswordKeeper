using System;
using System.Threading.Tasks;

namespace PasswordKeeper.Apps.Wpf.Messages.Navigation
{
    public class DialogMessageBase<TResponse>
    {
        public Guid DialogId { get; set; }

        public Func<TResponse, Task> OnResponseReceived { get; set; }
    }
}