using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary.Blackboard
{
    // So for some reason this node breaks serialization?
    [NodeInfo("Set Exposed Variable", "Blackboard/Set Exposed Variable")]
    public class SetExposedVariableNode : NodeSystemNode
    {
        [FormerlySerializedAs("m_Name"),
         ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string name;

        [FormerlySerializedAs("m_NewValue"),
         ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string newValue;

        [FormerlySerializedAs("m_Value"), ExposedProperty(PropPortDirection.Output,
             portCapacity: PropPortCapacity.Multi,
             preferredLocation: PropContainerLocation.OutputContainer)]
        public string value;

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            NodeSystemAsset graph = context.GraphInstance;
            string exposedVarName = await GetValueOfProp<string>(context, nameof(name));
            string newVal = await GetValueOfProp<string>(context, nameof(newValue));
            value = graph.SetExposedVariableValue(exposedVarName, newVal, out bool found);
            if (!found) value = "";

            return await base.OnProcess(context);
        }
    }
}