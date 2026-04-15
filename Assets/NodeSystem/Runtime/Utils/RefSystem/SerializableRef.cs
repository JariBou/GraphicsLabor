using System;
using NodeSystem.Runtime.References;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeSystem.Runtime.Utils.RefSystem
{
    [Serializable, Obsolete]
    public class SerializableRef
    {
        [SerializeField] private string _objectId = "";

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

        public T Get<T>() where T : Object
        {
            return ReferenceManager.GetGameObject<T>(_objectId);
        }
    }
}