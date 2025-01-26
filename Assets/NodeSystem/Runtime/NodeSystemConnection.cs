using System;

namespace NodeSystem.Runtime
{
    [Serializable]
    public struct NodeSystemConnection : IEquatable<NodeSystemConnection>
    {
        public NodeSystemConnectionPort inputPort;
        public NodeSystemConnectionPort outputPort;

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

    [Serializable]
    public struct NodeSystemConnectionPort : IEquatable<NodeSystemConnectionPort>
    {
        public string nodeId;
        public int portIndex;

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