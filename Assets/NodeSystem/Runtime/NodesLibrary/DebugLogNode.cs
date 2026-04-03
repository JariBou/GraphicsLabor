using NodeSystem.Runtime.Attributes;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary
{
    [NodeInfo("Debug Log", "Debug/Debug Log Console")]
    public class DebugLogNode : NodeSystemNode
    {
        [FormerlySerializedAs("logMessage")] [ExposedProperty(portDirection: PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer, disableInputWhenConnected: true)]
        public string LogMessage;
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            Debug.Log(GetValueOfProp<string>(info, nameof(LogMessage)));
            return base.OnProcess(info);
        }

        
    }
}