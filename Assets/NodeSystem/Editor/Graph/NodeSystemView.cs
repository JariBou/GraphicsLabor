using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Editor.Nodes;
using NodeSystem.Runtime;
using NodeSystem.Runtime.BlackBoard;
using NodeSystem.Runtime.NodesLibrary.Process;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph
{
    public class NodeSystemView : GraphView
    {
        private NodeSystemAsset m_nodeSystem;
        private SerializedObject m_serializedObject;
        private NodeSystemEditorWindow m_window;
        public NodeSystemEditorWindow window => m_window;
        public List<BlackboardProperty> ExposedProperties { get; private set; }

        public SerializedObject SerializedObject => m_serializedObject;

        public List<NodeSystemEditorNode> m_graphNodes;
        public Dictionary<string, NodeSystemEditorNode> m_nodeDictionary;
        public Dictionary<Edge, NodeSystemConnection> m_connectionsDictionary;
        
        private NodeSystemWindowSearchProvider m_searchProvider;
        
        private NodeSystemBlackboard m_blackboard;
        
        private List<NodeSystemNode> m_copiedNodesCache = new();
        
        public NodeSystemView(SerializedObject serializedObject, NodeSystemEditorWindow window)
        {
            m_serializedObject = serializedObject;
            m_window = window;
            m_nodeSystem = (NodeSystemAsset)serializedObject.targetObject;
            
            m_graphNodes = new List<NodeSystemEditorNode>();
            m_nodeDictionary = new Dictionary<string, NodeSystemEditorNode>();
            m_connectionsDictionary = new Dictionary<Edge, NodeSystemConnection>();
            ExposedProperties = m_nodeSystem.ExposedProperties;
            
            m_searchProvider = ScriptableObject.CreateInstance<NodeSystemWindowSearchProvider>();
            m_searchProvider.graph = this;
            nodeCreationRequest = ShowSearchWindow;
            
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeSystem/Editor/USS/NodeSystemEditor.uss");
            styleSheets.Add(styleSheet);
            
            GridBackground background = new GridBackground();
            background.name = "Grid";
            Add(background);
            background.SendToBack();
            
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphViewChangedEvent;
            Undo.undoRedoEvent += OnUndoRedo;

            
            // Neither work smh
            // EditorApplication.delayCall += () =>
            // {
            //     FrameAll();
            // };
            // schedule.Execute(() => { FrameAll(); });
            
            // canPasteSerializedData += CanPasteCallback;
            // unserializeAndPaste += PasteCallback;
            // serializeGraphElements += CopyCutCallback;
        }

        #region copy and paste
        
        private string CopyCutCallback(IEnumerable<GraphElement> elements)
        {
            List<GraphElement> enumerable = elements.ToList();
            Debug.Log("Copy/Cut Callback: " + enumerable.Count());
            m_copiedNodesCache.Clear();
            foreach (var element in enumerable)
            {
                if (element is NodeSystemEditorNode node)
                {
                    m_copiedNodesCache.Add(node.Node);
                } else if (element is Edge edge)
                {
                    
                }
            }
            return "";
        }

        private void PasteCallback(string operationname, string data)
        {
            Debug.Log("Paste callback: " + operationname);
            foreach (NodeSystemNode node in m_copiedNodesCache)
            {
                AddNodeToGraph(node);
                BindToSerializedObject();
            }
        }

        private bool CanPasteCallback(string data)
        {
            return true;
        }
        
        #endregion
        
        #region Blackboard

        public void AddBlackboardProperty(BlackboardProperty blackboardProperty, bool b)
        {
            // doesn't work for some reason?
            Undo.RecordObject(SerializedObject.targetObject, "Add BlackboardProperty"); 
            m_blackboard.AddProperty(blackboardProperty, b);
        }
        
        // Not used anymore
        public NodeSystemBlackboard GetNodeSystemBlackboard()
        {
            return m_blackboard;
        }
        
        public void SetBlackboard(NodeSystemBlackboard blackboard)
        {
            m_blackboard = blackboard;
        }
        
        public void ClearBlackBoardAndExposedProperties()
        {
            //ExposedProperties.Clear();
            m_blackboard.Clear();
        }
        
        [Obsolete]
        public void ModifyExposedProperties(Action<List<BlackboardProperty>> action)
        {
            action.Invoke(ExposedProperties);
            // Wait actually we don't need this, let's keep it still as an event possible source
            // action.Invoke(m_nodeSystem.ExposedProperties);
            // or if we create a setter
            // m_nodeSystem.ExposedProperties = ExposedProperties;
        }

        #endregion

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            List<Port> changedPorts = new List<Port>();
            // Debug.Log("OnGraphViewChangedEvent");
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(SerializedObject.targetObject, "Moved Nodes");
                foreach (NodeSystemEditorNode editorNode in graphViewChange.movedElements.OfType<NodeSystemEditorNode>())
                {
                    editorNode.UpdatePosition();
                }
            }
            
            if (graphViewChange.elementsToRemove != null)
            {
                List<NodeSystemEditorNode> nodesToRemove = graphViewChange.elementsToRemove.OfType<NodeSystemEditorNode>().ToList();
                if (nodesToRemove.Count > 0)
                {
                    Undo.RecordObject(SerializedObject.targetObject, "Removed Node");
                    
                    for (int i = nodesToRemove.Count()-1; i >= 0; i--)
                    {
                        RemoveNode(nodesToRemove[i]);
                    }
                }

                List<Edge> edgesToRemove = graphViewChange.elementsToRemove.OfType<Edge>().ToList();
                if (edgesToRemove.Any())
                {
                    Undo.RecordObject(SerializedObject.targetObject, "Removed Connection");
                    foreach (Edge edge in edgesToRemove)
                    {

                        Debug.Log("Removing Edge");
                        RemoveConnection(edge);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                Undo.RecordObject(SerializedObject.targetObject, "Connected Nodes");
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    changedPorts.Add(edge.input);
                    changedPorts.Add(edge.output);
                    CreateConnection(edge);
                }
            }

            SerializedObject.Update();

            // foreach (Port port in changedPorts)
            // {
            //     List<VisualElement> portContentContainer = new List<VisualElement>();
            //     for (int i = 0; i < port.contentContainer.childCount; i++)
            //     {
            //         portContentContainer.Add(port.contentContainer[i]);
            //         if (port.connections.Any())
            //         {
            //             VisualElement element = port.contentContainer[i];
            //             if (element is PropertyField propertyField)
            //             {
            //                 TextField tempField = new TextField()
            //                 {
            //                     name = propertyField.name,
            //                     bindingPath = propertyField.bindingPath,
            //                 };
            //                 portContentContainer[i] = tempField;
            //             } 
            //         }
            //         else
            //         {
            //             VisualElement element = port.contentContainer[i];
            //             if (element is TextField textField)
            //             {
            //                 PropertyField tempField = new PropertyField()
            //                 {
            //                     name = textField.name,
            //                     bindingPath = textField.bindingPath,
            //                 };
            //                 portContentContainer[i] = tempField;
            //             }
            //         }
            //     }
            //     port.contentContainer.Clear();
            //     foreach (VisualElement visualElement in portContentContainer)
            //     {
            //         port.contentContainer.Add(visualElement);
            //     }
            // }

            return graphViewChange;
        }
        
        private void OnUndoRedo(in UndoRedoInfo undo)
        {
            Debug.Log("OnUndoRedo");

            #region Nodes
            
            List<NodeSystemEditorNode> currentEditorNodes = new List<NodeSystemEditorNode>(m_graphNodes);
            List<NodeSystemNode> neededNodesList = new List<NodeSystemNode>(m_nodeSystem.Nodes);
            for (int i = 0; i < m_graphNodes.Count; i++)
            {
                NodeSystemEditorNode editorNode = m_graphNodes[i];
                if (m_nodeSystem.Nodes.Contains(editorNode.Node))
                {
                    NodeSystemNode nodeSystemNode = neededNodesList.Find(systemNode => systemNode == editorNode.Node);
                    editorNode.SetPosition(nodeSystemNode.position);
                    neededNodesList.Remove(editorNode.Node);
                    currentEditorNodes.Remove(editorNode);
                }
            }

            if (neededNodesList.Count > 0)
            {
                foreach (NodeSystemNode node in neededNodesList)
                {
                    AddNodeToGraph(node);
                    BindToSerializedObject();
                }
            }

            if (currentEditorNodes.Count > 0)
            {
                foreach (NodeSystemEditorNode editorNode in currentEditorNodes)
                {
                    RemoveElement(editorNode);
                    RemoveNode(editorNode);
                }
            }
            
            #endregion

            #region Connections
            
            List<NodeSystemConnection> neededConnections = new List<NodeSystemConnection>(m_nodeSystem.Connections);
            List<NodeSystemConnection> graphConnections = new List<NodeSystemConnection>(m_connectionsDictionary.Values);

            foreach (NodeSystemConnection connection in m_connectionsDictionary.Values)
            {
                if (neededConnections.Contains(connection))
                {
                    neededConnections.Remove(connection);
                    graphConnections.Remove(connection);
                }
            }

            if (graphConnections.Count > 0)
            {

                for (int i = 0; i < graphConnections.Count; i++)
                {
                    Edge edgeToRemove = m_connectionsDictionary.Keys.ToList()[m_connectionsDictionary.Values.ToList().IndexOf(graphConnections[i])];
                    edgeToRemove.input.Disconnect(edgeToRemove);
                    edgeToRemove.output.Disconnect(edgeToRemove);
                    RemoveElement(edgeToRemove);
                    RemoveConnection(edgeToRemove);
                }
            }

            if (neededConnections.Count > 0)
            {
                foreach (NodeSystemConnection connection in neededConnections)
                {
                    Edge edgeToCreate = new Edge
                    {
                        input = GetNode(connection.inputPort.nodeId).Ports[connection.inputPort.portIndex],
                        output = GetNode(connection.outputPort.nodeId).Ports[connection.outputPort.portIndex]
                    };
                    edgeToCreate.input.Connect(edgeToCreate);
                    edgeToCreate.output.Connect(edgeToCreate);
                    AddElement(edgeToCreate);
                    m_connectionsDictionary.Add(edgeToCreate, connection);
                }
            }
            
            #endregion
        }

        private void CreateConnection(Edge edge)
        {
            NodeSystemEditorNode inputNode = (NodeSystemEditorNode)edge.input.node;
            int inputIndex = inputNode.Ports.IndexOf(edge.input);
            
            NodeSystemEditorNode outputNode = (NodeSystemEditorNode)edge.output.node;
            int outputIndex = outputNode.Ports.IndexOf(edge.output);
            
            NodeSystemConnection connection = new(inputNode.Node.id, inputIndex, outputNode.Node.id, outputIndex);
            m_nodeSystem.Connections.Add(connection);
            m_connectionsDictionary.Add(edge, connection);
        }

        private void RemoveNode(NodeSystemEditorNode editorNode)
        {
            m_nodeSystem.Nodes.Remove(editorNode.Node);
            m_nodeDictionary.Remove(editorNode.Node.id);
            m_graphNodes.Remove(editorNode);
            SerializedObject.Update();
        }

        private void RemoveConnection(Edge edge)
        {
            if (m_connectionsDictionary.TryGetValue(edge, out NodeSystemConnection connection))
            {
                m_nodeSystem.Connections.Remove(connection);
                m_connectionsDictionary.Remove(edge);
            }
        }

        private void DrawNodes()
        {
            foreach (NodeSystemNode node in m_nodeSystem.Nodes)
            {
                AddNodeToGraph(node);
            }

            if (m_nodeSystem.Nodes.Count == 0)
            {
                StartNode startNode = new StartNode();
                //startNode.SetPosition();
                Add(startNode);
            }
            
            BindToSerializedObject();
        }
        
        private void DrawConnections()
        {
            if (m_nodeSystem.Connections == null) return;

            foreach (NodeSystemConnection connection in m_nodeSystem.Connections)
            {
                DrawConnection(connection);
            }
        }

        private void DrawConnection(NodeSystemConnection connection)
        {
            NodeSystemEditorNode inputNode = GetNode(connection.inputPort.nodeId);
            if(inputNode == null) return;
            NodeSystemEditorNode outputNode = GetNode(connection.outputPort.nodeId);
            if(outputNode == null) return;
            
            Port inputPort = inputNode.Ports[connection.inputPort.portIndex]; 
            Port outputPort = outputNode.Ports[connection.outputPort.portIndex];

            Edge edge = inputPort.ConnectTo(outputPort);
            m_connectionsDictionary.Add(edge, connection);
            AddElement(edge);
        }

        private NodeSystemEditorNode GetNode(string nodeId)
        {
            NodeSystemEditorNode node;
            m_nodeDictionary.TryGetValue(nodeId, out node);
            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new List<Port>();
            List<Port> validPorts = new List<Port>();
            

            foreach (NodeSystemEditorNode editorNode in m_graphNodes)
            {
                allPorts.AddRange(editorNode.Ports);
            }

            foreach (Port port in allPorts)
            {
                if (port == startPort) {continue;}
                if (port.node == startPort.node) {continue;}
                if (port.direction == startPort.direction) {continue;}

                switch (startPort.direction)
                {
                    case Direction.Input:
                        if (port.portType == startPort.portType || port.portType.IsSubclassOf(startPort.portType) /*|| startPort.portType.IsSubclassOf(typeof(Ref<>))*/)
                        {
                            validPorts.Add(port);
                        }
                        break;
                    case Direction.Output:
                        if (port.portType == startPort.portType || startPort.portType.IsSubclassOf(port.portType) /*|| startPort.portType.IsSubclassOf(typeof(Ref<>))*/)
                        {
                            validPorts.Add(port);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                
            }
            
            return validPorts;
        }

        private void ShowSearchWindow(NodeCreationContext obj)
        {
            m_searchProvider.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), m_searchProvider);
        }

        public void Add(NodeSystemNode node)
        {
            Undo.RecordObject(SerializedObject.targetObject, "Added Node");
            
            m_nodeSystem.Nodes.Add(node);
            
            SerializedObject.Update();

            AddNodeToGraph(node);
            BindToSerializedObject();
        }

        private void AddNodeToGraph(NodeSystemNode node)
        {
            if (node == null) return;

            node.typename = node.GetType().AssemblyQualifiedName;

            NodeSystemEditorNode editorNode = new(node, SerializedObject);
            editorNode.SetPosition(node.position);
            if (!m_nodeDictionary.ContainsKey(node.id))
            {
                m_graphNodes.Add(editorNode);
                m_nodeDictionary.Add(node.id, editorNode);
            }
            AddElement(editorNode);
        }

        private void BindToSerializedObject()
        {
            SerializedObject.Update();
            this.Bind(SerializedObject);
            
        }
        
        public void UnsubscribeFromEvents()
        {
            m_blackboard.UnsubscribeFromEvents();
            Undo.undoRedoEvent -= OnUndoRedo;
            graphViewChanged -= OnGraphViewChangedEvent;
            
            // canPasteSerializedData -= CanPasteCallback;
            // unserializeAndPaste -= PasteCallback;
            // serializeGraphElements -= CopyCutCallback;
        }
    }
}
