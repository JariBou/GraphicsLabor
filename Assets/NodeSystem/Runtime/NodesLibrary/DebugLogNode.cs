using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary
{
    [NodeInfo("Debug Log", "Debug/Debug Log Console")]
    public class DebugLogNode : NodeSystemNode
    {
        [FormerlySerializedAs("LogMessage"), ExposedProperty(PropPortDirection.Input,
             preferredLocation: PropContainerLocation.InputContainer,
             disableInputWhenConnected: true)]
        public string logMessage;

        public override Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            Debug.Log(GetValueOfProp<string>(context, nameof(logMessage)));
            return base.OnProcessAsync(context);
        }
    }
}