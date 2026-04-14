using NodeSystem.Runtime.NodesLibrary.Events;
using UnityEngine;

namespace NodeSystem.Runtime.Extensions
{
    public static class NodeSystemAssetExtensions
    {
        public static Awaitable CallEvent<T>(this NodeSystemAsset self, EventNodeBase<T> eventNode, T eventData)
            where T : EventData
        {
            return eventNode?.Invoke(new ExecContext(self), eventData);
        }

        public static Awaitable TryCallEvent<T>(this NodeSystemAsset self, T eventData) where T : EventData
        {
            var eventNode = self.FindEventNode<T>();
            return eventNode?.Invoke(new ExecContext(self), eventData);
        }
    }
}