using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("String to Int", "Utils/String to Int", isPure: true)]
    public class StringToIntNode : NodeSystemNode
    {
        [FormerlySerializedAs("string"), FormerlySerializedAs("m_string"), ExposedProperty(PropPortDirection.Input,
             preferredLocation: PropContainerLocation.InputContainer,
             disableInputWhenConnected: true)]
        public string inputString;

        [FormerlySerializedAs("m_Value"), ExposedProperty(PropPortDirection.Output,
             preferredLocation: PropContainerLocation.OutputContainer,
             labelOnly: true)]
        public int value;

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            string valueOfProp = await GetValueOfProp<string>(context, nameof(inputString));
            int.TryParse(valueOfProp, out value);
            return await base.OnProcess(context);
        }
    }
}