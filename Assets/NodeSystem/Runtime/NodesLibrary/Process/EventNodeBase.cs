using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Process
{
    public abstract class EventNodeBase<T> : NodeSystemNode
    {
        // public async Awaitable Trigger(T arg)
        // {
        //     await this.OnProcess()
        // }
    }

    public class EventTest : EventNodeBase<int>
    {
        [SerializeField, ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer, portCapacity: PropPortCapacity.Multi)] 
        public int context;
        
    }
}