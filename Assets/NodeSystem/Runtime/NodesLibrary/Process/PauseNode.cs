using System.Threading.Tasks;
using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Pause Exec", "Process/Pause Execution")]
    public class PauseNode : NodeSystemNode
    {
        public override async Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            return await Task.FromResult(new ProcessInfo(id, GetNextNode(info.GraphInstance).id, ProcessInfo.ExecutionFlowType.Wait));
        }
    }
}