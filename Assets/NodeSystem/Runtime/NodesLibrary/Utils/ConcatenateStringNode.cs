using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("Concatenate String", "Utils/Concatenate String", isPure: true)]
    public class ConcatenateStringNode : NodeSystemNode
    {
        // TODO: make it so you can expand number of ports
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string m_a;
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string m_b;
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public string m_Value;

        public override ProcessInfo OnProcess(ExecInfo info)
        {
            m_Value = GetValueOfProp<string>(info, nameof(m_a)) + GetValueOfProp<string>(info, nameof(m_b));
            return base.OnProcess(info);
        }
    }
}