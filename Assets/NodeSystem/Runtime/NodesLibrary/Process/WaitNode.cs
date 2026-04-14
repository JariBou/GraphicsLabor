using System.Collections;
using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Wait For Seconds", "Process/Wait For Seconds")]
    public class WaitNode : NodeSystemNode
    {
        [ExposedProperty(portDirection: PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public uint time;
        
        public override async Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            await Awaitable.WaitForSecondsAsync(time);
            // return new ProcessInfo(id, GetNextNode(info.GraphInstance).id, ProcessInfo.ExecutionFlowType.Wait);
            return await ContinueExecution(GetNextNode(info.GraphInstance).id);
        }
    }
}