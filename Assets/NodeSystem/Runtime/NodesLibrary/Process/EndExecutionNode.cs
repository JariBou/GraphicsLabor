using NodeSystem.Runtime.Attributes;
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