using System;
using System.Runtime.Serialization;
using NodeSystem.Runtime.References;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeSystem.Runtime.Utils.RefSystem
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

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ObjectValue", _objectId);
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
    }
}