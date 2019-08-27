using System;

namespace PasswordKeeper.Apps.Communication
{
    public interface IMessenger
    {
        void Reset();

        void Send<TMessage>(object sender, TMessage message);

        void Subscribe<TMessage>(object receiver, Action<TMessage> callback);

        void Unsubscribe<TMessage>(object receiver);
    }
}