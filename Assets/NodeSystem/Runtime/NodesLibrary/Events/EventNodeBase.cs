using System;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Executioners;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Events
{
    [Serializable]
    public abstract class EventNodeBase<T> : NodeSystemNode, IEventNode where T : EventData
    {
        [SerializeField] private string _eventName = "New Event";

        public string EventName
        {
            get => _eventName;
            set => _eventName = value;
        }

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