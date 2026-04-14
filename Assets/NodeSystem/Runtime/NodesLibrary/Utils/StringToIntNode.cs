using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("String to Int", "Utils/String to Int", isPure: true)]
    public class StringToIntNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer, disableInputWhenConnected: true)]
        public string m_string;
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer, labelOnly: true)]
        public int m_Value;

        public override Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            string valueOfProp = GetValueOfProp<string>(info, nameof(m_string));
            int.TryParse(valueOfProp, out m_Value);
            return base.OnProcess(info);
        }
    }
}