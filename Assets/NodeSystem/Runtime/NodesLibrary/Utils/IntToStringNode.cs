using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("Int to String Node", "Utils/Int to String Node", FlowDirection.None, true)]
    public class IntToStringNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public int inInt;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public string outString;

        public override Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            outString = GetValueOfProp<int>(context, nameof(inInt)).ToString();
            return base.OnProcessAsync(context);
        }
    }
}