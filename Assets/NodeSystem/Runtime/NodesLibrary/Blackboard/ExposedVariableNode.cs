using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary.Blackboard
{
    [NodeInfo("Exposed Variable", "Blackboard/Exposed Variable", FlowDirection.None, true)]
    public class ExposedVariableNode : NodeSystemNode
    {
        [FormerlySerializedAs("m_Name"),
         ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string name;

        [FormerlySerializedAs("m_Value"), ExposedProperty(PropPortDirection.Output,
             portCapacity: PropPortCapacity.Multi,
             preferredLocation: PropContainerLocation.OutputContainer, labelOnly: true)]
        public string value;

        public override Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            NodeSystemAsset graph = context.GraphInstance;
            value = graph.GetExposedVariableValue(name, out bool found);
            if (!found) value = "";
            return base.OnProcess(context);
        }
    }
}