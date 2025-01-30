using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("End Execution", "Process/End Execution",flowDirection: FlowDirection.Input)]
    public class EndExecutionNode : NodeSystemNode
    {
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            return new ProcessInfo(id, "", ProcessInfo.ExecutionFlowType.EndExecution);
        }
    }
}