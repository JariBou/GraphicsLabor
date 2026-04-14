using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("End Execution", "Process/End Execution",flowDirection: FlowDirection.Input)]
    public class EndExecutionNode : NodeSystemNode
    {
        public override async Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            return await EndExecution();
        }
    }
}