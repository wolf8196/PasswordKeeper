using System;

namespace PasswordKeeper.Apps.Communication
{
    public sealed partial class Messenger : IMessenger
    {
        private readonly MultiDictionary<Type, object, Action<object>> subscriptionsDictionary;

        public Messenger()
        {
            subscriptionsDictionary = new MultiDictionary<Type, object, Action<object>>();
        }

        public void Reset()
        {
            subscriptionsDictionary.Clear();
        }

        public void Send<TMessage>(object sender, TMessage message)
        {
            if (subscriptionsDictionary.ContainsKey(typeof(TMessage)))
            {
                foreach (var receivers in subscriptionsDictionary[typeof(TMessage)])
                {
                    if (receivers.Key != sender)
                    {
                        receivers.Value(message);
                    }
                }
            }
        }

        public void Subscribe<TMessage>(object receiver, Action<TMessage> callback)
        {
            subscriptionsDictionary.Add(typeof(TMessage), receiver, (o) => callback((TMessage)o));
        }

        public void Unsubscribe<TMessage>(object receiver)
        {
            subscriptionsDictionary.Remove(typeof(TMessage), receiver);
        }
    }
}