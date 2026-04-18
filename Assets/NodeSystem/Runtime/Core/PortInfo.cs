using System;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.Core
{
    [Serializable]
    public struct PortInfo
    {
        [SerializeField] private string _exposedPropertyName;
        [SerializeField] private string _ownerId;
        [SerializeField] private int _portIndex;

        [FormerlySerializedAs("_flowType"), FormerlySerializedAs("_portType"), SerializeField]
        private PropPortDirection _portDirection;

        public readonly string ExposedPropertyName => _exposedPropertyName;

        public readonly string OwnerId => _ownerId;

        public readonly int PortIndex => _portIndex;

        public readonly PropPortDirection PortDirection => _portDirection;

        public PortInfo(string exposedPropertyName, string ownerId, int portIndex, PropPortDirection portDirection)
        {
            _exposedPropertyName = exposedPropertyName;
            _ownerId = ownerId;
            _portIndex = portIndex;
            _portDirection = portDirection;
        }
    }
}