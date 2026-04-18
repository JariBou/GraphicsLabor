using System;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    [Serializable]
    public struct NodeSystemConnectionPort : IEquatable<NodeSystemConnectionPort>
    {
        [FormerlySerializedAs("NodeId")] public string nodeId;
        [FormerlySerializedAs("PortIndex")] public int portIndex;

        public NodeSystemConnectionPort(string nodeId, int portIndex)
        {
            this.nodeId = nodeId;
            this.portIndex = portIndex;
        }

        public bool Equals(NodeSystemConnectionPort other)
        {
            return nodeId == other.nodeId && portIndex == other.portIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is NodeSystemConnectionPort other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(nodeId, portIndex);
        }
    }
}