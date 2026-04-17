using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NodeSystem.Editor.Graph.Elements;
using NodeSystem.Editor.Nodes;
using NodeSystem.Runtime;
using NodeSystem.Runtime.BlackBoard;
using NodeSystem.Runtime.NodesLibrary.Process;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph.View
{
    public partial class NodeSystemView : GraphView
    {
        private Vector2 _mousePos;

        private NodeSystemBlackboard m_blackboard;
        public Dictionary<Edge, NodeSystemConnection> m_connectionsDictionary;

        public List<NodeSystemEditorNode> m_graphNodes;
        public Dictionary<string, NodeSystemEditorNode> m_nodeDictionary;
        private readonly NodeSystemAsset m_nodeSystem;

        private readonly NodeSystemWindowSearchProvider m_searchProvider;

        public NodeSystemView(SerializedObject serializedObject, NodeSystemEditorWindow window)
        {
            SerializedObject = serializedObject;
            this.window = window;
            m_nodeSystem = (NodeSystemAsset)serializedObject.targetObject;

            m_graphNodes = new List<NodeSystemEditorNode>();
            m_nodeDictionary = new Dictionary<string, NodeSystemEditorNode>();
            m_connectionsDictionary = new Dictionary<Edge, NodeSystemConnection>();
            ExposedProperties = m_nodeSystem.ExposedProperties;

            m_searchProvider = ScriptableObject.CreateInstance<NodeSystemWindowSearchProvider>();
            m_searchProvider.graph = this;
            nodeCreationRequest = ShowSearchWindow;

            StyleSheet styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeSystem/Editor/USS/NodeSystemEditor.uss");
            styleSheets.Add(styleSheet);

            GridBackground background = new()
            {
                name = "Grid"
            };
            Add(background);
            background.SendToBack();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());
            RegisterCallback<MouseMoveEvent>(OnMouseMoved);

            SetupZoom(0.4f, 2.0f);

            AddMinimap(window);
            GenerateBlackBoard();

            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphViewChangedEvent;
            Undo.undoRedoEvent += OnUndoRedo;

            // schedule.Execute(_ => FrameAll());
            // Neither work smh
            // EditorApplication.delayCall += () =>
            // {
            //     FrameAll();
            // };
            // schedule.Execute(() => { FrameAll(); });

            SubscribeToCopyCutPaste();
        }

        public NodeSystemEditorWindow window { get; }

        public List<BlackboardProperty> ExposedProperties { get; }

        public SerializedObject SerializedObject { get; }

        private void OnMouseMoved(MouseMoveEvent evt)
        {
            _mousePos = evt.localMousePosition;
        }


        private void AddMinimap(NodeSystemEditorWindow editorWindow)
        {
            MiniMap miniMap = new()
            {
                anchored = false
            };
            miniMap.SetPosition(new Rect(editorWindow.position.width - 200 - 15, 20, 200, 180));
            Add(miniMap);
        }

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            var changedPorts = new List<Port>();
            // Debug.Log("OnGraphViewChangedEvent");
            if (graphViewChange.movedElements != null)
            {
                Undo.RecordObject(SerializedObject.targetObject, "Moved Nodes");
                foreach (NodeSystemEditorNode editorNode in
                         graphViewChange.movedElements.OfType<NodeSystemEditorNode>()) editorNode.UpdatePosition();
            }

            if (graphViewChange.elementsToRemove != null)
            {
                var nodesToRemove = graphViewChange.elementsToRemove.OfType<NodeSystemEditorNode>().ToList();
                if (nodesToRemove.Count > 0)
                {
                    Undo.RecordObject(SerializedObject.targetObject, "Removed Node");

                    for (int i = nodesToRemove.Count() - 1; i >= 0; i--) RemoveNode(nodesToRemove[i]);
                }

                var edgesToRemove = graphViewChange.elementsToRemove.OfType<Edge>().ToList();
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

            return graphViewChange;
        }

        private void OnUndoRedo(in UndoRedoInfo undo)
        {
            Debug.Log("OnUndoRedo");

            #region Nodes

            var currentEditorNodes = new List<NodeSystemEditorNode>(m_graphNodes);
            var neededNodesList = new List<NodeSystemNode>(m_nodeSystem.Nodes);
            for (int i = 0; i < m_graphNodes.Count; i++)
            {
                NodeSystemEditorNode editorNode = m_graphNodes[i];
                if (m_nodeSystem.Nodes.Contains(editorNode.Node))
                {
                    NodeSystemNode nodeSystemNode = neededNodesList.Find(systemNode => systemNode == editorNode.Node);
                    editorNode.SetPosition(nodeSystemNode.Position);
                    neededNodesList.Remove(editorNode.Node);
                    currentEditorNodes.Remove(editorNode);
                }
            }

            if (neededNodesList.Count > 0)
                foreach (NodeSystemNode node in neededNodesList)
                {
                    AddNodeToGraph(node);
                    BindToSerializedObject();
                }

            if (currentEditorNodes.Count > 0)
                foreach (NodeSystemEditorNode editorNode in currentEditorNodes)
                {
                    RemoveElement(editorNode);
                    RemoveNode(editorNode);
                }

            #endregion

            #region Connections

            var neededConnections = new List<NodeSystemConnection>(m_nodeSystem.Connections);
            var graphConnections = new List<NodeSystemConnection>(m_connectionsDictionary.Values);

            foreach (NodeSystemConnection connection in m_connectionsDictionary.Values)
                if (neededConnections.Contains(connection))
                {
                    neededConnections.Remove(connection);
                    graphConnections.Remove(connection);
                }

            if (graphConnections.Count > 0)
                for (int i = 0; i < graphConnections.Count; i++)
                {
                    Edge edgeToRemove =
                        m_connectionsDictionary.Keys.ToList()[
                            m_connectionsDictionary.Values.ToList().IndexOf(graphConnections[i])];
                    edgeToRemove.input.Disconnect(edgeToRemove);
                    edgeToRemove.output.Disconnect(edgeToRemove);
                    RemoveElement(edgeToRemove);
                    RemoveConnection(edgeToRemove);
                }

            if (neededConnections.Count > 0)
                foreach (NodeSystemConnection connection in neededConnections)
                {
                    NsEdge edgeToCreate = new()
                    {
                        input = GetNode(connection.InputPort.NodeId).Ports[connection.InputPort.PortIndex],
                        output = GetNode(connection.OutputPort.NodeId).Ports[connection.OutputPort.PortIndex]
                    };
                    edgeToCreate.input.Connect(edgeToCreate);
                    edgeToCreate.output.Connect(edgeToCreate);
                    AddElement(edgeToCreate);
                    m_connectionsDictionary.Add(edgeToCreate, connection);
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

        internal void RemoveConnection(Edge edge)
        {
            if (m_connectionsDictionary.TryGetValue(edge, out NodeSystemConnection connection))
            {
                m_nodeSystem.Connections.Remove(connection);
                m_connectionsDictionary.Remove(edge);
            }
        }

        private void DrawNodes()
        {
            foreach (NodeSystemNode node in m_nodeSystem.Nodes) AddNodeToGraph(node);

            if (m_nodeSystem.Nodes.Count == 0)
            {
                StartNode startNode = new();
                //startNode.SetPosition();
                Add(startNode);
            }

            BindToSerializedObject();
        }

        private void DrawConnections()
        {
            if (m_nodeSystem.Connections == null) return;

            foreach (NodeSystemConnection connection in m_nodeSystem.Connections) DrawConnection(connection);
        }

        private void DrawConnection(NodeSystemConnection connection)
        {
            NodeSystemEditorNode inputNode = GetNode(connection.InputPort.NodeId);
            if (inputNode == null) return;
            NodeSystemEditorNode outputNode = GetNode(connection.OutputPort.NodeId);
            if (outputNode == null) return;

            Port inputPort = inputNode.Ports[connection.InputPort.PortIndex];
            Port outputPort = outputNode.Ports[connection.OutputPort.PortIndex];

            Edge edge = inputPort.ConnectTo<NsEdge>(outputPort);
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
            var allPorts = new List<Port>();
            var validPorts = new List<Port>();


            foreach (NodeSystemEditorNode editorNode in m_graphNodes) allPorts.AddRange(editorNode.Ports);

            foreach (Port port in allPorts)
            {
                if (port == startPort) continue;
                if (port.node == startPort.node) continue;
                if (port.direction == startPort.direction) continue;

                switch (startPort.direction)
                {
                    case Direction.Input:
                        if (port.portType == startPort.portType ||
                            port.portType.IsSubclassOf(startPort
                                .portType) /*|| startPort.portType.IsSubclassOf(typeof(Ref<>))*/) validPorts.Add(port);
                        break;
                    case Direction.Output:
                        if (port.portType == startPort.portType ||
                            startPort.portType
                                .IsSubclassOf(port.portType) /*|| startPort.portType.IsSubclassOf(typeof(Ref<>))*/
                           ) validPorts.Add(port);
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

        [CanBeNull]
        private NodeSystemEditorNode AddNodeToGraph(NodeSystemNode node)
        {
            if (node == null) return null;

            node.typename = node.GetType().AssemblyQualifiedName;

            NodeSystemEditorNode editorNode = new(node, SerializedObject);
            editorNode.SetPosition(node.Position);
            if (!m_nodeDictionary.ContainsKey(node.id))
            {
                m_graphNodes.Add(editorNode);
                m_nodeDictionary.Add(node.id, editorNode);
            }

            AddElement(editorNode);
            return editorNode;
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

            RemoveCopyCutPasteCallbacks();
        }
    }
}