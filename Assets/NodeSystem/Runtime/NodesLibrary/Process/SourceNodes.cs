using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.Nodes
{
    [NodeInfo("GameObject Source Node", "Process/GameObject Source Node", FlowDirection.Output)]
    public class GameObjectSourceNode : NodeSystemNode
    {
        [SourceProperty(typeof(GameObject)), SerializeField]
        public string Source;
    }
}