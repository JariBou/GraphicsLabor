using System;

namespace NodeSystem.Runtime.NodesLibrary.Events
{
    public interface IEventNode
    {
        public Type EventDataType { get; }
        public string EventName { get; set; }
    }
}