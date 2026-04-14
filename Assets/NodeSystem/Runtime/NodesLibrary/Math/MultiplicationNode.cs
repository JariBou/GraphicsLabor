using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Math
{
    [NodeInfo("Multiply", "Math/Multiply", FlowDirection.None, true)]
    public class MultiplicationNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public int a;

        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public int b;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public int result;

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            result = await GetValueOfProp<int>(context, nameof(a)) * await GetValueOfProp<int>(context, nameof(b));

            return await base.OnProcess(context);
        }
    }
}