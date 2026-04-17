using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Utils.RefSystem;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("Object Ref", "WIP/Object Ref", FlowDirection.None, true)]
    public class ObjectRefNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Output,
            preferredLocation: PropContainerLocation.OutputContainer)]
        public SerializableGameObjectRef Source = new();

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            // await GetValueOfProp<SerializableRef>(context, nameof(Source));
            return await base.OnProcess(context);
        }
    }
}