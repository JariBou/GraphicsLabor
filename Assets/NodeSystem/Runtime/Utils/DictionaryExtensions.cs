using System;
using System.Collections.Generic;

namespace NodeSystem.Runtime.Utils
{
    public static class DictionaryExtensions
    {
        public static TValue TryAddAndGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
                                                        Func<TValue> addAction)
        {
            if (dictionary.TryGetValue(key, out TValue value)) return value;

            dictionary.Add(key, addAction.Invoke());
            return dictionary[key];
        }

        public static TValue TryAddAndGet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
                                                        TValue defaultValue = default)
        {
            if (dictionary.TryGetValue(key, out TValue value)) return value;

            dictionary.Add(key, defaultValue);
            return dictionary[key];
        }
    }
}