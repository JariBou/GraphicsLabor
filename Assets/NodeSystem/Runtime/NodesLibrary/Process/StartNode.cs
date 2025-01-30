using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    [NodeInfo("Start", "Process/Start", flowDirection: FlowDirection.Output)]
    public class StartNode : NodeSystemNode
    {
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            Debug.Log("Hello World Start");
            return base.OnProcess(info);
        }
    }
}