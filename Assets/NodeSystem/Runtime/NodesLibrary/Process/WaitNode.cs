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
        
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            info.NodeSystemExecutioner.GetObject().StartCoroutine(Wait(info));
            return new ProcessInfo(id, GetNextNode(info.GraphInstance).id, ProcessInfo.ExecutionFlowType.Wait);
        }

        public IEnumerator Wait(ExecInfo info)
        {
            yield return new WaitForSeconds(time);
            info.NodeSystemExecutioner.TickProcess();
        }
    }
}