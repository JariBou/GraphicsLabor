using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Runtime.BlackBoard;
using NodeSystem.Runtime.NodesLibrary.Events;
using NodeSystem.Runtime.NodesLibrary.Process;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime
{
    [CreateAssetMenu(menuName = NodeSystemConsts.AddComponentMenuCategoryName+"/New Node Graph")]
    public class NodeSystemAsset : ScriptableObject
    {
        [SerializeField] private string _graphId = GuidSystem.NewGuid();

        [SerializeReference] private List<NodeSystemNode> m_nodes = new();

        [SerializeField] private List<NodeSystemConnection> m_connections = new();

        [SerializeField] private List<BlackboardProperty> m_exposedProperties = new();
        private readonly Dictionary<Type, NodeSystemNode> m_eventNodeLookup = new();

        private readonly Dictionary<string, NodeSystemNode> m_nodeLookup = new();

        public List<NodeSystemNode> Nodes => m_nodes;
        public List<NodeSystemConnection> Connections => m_connections;

        public List<BlackboardProperty> ExposedProperties => m_exposedProperties;

        public string GraphId => _graphId;

        public void Init()
        {
            foreach (NodeSystemNode node in Nodes)
            {
                // Node lookup init
                m_nodeLookup.Add(node.id, node);

                // Event node lookup init
                if (node is IEventNode eventNode)
                    if (!m_eventNodeLookup.TryAdd(eventNode.EventDataType, node))
                        Debug.LogError(
                            $"Found duplicate Event node for event of type '{eventNode.EventDataType}', only 1 event node per event type is supported.");
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
            foreach (NodeSystemConnection connection in Connections)
                if (connection.InputPort.NodeId == portInfo.OwnerId &&
                    connection.InputPort.PortIndex == portInfo.PortIndex)
                {
                    outConnection = connection;
                    return true;
                }

            outConnection = default;
            return false;
            // return Connections.Find(connection => connection.outputPort.nodeId == portInfo.OwnerId && connection.outputPort.portIndex == portInfo.PortIndex);
        }

        public void ModifyExposedVariable(string propertyName, string newValue)
        {
            BlackboardProperty property = ExposedProperties.Find(bgProp => bgProp.PropertyName == propertyName);
            property.PropertyValue = newValue;
        }

        public NodeSystemNode GetStartNode()
        {
            var startNodes = Nodes.OfType<StartNode>().ToArray();
            return startNodes.Length > 0 ? startNodes[0] : null;
        }

        public NodeSystemNode GetNode(string nextNodeId)
        {
            return m_nodeLookup.GetValueOrDefault(nextNodeId);
        }

        public NodeSystemNode GetNodeFromOutputConnection(string startingNodeId, int outputPortIndex)
        {
            foreach (NodeSystemConnection connection in Connections)
                if (connection.OutputPort.NodeId == startingNodeId &&
                    connection.OutputPort.PortIndex == outputPortIndex)
                {
                    string nodeId = connection.InputPort.NodeId;
                    NodeSystemNode node = m_nodeLookup[nodeId];
                    return node;
                }

            return null;
        }

        public string GetExposedVariableValue(string variableName, out bool found)
        {
            BlackboardProperty blackboardProperty =
                ExposedProperties.Find(bgProp => bgProp.PropertyName == variableName);
            if (blackboardProperty == null)
            {
                found = false;
                return "";
            }

            found = true;
            return blackboardProperty.PropertyValue;
        }

        public string SetExposedVariableValue(string variableName, string newVal, out bool found)
        {
            BlackboardProperty blackboardProperty =
                ExposedProperties.Find(bgProp => bgProp.PropertyName == variableName);
            if (blackboardProperty == null)
            {
                found = false;
                return "";
            }

            found = true;
            blackboardProperty.PropertyValue = newVal;
            return blackboardProperty.PropertyValue;
        }

        /// <summary>
        ///     Finds the <see cref="GameObjectSourceNode" /> linked to the parameter
        /// </summary>
        /// <param name="gameObject"> The linked gameobject </param>
        /// <returns> The Node or null if not found </returns>
        public NodeSystemNode GetNodeToPlayFromSource(GameObject gameObject)
        {
            string guid = ReferenceManager.GetGuidOf(gameObject);
            if (guid == "") return null;
            foreach (NodeSystemNode node in m_nodes)
                if (node is GameObjectSourceNode sourceNode)
                    if (sourceNode.Source == guid)
                        return node;

            return null;
        }

        /// <summary>
        ///     Gets the <see cref="EventNodeBase{T}" /> in the graph with event type T
        /// </summary>
        /// <typeparam name="T"> The type of the event </typeparam>
        /// <returns> The node or null </returns>
        public EventNodeBase<T> FindEventNode<T>() where T : EventData
        {
            if (m_eventNodeLookup.TryGetValue(typeof(T), out NodeSystemNode node)) return node as EventNodeBase<T>;

            return null;
        }
    }
}