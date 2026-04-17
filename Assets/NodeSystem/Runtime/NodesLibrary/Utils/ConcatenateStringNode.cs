using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using UnityEngine;

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

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            m_Value = await GetValueOfProp<string>(context, nameof(m_a)) +
                      await GetValueOfProp<string>(context, nameof(m_b));
            return await base.OnProcess(context);
        }
    }
}