using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("TextOutput", "Utils/TextOutput")]
    public class TextOutputNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Output)]
        public string TextOutput;

        public override Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            return base.OnProcess(context);
        }
    }
}