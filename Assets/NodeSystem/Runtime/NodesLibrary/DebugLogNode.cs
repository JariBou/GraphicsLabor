using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary
{
    [NodeInfo("Debug Log", "Debug/Debug Log Console")]
    public class DebugLogNode : NodeSystemNode
    {
        [FormerlySerializedAs("logMessage")]
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer,
            disableInputWhenConnected: true)]
        public string LogMessage;

        public override Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            Debug.Log(GetValueOfProp<string>(context, nameof(LogMessage)));
            return base.OnProcess(context);
        }
    }
}