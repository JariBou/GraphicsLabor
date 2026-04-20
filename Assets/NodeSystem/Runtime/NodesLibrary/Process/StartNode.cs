using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Start", "Process/Start", FlowDirection.Output)]
    public class StartNode : NodeSystemNode
    {
        public override Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            Debug.Log("Hello World Start");
            return base.OnProcessAsync(context);
        }
    }
}