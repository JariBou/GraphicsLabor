using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Blackboard
{
    [NodeInfo("Exposed Variable", "Blackboard/Exposed Variable", FlowDirection.None, isPure: true)]
    public class ExposedVariableNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string m_Name;
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public string m_Value;

        public override ProcessInfo OnProcess(ExecInfo info)
        {
            NodeSystemAsset graph = info.GraphInstance;
            m_Value = graph.GetExposedVariableValue(m_Name, out bool found);
            if (!found) m_Value = "";
            return base.OnProcess(info);
        }
    }
}