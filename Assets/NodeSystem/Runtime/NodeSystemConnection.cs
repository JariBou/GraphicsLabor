using System;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    [Serializable]
    public struct NodeSystemConnection : IEquatable<NodeSystemConnection>
    {
        [FormerlySerializedAs("InputPort")] public NodeSystemConnectionPort inputPort;
        [FormerlySerializedAs("OutputPort")] public NodeSystemConnectionPort outputPort;

        public NodeSystemConnection(NodeSystemConnectionPort inputPort, NodeSystemConnectionPort outputPort)
        {
            this.inputPort = inputPort;
            this.outputPort = outputPort;
        }

        public NodeSystemConnection(string inputNodeId, int inputPortIndex, string outputNodeId, int outputPortIndex)
        {
            inputPort = new NodeSystemConnectionPort(inputNodeId, inputPortIndex);
            outputPort = new NodeSystemConnectionPort(outputNodeId, outputPortIndex);
        }

        public bool Equals(NodeSystemConnection other)
        {
            return inputPort.Equals(other.inputPort) && outputPort.Equals(other.outputPort);
        }

        public override bool Equals(object obj)
        {
            return obj is NodeSystemConnection other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(inputPort, outputPort);
        }
    }
}