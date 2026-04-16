using System;
using NodeSystem.Runtime.References;
using UnityEngine;

namespace NodeSystem.Runtime.Utils.RefSystem
{
    [Serializable]
    public class SerializableGameObjectRef : ISerializableTypedRef
    {
        [SerializeField] private string _objectId = ReferenceManager.NoneReference;

        [SerializeField] private string _refTypename = "";

        public string ObjectId
        {
            get => _objectId;
#if UNITY_EDITOR
            set => _objectId = value;
#else
            private set => _objectId = value;
#endif
        }

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

        public GameObject Get()
        {
            return ReferenceManager.GetGameObject<GameObject>(_objectId);
        }
    }
}