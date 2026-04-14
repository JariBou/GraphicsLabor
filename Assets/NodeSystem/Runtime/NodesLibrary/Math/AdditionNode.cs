using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Math
{
    [NodeInfo("Addition Node", "Math/Addition", FlowDirection.None, true)]
    public class AdditionNode : NodeSystemNode
    {
        // TODO: make it so you can expand number of ports
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public int a;

        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public int b;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public int result;

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            result = await GetValueOfProp<int>(context, nameof(a)) + await GetValueOfProp<int>(context, nameof(b));

            return await base.OnProcess(context);
        }
    }
}