using System.Collections;
using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.Nodes
{
    [NodeInfo("Pause Exec", "Process/Pause Execution")]
    public class PauseNode : NodeSystemNode
    {
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            return new ProcessInfo(id, GetNextNode(info.GraphInstance).id, ProcessInfo.ExecutionFlowType.Wait);
        }
    }
}