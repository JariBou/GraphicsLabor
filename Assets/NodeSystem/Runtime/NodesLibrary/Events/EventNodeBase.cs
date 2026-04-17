using System;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Executioners;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Events
{
    //TODO: For now we will support only 1 node of each event type
    // TODO: LATER! auto gen of event nodes via event creation
    public abstract class EventNodeBase<T> : NodeSystemNode, IEventNode where T : EventData
    {
        public Type EventDataType => typeof(T);

        public abstract Awaitable Invoke(ExecContext ctx, T eventData);

        protected Awaitable DefaultInvoke(ExecContext ctx)
        {
            return NodeGlobalExecutioner.Instance.RunNode(ctx, this);
        }
    }
}