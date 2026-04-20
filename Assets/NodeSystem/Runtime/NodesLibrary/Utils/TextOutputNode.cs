using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("TextOutput", "Utils/TextOutput")]
    public class TextOutputNode : NodeSystemNode
    {
        [FormerlySerializedAs("TextOutput"), ExposedProperty(PropPortDirection.Output)]
        public string textOutput;
    }
}