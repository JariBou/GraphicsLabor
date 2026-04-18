using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Wait For Seconds", "Process/Wait For Seconds")]
    public class WaitNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public uint time;

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            await Awaitable.WaitForSecondsAsync(time);
            // return new ProcessInfo(id, GetNextNode(info.GraphInstance).id, ProcessInfo.ExecutionFlowType.Wait);
            return await ContinueExecution(GetNextNode(context.GraphInstance).ID);
        }
    }
}