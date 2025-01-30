using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("Object Ref", "WIP/Object Ref", FlowDirection.None, isPure: true)]
    public class ObjectRefNode : NodeSystemNode
    {
        [ExposedProperty(PropPortDirection.Output, portType: typeof(GameObject), preferredLocation: PropContainerLocation.OutputContainer)]
        public SerializableRef Source = new();
        
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            GetValueOfProp<SerializableRef>(info, nameof(Source));
            return base.OnProcess(info);
        }

    }
}