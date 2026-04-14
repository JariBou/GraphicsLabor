using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Blackboard
{
    // So for some reason this node breaks serialization?
    [NodeInfo("Set Exposed Variable", "Blackboard/Set Exposed Variable", FlowDirection.Both)]
    public class SetExposedVariableNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string m_Name;
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string m_NewValue;
        [ExposedProperty(PropPortDirection.Output, portCapacity: PropPortCapacity.Multi, preferredLocation: PropContainerLocation.OutputContainer)]
        public string m_Value;

        public override Awaitable<ProcessInfo> OnProcess(ExecInfo info)
        {
            NodeSystemAsset graph = info.GraphInstance;
            string exposedVarName = GetValueOfProp<string>(info, nameof(m_Name));
            string newVal = GetValueOfProp<string>(info, nameof(m_NewValue));
            m_Value = graph.SetExposedVariableValue(exposedVarName, newVal, out bool found);
            if (!found) m_Value = "";
            
            return base.OnProcess(info);
        }
    }
}