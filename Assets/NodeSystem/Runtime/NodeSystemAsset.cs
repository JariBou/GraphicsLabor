using System.Collections.Generic;
using System.Linq;
using NodeSystem.Runtime.BlackBoard;
using NodeSystem.Runtime.NodesLibrary.Process;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime
{
    [CreateAssetMenu(menuName = "NodeSystem/New Graph")]
    public class NodeSystemAsset : ScriptableObject
    {
        [SerializeField] private string _graphId = GuidSystem.NewGuid();
        
        [SerializeReference]
        private List<NodeSystemNode> m_nodes;
        
        [SerializeField]
        private List<NodeSystemConnection> m_connections;

        [SerializeField] private List<BlackboardProperty> m_exposedProperties;
        
        private Dictionary<string, NodeSystemNode> m_nodeLookup = new();
        
        public List<NodeSystemNode> Nodes => m_nodes;
        public List<NodeSystemConnection> Connections => m_connections;

        public List<BlackboardProperty> ExposedProperties => m_exposedProperties;

        public string GraphId => _graphId;

        public NodeSystemAsset()
        {
            m_nodes = new List<NodeSystemNode>();
            m_connections = new List<NodeSystemConnection>();
            m_exposedProperties = new List<BlackboardProperty>();
        }

        public void Init()
        {
            foreach (NodeSystemNode node in Nodes)
            {
                Debug.Log(node.id);
                m_nodeLookup.Add(node.id, node);
            }
        }

        /// <summary>
        /// Returns true if connection is found to the passed in port
        /// </summary>
        /// <param name="portInfo"></param>
        /// <param name="outConnection"></param>
        /// <returns></returns>
        public bool GetConnectionToPort(PortInfo portInfo, out NodeSystemConnection outConnection)
        {
            foreach (NodeSystemConnection connection in Connections)
            {
                if (connection.inputPort.nodeId == portInfo.OwnerId &&
                    connection.inputPort.portIndex == portInfo.PortIndex)
                {
                    outConnection = connection;
                    return true;
                }
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
            StartNode[] startNodes = Nodes.OfType<StartNode>().ToArray();
            return startNodes.Length > 0 ? startNodes[0] : null;
        }

        public NodeSystemNode GetNode(string nextNodeId)
        {
            return m_nodeLookup.GetValueOrDefault(nextNodeId);
        }

        public NodeSystemNode GetNodeFromOutputConnection(string startingNodeId, int outputPortIndex)
        {
            
            foreach (NodeSystemConnection connection in Connections)
            {
                if (connection.outputPort.nodeId == startingNodeId && connection.outputPort.portIndex == outputPortIndex)
                {
                    string nodeId = connection.inputPort.nodeId;
                    NodeSystemNode node = m_nodeLookup[nodeId];
                    return node;
                }
            }
            
            return null;
        }

        public string GetExposedVariableValue(string variableName, out bool found)
        {
            BlackboardProperty blackboardProperty = ExposedProperties.Find(bgProp => bgProp.PropertyName == variableName);
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
            BlackboardProperty blackboardProperty = ExposedProperties.Find(bgProp => bgProp.PropertyName == variableName);
            if (blackboardProperty == null)
            {
                found = false;
                return "";
            }
            found = true;
            blackboardProperty.PropertyValue = newVal;
            return blackboardProperty.PropertyValue;
        }

        public NodeSystemNode GetNodeToPlay(GameObject gameObject)
        {
            string guid = ReferenceManager.GetGuidOf(gameObject);
            foreach (NodeSystemNode node in m_nodes)  
            {
                if (node is GameObjectSourceNode sourceNode)
                {
                    if (sourceNode.Source == guid)
                    {
                        return node;
                    }
                }
            }

            return null;
        }
    }
}
