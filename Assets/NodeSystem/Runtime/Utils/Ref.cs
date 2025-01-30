using System;
using System.Runtime.Serialization;
using NodeSystem.Runtime.References;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeSystem.Runtime.Utils
{
    [Serializable]
    public class Ref<T> : Object, ISerializable where T : Object
    {
        [SerializeField] private string _objectId;
        public string ObjectId
        {
            get => _objectId; 
            #if UNITY_EDITOR
            set => _objectId = value;
            #else
            private set => _objectId = value;
            #endif
        }

        public T Get()
        {
            return ReferenceManager.GetGameObject<T>(ObjectId);
        }

        public Type GetRefType()
        {
            return typeof(T);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
            info.AddValue("ObjectValue", _objectId);
        }
    }

    [Serializable]
    public class SerializableRef
    {
        [SerializeField] private string _objectId = "";

        public string ObjectId
        {
            get => _objectId; 
            #if UNITY_EDITOR
            set => _objectId = value;
            #else
            private set => _objectId = value;
            #endif
        }
        
        [SerializeField] private string _refTypename = "";

        public string RefTypename
        {
            get => _refTypename; 
            #if UNITY_EDITOR
            set => _refTypename = value; 
            #else
            private set => _refTypename = value;
            #endif
        }

        public Type GetRefType()
        {
            return Type.GetType(_refTypename);
        }

        public T Get<T>() where T : Object
        {
            return ReferenceManager.GetGameObject<T>(_objectId);
        }
    }
}