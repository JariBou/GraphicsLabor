using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("Int Node", "Utils/Int Node", FlowDirection.None)]
    public class IntNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer,
            disableInputWhenConnected: false)]
        public int outInt;
    }
}