using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core.PortConfigEnums;
using NodeSystem.Runtime.Core.RefSystem;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("GameObject Source Node", "Process/GameObject Source Node", FlowDirection.Output)]
    public class GameObjectSourceNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Output), SerializeField]
        public SerializableGameObjectRef source;
    }
}