using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Blackboard
{
    [NodeInfo("Exposed Variable", "Blackboard/Exposed Variable", FlowDirection.None, true)]
    public class ExposedVariableNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string m_Name;

        [ExposedProperty(PropPortDirection.Output, portCapacity: PropPortCapacity.Multi,
            preferredLocation: PropContainerLocation.OutputContainer, labelOnly: true)]
        public string m_Value;

        public override Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            NodeSystemAsset graph = context.GraphInstance;
            m_Value = graph.GetExposedVariableValue(m_Name, out bool found);
            if (!found) m_Value = "";
            return base.OnProcess(context);
        }
    }
}