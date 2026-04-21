using System.Threading.Tasks;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using NodeSystem.Runtime.Executioners;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Wait For Seconds", "Process/Wait For Seconds")]
    public class WaitNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public float time;

        public override async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            _ = DoWaitAndResume(context);
            // return new ProcessInfo(id, GetNextNode(info.GraphInstance).id, ProcessInfo.ExecutionFlowType.Wait);
            return await Task.FromResult(new ProcessInfo(ID, GetNextNode(context.GraphInstance).ID,
                ProcessInfo.ExecutionFlowType.Wait));
        }

        private async Awaitable DoWaitAndResume(ExecContext context)
        {
            await Awaitable.WaitForSecondsAsync(time);
            _ = NodeGlobalExecutioner.Instance.RunNodeAwaitAsync(context, GetNextNode(context.GraphInstance));
        }
    }
}