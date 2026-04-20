using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core.PortConfigEnums;
using NodeSystem.Runtime.Core.RefSystem;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("Object Ref", "WIP/Object Ref", FlowDirection.None, true)]
    public class ObjectRefNode : NodeSystemNode
    {
        [FormerlySerializedAs("Source"), ExposedProperty(PropPortDirection.Output,
             preferredLocation: PropContainerLocation.OutputContainer)]
        public SerializableGameObjectRef source = new();
    }
}