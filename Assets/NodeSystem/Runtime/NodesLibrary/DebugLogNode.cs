using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary
{
    [NodeInfo("Debug Log", "Debug/Debug Log Console")]
    public class DebugLogNode : NodeSystemNode
    {
        [ExposedProperty(portDirection: PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer, portType: typeof(string))]
        public string logMessage;
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            Debug.Log(GetValueOfProp<string>(info, nameof(logMessage)));
            return base.OnProcess(info);
        }

        
    }
}