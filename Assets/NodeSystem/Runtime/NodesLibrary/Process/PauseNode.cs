using System.Threading.Tasks;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Pause Exec", "Process/Pause Execution")]
    public class PauseNode : NodeSystemNode
    {
        public override async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            return await Task.FromResult(new ProcessInfo(ID, GetNextNode(context.GraphInstance).ID,
                ProcessInfo.ExecutionFlowType.Wait));
        }
    }
}