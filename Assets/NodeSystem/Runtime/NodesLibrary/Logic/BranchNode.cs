using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Logic
{
    [NodeInfo("Branch", "Logic/Branch")]
    public class BranchNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public bool condition;
        
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            NodeSystemAsset graph = info.GraphInstance;
            NodeSystemNode nextNode = GetNodeConnectedToPort(graph, GetValueOfProp<bool>(info, nameof(condition)) ? 0 : 1);
            if (nextNode != null)
            {
                return new ProcessInfo(id, nextNode.id, ProcessInfo.ExecutionFlowType.ExecuteNext);
            }
            return new ProcessInfo(id, "", ProcessInfo.ExecutionFlowType.EndExecution);
        }
    }
}