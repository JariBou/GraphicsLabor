using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("TextOutput", "Utils/TextOutput")]
    public class TextOutputNode : NodeSystemNode
    {
        [ExposedProperty(portDirection: PropPortDirection.Output)]
        public string TextOutput;
        public override Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            return base.OnProcess(info);
        }
    }
}