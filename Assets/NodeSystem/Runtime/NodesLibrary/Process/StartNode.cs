using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Start", "Process/Start", FlowDirection.Output)]
    public class StartNode : NodeSystemNode
    {
        public override Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            Debug.Log("Hello World Start");
            return base.OnProcess(context);
        }
    }
}