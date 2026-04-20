using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
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

        public override async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            result = await GetValueOfProp<int>(context, nameof(a)) * await GetValueOfProp<int>(context, nameof(b));

            return await base.OnProcessAsync(context);
        }
    }
}