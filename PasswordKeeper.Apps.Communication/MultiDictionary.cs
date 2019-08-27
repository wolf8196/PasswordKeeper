using System.Collections.Generic;

namespace PasswordKeeper.Apps.Communication
{
    public partial class Messenger
    {
        private sealed class MultiDictionary<TOuterKey, TInnerKey, TValue>
        {
            private readonly Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>> internalDictionary;

            internal MultiDictionary()
            {
                internalDictionary = new Dictionary<TOuterKey, Dictionary<TInnerKey, TValue>>();
            }

            internal Dictionary<TInnerKey, TValue> this[TOuterKey outerKey]
            {
                get
                {
                    return internalDictionary[outerKey];
                }
            }

            internal void Clear()
            {
                internalDictionary.Clear();
            }

            internal bool ContainsKey(TOuterKey outerKey)
            {
                return internalDictionary.ContainsKey(outerKey);
            }

            internal void Add(TOuterKey outerKey, TInnerKey innerKey, TValue value)
            {
                CheckOuterKey(outerKey);
                internalDictionary[outerKey][innerKey] = value;
            }

            internal void Remove(TOuterKey outerKey, TInnerKey innerKey)
            {
                internalDictionary[outerKey].Remove(innerKey);
            }

            private void CheckOuterKey(TOuterKey outerKey)
            {
                if (!internalDictionary.ContainsKey(outerKey) || internalDictionary[outerKey] == null)
                {
                    internalDictionary[outerKey] = new Dictionary<TInnerKey, TValue>();
                }
            }
        }
    }
}