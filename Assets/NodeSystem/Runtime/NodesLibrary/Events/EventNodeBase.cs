using System;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Executioners;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Events
{
    //TODO: For now we will support only 1 node of each event type
    public abstract class EventNodeBase<T> : NodeSystemNode, IEventNode where T : EventData
    {
        public Type EventDataType => typeof(T);

        public abstract Awaitable Invoke(ExecContext ctx, T eventData);

        protected async Awaitable DefaultInvokeNoAwaitAsync(ExecContext ctx)
        {
            await NodeGlobalExecutioner.Instance.RunNodeNoAwaitAsync(ctx, this);
        }
        
        protected async Awaitable DefaultInvokeAwaitAsync(ExecContext ctx)
        {
            await NodeGlobalExecutioner.Instance.RunNodeAwaitAsync(ctx, this);
        }
    }
}