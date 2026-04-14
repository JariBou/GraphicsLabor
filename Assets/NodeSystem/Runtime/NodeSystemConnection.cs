using System;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    [Serializable]
    public struct NodeSystemConnection : IEquatable<NodeSystemConnection>
    {
        [FormerlySerializedAs("inputPort")] public NodeSystemConnectionPort InputPort;
        [FormerlySerializedAs("outputPort")] public NodeSystemConnectionPort OutputPort;

        public NodeSystemConnection(NodeSystemConnectionPort inputPort, NodeSystemConnectionPort outputPort)
        {
            InputPort = inputPort;
            OutputPort = outputPort;
        }

        public NodeSystemConnection(string inputNodeId, int inputPortIndex, string outputNodeId, int outputPortIndex)
        {
            InputPort = new NodeSystemConnectionPort(inputNodeId, inputPortIndex);
            OutputPort = new NodeSystemConnectionPort(outputNodeId, outputPortIndex);
        }

        public bool Equals(NodeSystemConnection other)
        {
            return InputPort.Equals(other.InputPort) && OutputPort.Equals(other.OutputPort);
        }

        public override bool Equals(object obj)
        {
            return obj is NodeSystemConnection other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(InputPort, OutputPort);
        }
    }

    [Serializable]
    public struct NodeSystemConnectionPort : IEquatable<NodeSystemConnectionPort>
    {
        [FormerlySerializedAs("nodeId")] public string NodeId;
        [FormerlySerializedAs("portIndex")] public int PortIndex;

        public NodeSystemConnectionPort(string nodeId, int portIndex)
        {
            NodeId = nodeId;
            PortIndex = portIndex;
        }

        public bool Equals(NodeSystemConnectionPort other)
        {
            return NodeId == other.NodeId && PortIndex == other.PortIndex;
        }

        public override bool Equals(object obj)
        {
            return obj is NodeSystemConnectionPort other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NodeId, PortIndex);
        }
    }
}