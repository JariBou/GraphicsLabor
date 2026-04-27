using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NodeSystem.Runtime.BlackBoard;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.NodesLibrary.Events;
using NodeSystem.Runtime.NodesLibrary.Process;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    [CreateAssetMenu(menuName = NodeSystemConsts.AddComponentMenuCategoryName + "/New Node Graph")]
    public class NodeSystemAsset : ScriptableObject
    {
        [SerializeField] private string _graphId = GuidSystem.NewGuid();

        [FormerlySerializedAs("m_nodes"), SerializeReference]
        private List<NodeSystemNode> _nodes = new();

        [FormerlySerializedAs("m_connections"), SerializeField]
        private List<NodeSystemConnection> _connections = new();

        [FormerlySerializedAs("m_exposedProperties"), SerializeField]
        private List<BlackboardProperty> _exposedProperties = new();

        private readonly Dictionary<Type, NodeSystemNode> _eventNodeLookup = new();
        private readonly Dictionary<Type, Dictionary<string, NodeSystemNode>> _eventNodeLookup2 = new();

        private readonly Dictionary<string, NodeSystemNode> _nodeLookup = new();

        public List<NodeSystemNode> Nodes => _nodes;
        public List<NodeSystemConnection> Connections => _connections;

        public List<BlackboardProperty> ExposedProperties => _exposedProperties;

        public string GraphId => _graphId;

        public void Init()
        {
            foreach (NodeSystemNode node in Nodes)
            {
                // == Node lookup init ==
                _nodeLookup.Add(node.ID, node);
                // ==========================

                // == Event node lookup init ==
                if (node is not IEventNode eventNode) continue;

                Dictionary<string, NodeSystemNode> nodeContainer =
                    _eventNodeLookup2.TryAddAndGet(eventNode.EventDataType, new Dictionary<string, NodeSystemNode>());
                nodeContainer.TryAdd(eventNode.EventName, node);

                if (!_eventNodeLookup.TryAdd(eventNode.EventDataType, node))
                {
                    Debug.LogError(
                        $"Found duplicate Event node for event of type '{eventNode.EventDataType}', only 1 event node per event type is supported.");
                }
                // ==========================
            }
        }

        /// <summary>
        ///     Returns true if connection is found to the passed in port
        /// </summary>
        /// <param name="portInfo"></param>
        /// <param name="outConnection"></param>
        /// <returns></returns>
        public bool GetConnectionToPort(PortInfo portInfo, out NodeSystemConnection outConnection)
        {
            foreach (NodeSystemConnection connection in Connections.Where(connection => connection.inputPort.nodeId == portInfo.OwnerId &&
                                                                                        connection.inputPort.portIndex == portInfo.PortIndex))
            {
                outConnection = connection;
                return true;
            }

            outConnection = default;
            return false;
            // return Connections.Find(connection => connection.outputPort.nodeId == portInfo.OwnerId && connection.outputPort.portIndex == portInfo.PortIndex);
        }

        [Obsolete]
        public void ModifyExposedVariable(string propertyName, string newValue)
        {
            BlackboardProperty property = ExposedProperties.Find(bgProp => bgProp.propertyName == propertyName);
            property.propertyValue = newValue;
        }

        public NodeSystemNode GetStartNode()
        {
            StartNode[] startNodes = Nodes.OfType<StartNode>().ToArray();
            return startNodes.Length > 0 ? startNodes[0] : null;
        }

        public NodeSystemNode GetNode(string nextNodeId)
        {
            return _nodeLookup.GetValueOrDefault(nextNodeId);
        }

        public NodeSystemNode GetNodeFromOutputConnection(string startingNodeId, int outputPortIndex)
        {
            return (from connection in Connections
                    where connection.outputPort.nodeId == startingNodeId && connection.outputPort.portIndex == outputPortIndex
                    select connection.inputPort.nodeId
                    into nodeId
                    select _nodeLookup[nodeId]).FirstOrDefault();
        }

        [Obsolete]
        public string GetExposedVariableValue(string variableName, out bool found)
        {
            BlackboardProperty blackboardProperty =
                ExposedProperties.Find(bgProp => bgProp.propertyName == variableName);
            if (blackboardProperty == null)
            {
                found = false;
                return "";
            }

            found = true;
            return blackboardProperty.propertyValue;
        }

        [Obsolete]
        public string SetExposedVariableValue(string variableName, string newVal, out bool found)
        {
            BlackboardProperty blackboardProperty =
                ExposedProperties.Find(bgProp => bgProp.propertyName == variableName);
            if (blackboardProperty == null)
            {
                found = false;
                return "";
            }

            found = true;
            blackboardProperty.propertyValue = newVal;
            return blackboardProperty.propertyValue;
        }

        /// <summary>
        ///     Finds the <see cref="GameObjectSourceNode" /> linked to the parameter
        /// </summary>
        /// <param name="gameObject"> The linked gameobject </param>
        /// <returns> The Node or null if not found </returns>
        public NodeSystemNode GetNodeToPlayFromSource(GameObject gameObject)
        {
            string guid = ReferenceManager.GetGuidOf(gameObject);
            return guid == "" ? null : _nodes.OfType<GameObjectSourceNode>().FirstOrDefault(node => node.source.ObjectId == guid);
        }

        /// <summary>
        ///     Gets the first <see cref="EventNodeBase{T}" /> in the graph with event type T
        /// </summary>
        /// <typeparam name="T"> The type of the event </typeparam>
        /// <returns> The node or null </returns>
        [CanBeNull]
        public EventNodeBase<T> FindEventNode<T>() where T : EventData
        {
            if (!_eventNodeLookup2.TryGetValue(typeof(T), out Dictionary<string, NodeSystemNode> container)) return null;
            return container.FirstOrDefault().Value as EventNodeBase<T>;
            // if (container.Count > 0) return container.FirstOrDefault().Value as EventNodeBase<T>;
            //
            // return null;
        }

        /// <summary>
        ///     Gets the <see cref="EventNodeBase{T}" /> in the graph with event type T  and eventName
        /// </summary>
        /// <typeparam name="T"> The type of the event </typeparam>
        /// <returns> The node or null </returns>
        [CanBeNull]
        public EventNodeBase<T> FindEventNode<T>(string eventName) where T : EventData
        {
            if (!_eventNodeLookup2.TryGetValue(typeof(T), out Dictionary<string, NodeSystemNode> container)) return null;
            if (container.TryGetValue(eventName, out NodeSystemNode eventNode)) return eventNode as EventNodeBase<T>;

            return null;
            // if (_eventNodeLookup2.TryGetValue(typeof(T), out Dictionary<string, NodeSystemNode> container))
            // {
            //     if (container.Count > 0)
            //     {
            //         if (container.TryGetValue(eventName, out NodeSystemNode eventNode)) return eventNode as EventNodeBase<T>;
            //     }
            // }
            //
            // return null;
        }
    }
}