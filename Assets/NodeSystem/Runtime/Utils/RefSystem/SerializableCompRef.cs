using System;
using NodeSystem.Runtime.References;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.Utils.RefSystem
{
    [Serializable]
    public class SerializableCompRef<T> : ISerializableTypedRef where T : Component
    {
        [FormerlySerializedAs("_objectId")] [SerializeField]
        private string _compId = ReferenceManager.NoneReference;

        [SerializeField] private string _ownerId = ReferenceManager.NoneReference;

        [SerializeField, Obsolete] private string _refTypename = "";

        public string CompId
        {
            get => _compId;
#if UNITY_EDITOR
            set => _compId = value;
#else
            private set => _compId = value;
#endif
        }

        public string OwnerId
        {
            get => _ownerId;
#if UNITY_EDITOR
            set => _ownerId = value;
#else
            private set => _ownerId = value;
#endif
        }

        [Obsolete]
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
            return typeof(T);
        }

        public T Get()
        {
            GameObject owner = ReferenceManager.GetGameObject<GameObject>(_ownerId);
            if (owner == null) return null;

            GameObjectComponentReferenceBank compRefBank = owner.GetComponent<GameObjectComponentReferenceBank>();
            if (compRefBank == null)
            {
                Debug.LogError($"Missing GameObjectComponentReferenceBank on GameObject '{owner.name}'");
                return null;
            }

            return compRefBank.GetComp<T>(_compId);
        }
    }
}