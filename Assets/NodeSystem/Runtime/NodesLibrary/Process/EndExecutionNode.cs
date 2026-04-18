using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("End Execution", "Process/End Execution", FlowDirection.Input)]
    public class EndExecutionNode : NodeSystemNode
    {
        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            return await EndExecution();
        }
    }
}