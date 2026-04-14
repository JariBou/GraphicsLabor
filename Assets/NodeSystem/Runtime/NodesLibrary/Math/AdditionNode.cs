using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Math
{
    [NodeInfo("Addition Node", "Math/Addition", FlowDirection.None, isPure: true)]
    public class AdditionNode : NodeSystemNode
    {
        // TODO: make it so you can expand number of ports
        [ExposedProperty(portDirection: PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public int a;
        [ExposedProperty(portDirection: PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public int b;

        [ExposedProperty(portDirection: PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public int result;

        public override Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            result = GetValueOfProp<int>(info, nameof(a)) + GetValueOfProp<int>(info, nameof(b));
            
            return base.OnProcess(info);
        }
    }
}