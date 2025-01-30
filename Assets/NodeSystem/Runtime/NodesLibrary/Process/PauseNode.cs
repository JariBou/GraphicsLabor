using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Process
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