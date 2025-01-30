using System;
using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("Int to String Node", "Utils/Int to String Node", flowDirection: FlowDirection.None, isPure: true)]
    public class IntToStringNode : NodeSystemNode
    {
        [ExposedProperty(portDirection: PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)] public int inInt;
        
        [ExposedProperty(portDirection: PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)] public String outString;

        public override ProcessInfo OnProcess(ExecInfo info)
        {
            outString = GetValueOfProp<int>(info, nameof(inInt)).ToString();
            return base.OnProcess(info);
        }
    }
}