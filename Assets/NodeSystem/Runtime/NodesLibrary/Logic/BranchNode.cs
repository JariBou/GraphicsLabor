using System.Threading.Tasks;
using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Logic
{
    [NodeInfo("Branch", "Logic/Branch")]
    public class BranchNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public bool condition;
        
        public override async Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            NodeSystemAsset graph = info.GraphInstance;
            NodeSystemNode nextNode = GetNodeConnectedToPort(graph, GetValueOfProp<bool>(info, nameof(condition)) ? 0 : 1);
            if (nextNode != null)
            {
                return await Task.FromResult(new ProcessInfo(id, nextNode.id, ProcessInfo.ExecutionFlowType.ExecuteNext));
            }
            return await EndExecution();
        }
    }
}