using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Utility
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] internal List<SerializedKeyValuePair<TKey, TValue>> SerializedKeyValues = new();
#if UNITY_EDITOR
        [SerializeField] internal DictionaryDrawStyle _drawElementsAsFoldout;
#endif
        
        
        public SerializedDictionary(SerializedDictionary<TKey, TValue> serializedDictionary) : base(serializedDictionary)
        {
        #if UNITY_EDITOR
            foreach (var kvp in serializedDictionary.SerializedKeyValues)
            {
                SerializedKeyValues.Add(new SerializedKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value));
            }
        #endif
        }
        
        public SerializedDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
            GenerateSerializedList();
        }

        public SerializedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(
            dictionary, comparer)
        {
            GenerateSerializedList();
        }

        public SerializedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection)
        {
            GenerateSerializedList();
        }

        public SerializedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection,
            IEqualityComparer<TKey> comparer) : base(collection, comparer)
        {
            GenerateSerializedList();
        }
        public SerializedDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public SerializedDictionary(int capacity) : base(capacity) { }
        public SerializedDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        
        
#if UNITY_EDITOR
        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                base[key] = value;
                bool entryFound = false;
                for (int i = 0; i < SerializedKeyValues.Count; i++)
                {
                    SerializedKeyValuePair<TKey, TValue> keyValue = SerializedKeyValues[i];
                    if (!GHelpers.AreKeysEqual(key, keyValue.Key))
                        continue;
                    entryFound = true;
                    keyValue.Value = value;
                    SerializedKeyValues[i] = keyValue;
                }
                
                if (!entryFound)
                {
                    SerializedKeyValues.Add(new SerializedKeyValuePair<TKey, TValue>(key, value));
                }
            }
        }
        
        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
            SerializedKeyValues.Add(new SerializedKeyValuePair<TKey, TValue>(key, value));
        }

        public new void Clear()
        {
            base.Clear();
            SerializedKeyValues.Clear();
        }

        public new bool Remove(TKey key)
        {
            if (!TryGetValue(key, out TValue value)) return false;
            
            base.Remove(key);
            SerializedKeyValues.Remove(new SerializedKeyValuePair<TKey, TValue>(key, value));
            return true;
        }

        public new bool TryAdd(TKey key, TValue value)
        {
            if (!base.TryAdd(key, value)) return false;
            
            SerializedKeyValues.Add(new SerializedKeyValuePair<TKey, TValue>(key, value));
            return true;

        }
#endif
        
        public void GLog()
        {
            foreach (SerializedKeyValuePair<TKey,TValue> keyValue in SerializedKeyValues)
            {
                GLogger.Log($"{keyValue.Key} - {keyValue.Value}");
            }
        }
        
        // Conditional makes it so that this method is only compiled if the condition is met
        [Conditional("UNITY_EDITOR")]
        private void GenerateSerializedList()
        {
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            {
                SerializedKeyValues.Add(new SerializedKeyValuePair<TKey, TValue>(kvp.Key, kvp.Value));
            }
        }

        // From Dictionary to "json"
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (SerializedKeyValues.Count == 0 && Count > 0) GenerateSerializedList();
#else
            SerializedKeyValues.Clear();
            GenerateSerializedList();
#endif
        }

        // From "json" to Dictionary
        public void OnAfterDeserialize()
        {
            base.Clear();

            foreach (SerializedKeyValuePair<TKey, TValue> keyValue in SerializedKeyValues)
            {
#if UNITY_EDITOR
                if (GHelpers.IsKeyValid(keyValue.Key) && !ContainsKey(keyValue.Key))
                {
                    base.Add(keyValue.Key, keyValue.Value);
                }
#else
                Add(kvp.Key, kvp.Value);
#endif
            }
        }
    }

    [Serializable]
    public class SerializedKeyValuePair<TKey, TValue>
    {
        [SerializeField] public TKey Key;
        [SerializeField] public TValue Value;

        public SerializedKeyValuePair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
    
    #if UNITY_EDITOR
    internal enum DictionaryDrawStyle
    {
        Element, Foldout
    }
    #endif
}