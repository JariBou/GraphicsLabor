using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Dictionary<Edge, NodeSystemConnection> _connectionsDictionary;

        private readonly List<NodeSystemEditorNode> _graphNodes;
        private readonly Dictionary<string, NodeSystemEditorNode> _nodeDictionary;
        private readonly NodeSystemAsset _nodeSystem;
        private readonly NodeSystemWindowSearchProvider _searchProvider;

        private NodeSystemBlackboard _blackboard;
        private Vector2 _mousePos;

        public NodeSystemEditorWindow Window { get; }

        public List<BlackboardProperty> ExposedProperties { get; }

        private SerializedObject SerializedObject { get; }


        public NodeSystemView(SerializedObject serializedObject, NodeSystemEditorWindow window)
        {
            foreach (GraphElement graphElement in graphElements)
            {
                RemoveElement(graphElement); // Justin Case
            }

            SerializedObject = serializedObject;
            Window = window;
            _nodeSystem = (NodeSystemAsset)serializedObject.targetObject;

            _graphNodes = new List<NodeSystemEditorNode>();
            _nodeDictionary = new Dictionary<string, NodeSystemEditorNode>();
            _connectionsDictionary = new Dictionary<Edge, NodeSystemConnection>();
            ExposedProperties = _nodeSystem.ExposedProperties;

            _searchProvider = ScriptableObject.CreateInstance<NodeSystemWindowSearchProvider>();
            _searchProvider.graph = this;
            nodeCreationRequest = ShowSearchWindow;

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/NodeSystem/Editor/USS/NodeSystemEditor.uss");
            styleSheets.Add(styleSheet);

            GridBackground background = new()
            {
                name = "Grid",
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
            // GenerateBlackBoard();

            DrawNodes();
            DrawConnections();

            graphViewChanged += OnGraphViewChangedEvent;
            Undo.undoRedoEvent += OnUndoRedo;

            SubscribeToCopyCutPaste();
        }

        private void OnMouseMoved(MouseMoveEvent evt)
        {
            _mousePos = evt.localMousePosition;
        }

        private void AddMinimap(NodeSystemEditorWindow editorWindow)
        {
            MiniMap miniMap = new()
            {
                anchored = false,
            };
            miniMap.SetPosition(new Rect(editorWindow.position.width - 200 - 15, 20, 200, 180));
            Add(miniMap);
        }

        private GraphViewChange OnGraphViewChangedEvent(GraphViewChange graphViewChange)
        {
            if (graphViewChange.movedElements != null)
            {
                RecordAction("Moved Nodes");
                foreach (NodeSystemEditorNode editorNode in
                         graphViewChange.movedElements.OfType<NodeSystemEditorNode>())
                {
                    editorNode.UpdatePosition();
                }
            }

            if (graphViewChange.elementsToRemove != null)
            {
                List<NodeSystemEditorNode> nodesToRemove =
                    graphViewChange.elementsToRemove.OfType<NodeSystemEditorNode>().ToList();
                if (nodesToRemove.Count > 0)
                {
                    RecordAction("Removed Node");
                    for (int i = nodesToRemove.Count - 1; i >= 0; i--)
                    {
                        RemoveNode(nodesToRemove[i]);
                    }
                }

                List<Edge> edgesToRemove = graphViewChange.elementsToRemove.OfType<Edge>().ToList();
                if (edgesToRemove.Any())
                {
                    RecordAction("Removed Connection");
                    foreach (Edge edge in edgesToRemove)
                    {
                        Debug.Log("Removing Edge");
                        RemoveConnection(edge);
                    }
                }
            }

            if (graphViewChange.edgesToCreate != null)
            {
                RecordAction("Connected Nodes");
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
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

            List<NodeSystemEditorNode> currentEditorNodes = new(_graphNodes);
            List<NodeSystemNode> neededNodesList = new(_nodeSystem.Nodes);
            foreach (NodeSystemEditorNode editorNode in _graphNodes)
            {
                if (!_nodeSystem.Nodes.Contains(editorNode.Node)) continue;

                NodeSystemEditorNode node = editorNode;
                NodeSystemNode nodeSystemNode = neededNodesList.Find(systemNode => systemNode == node.Node);
                editorNode.SetPosition(nodeSystemNode.Position);
                neededNodesList.Remove(editorNode.Node);
                currentEditorNodes.Remove(editorNode);
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

            List<NodeSystemConnection> neededConnections = new(_nodeSystem.Connections);
            List<NodeSystemConnection> graphConnections = new(_connectionsDictionary.Values);

            foreach (NodeSystemConnection connection in _connectionsDictionary.Values.Where(neededConnections.Contains))
            {
                neededConnections.Remove(connection);
                graphConnections.Remove(connection);
            }

            if (graphConnections.Count > 0)
            {
                foreach (Edge edgeToRemove in graphConnections.Select(con => _connectionsDictionary.Keys.ToList()[
                                                                          _connectionsDictionary.Values.ToList()
                                                                                                .IndexOf(con)]))
                {
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
                    NsEdge edgeToCreate = new(connection, GetNode);
                    edgeToCreate.input.Connect(edgeToCreate);
                    edgeToCreate.output.Connect(edgeToCreate);
                    AddElement(edgeToCreate);
                    _connectionsDictionary.Add(edgeToCreate, connection);
                }
            }

            #endregion
        }

        private void CreateConnection(Edge edge)
        {
            NodeSystemEditorNode inputNode = (NodeSystemEditorNode)edge.input.node;
            int inputIndex = inputNode.GetIndexOfPort(edge.input);

            NodeSystemEditorNode outputNode = (NodeSystemEditorNode)edge.output.node;
            int outputIndex = outputNode.GetIndexOfPort(edge.output);

            NodeSystemConnection connection = new(inputNode.Node.ID, inputIndex, outputNode.Node.ID, outputIndex);
            _nodeSystem.Connections.Add(connection);
            _connectionsDictionary.Add(edge, connection);
        }

        private void RemoveNode(NodeSystemEditorNode editorNode)
        {
            _nodeSystem.Nodes.Remove(editorNode.Node);
            _nodeDictionary.Remove(editorNode.Node.ID);
            _graphNodes.Remove(editorNode);
            SerializedObject.Update();
        }

        private void RemoveConnection(Edge edge)
        {
            if (!_connectionsDictionary.TryGetValue(edge, out NodeSystemConnection connection)) return;

            _nodeSystem.Connections.Remove(connection);
            _connectionsDictionary.Remove(edge);
        }

        private void DrawNodes()
        {
            foreach (NodeSystemNode node in _nodeSystem.Nodes)
            {
                AddNodeToGraph(node);
            }

            if (_nodeSystem.Nodes.Count == 0)
            {
                StartNode startNode = new();
                //startNode.SetPosition();
                Add(startNode, false);
            }

            BindToSerializedObject();
        }

        private void DrawConnections()
        {
            if (_nodeSystem.Connections == null) return;

            foreach (NodeSystemConnection connection in _nodeSystem.Connections)
            {
                DrawConnection(connection);
            }
        }

        private void DrawConnection(NodeSystemConnection connection)
        {
            NodeSystemEditorNode inputNode = GetNode(connection.inputPort.nodeId);
            if (inputNode == null) return;

            NodeSystemEditorNode outputNode = GetNode(connection.outputPort.nodeId);
            if (outputNode == null) return;

            Port inputPort = inputNode.Ports[connection.inputPort.portIndex];
            Port outputPort = outputNode.Ports[connection.outputPort.portIndex];

            Edge edge = inputPort.ConnectTo<NsEdge>(outputPort);
            _connectionsDictionary.Add(edge, connection);
            AddElement(edge);
        }

        private NodeSystemEditorNode GetNode(string nodeId)
        {
            _nodeDictionary.TryGetValue(nodeId, out NodeSystemEditorNode node);
            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> allPorts = new();
            List<Port> validPorts = new();


            foreach (NodeSystemEditorNode editorNode in _graphNodes)
            {
                allPorts.AddRange(editorNode.Ports);
            }

            foreach (Port port in from port in allPorts
                                  where port != startPort
                                  where port.node != startPort.node
                                  where port.direction != startPort.direction
                                  select port)
            {
                switch (startPort.direction)
                {
                    case Direction.Input:
                        if (port.portType == startPort.portType || port.portType.IsSubclassOf(startPort.portType)) validPorts.Add(port);
                        break;
                    case Direction.Output:
                        if (port.portType == startPort.portType || startPort.portType.IsSubclassOf(port.portType)) validPorts.Add(port);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return validPorts;
        }

        private void ShowSearchWindow(NodeCreationContext obj)
        {
            _searchProvider.target = (VisualElement)focusController.focusedElement;
            SearchWindow.Open(new SearchWindowContext(obj.screenMousePosition), _searchProvider);
        }

        public void Add(NodeSystemNode node, bool autoRecord = true)
        {
            if (autoRecord) RecordAction("Added Node");

            _nodeSystem.Nodes.Add(node);

            SerializedObject.Update();

            AddNodeToGraph(node);
            BindToSerializedObject();
        }

        private void AddNodeToGraph(NodeSystemNode node)
        {
            if (node == null) return;

            NodeSystemEditorNode editorNode = new(node, SerializedObject);
            editorNode.SetPosition(node.Position);
            if (!_nodeDictionary.ContainsKey(node.ID))
            {
                _graphNodes.Add(editorNode);
                _nodeDictionary.Add(node.ID, editorNode);
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
            _blackboard?.UnsubscribeFromEvents();
            Undo.undoRedoEvent -= OnUndoRedo;
            graphViewChanged -= OnGraphViewChangedEvent;

            RemoveCopyCutPasteCallbacks();
        }

        public void DeleteConnection(Edge edge)
        {
            edge.output.Disconnect(edge);
            edge.input.Disconnect(edge);
            RemoveConnection(edge);
            RemoveElement(edge);
            ClearSelection();
        }

        public void RecordAction(string actionName)
        {
            Undo.RecordObject(SerializedObject.targetObject, actionName);
        }
    }
}