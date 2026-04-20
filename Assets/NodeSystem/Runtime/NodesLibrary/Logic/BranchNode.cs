using System.Threading.Tasks;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Logic
{
    [NodeInfo("Branch", "Logic/Branch")]
    public class BranchNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public bool condition;

        public override async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            NodeSystemAsset graph = context.GraphInstance;
            NodeSystemNode nextNode =
                GetNodeConnectedToPort(graph, await GetValueOfProp<bool>(context, nameof(condition)) ? 0 : 1);
            if (nextNode != null)
                return await Task.FromResult(
                    new ProcessInfo(ID, nextNode.ID, ProcessInfo.ExecutionFlowType.ExecuteNext));
            return await EndExecution();
        }
    }
}