using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Utils.RefSystem;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("Object Ref", "WIP/Object Ref", FlowDirection.None, true)]
    public class ObjectRefNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Output, typeof(GameObject),
            preferredLocation: PropContainerLocation.OutputContainer)]
        public SerializableRef Source = new();
        
        
        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            // await GetValueOfProp<SerializableRef>(context, nameof(Source));
            return await base.OnProcess(context);
        }
    }
}