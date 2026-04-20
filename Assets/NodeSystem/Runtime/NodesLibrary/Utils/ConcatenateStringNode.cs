using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("Concatenate String", "Utils/Concatenate String", isPure: true)]
    public class ConcatenateStringNode : NodeSystemNode
    {
        // TODO: make it so you can expand number of ports
        [FormerlySerializedAs("m_a"),
         ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string a;

        [FormerlySerializedAs("m_b"),
         ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public string b;

        [FormerlySerializedAs("m_Value"),
         ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public string value;

        public override async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            value = await GetValueOfProp<string>(context, nameof(a)) +
                    await GetValueOfProp<string>(context, nameof(b));
            return await base.OnProcessAsync(context);
        }
    }
}