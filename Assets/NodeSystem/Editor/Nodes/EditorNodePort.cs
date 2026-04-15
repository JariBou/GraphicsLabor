using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Nodes
{
    public class EditorNodePort : Port
    {
        public bool HideWhenConnected = true;
        private string m_propBindingPath;

        protected EditorNodePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
        }

        public string LinkedPropertyName { get; set; }


        public static EditorNodePort Create(
            Orientation orientation,
            Direction direction,
            Capacity capacity,
            Type type)
        {
            return Create<Edge>(orientation, direction, capacity, type);
        }

        public override void OnStartEdgeDragging()
        {
            base.OnStartEdgeDragging();
        }

        public override void OnStopEdgeDragging()
        {
            base.OnStopEdgeDragging();
        }

        public override void Connect(Edge edge)
        {
            Debug.LogWarning("Connect!");
            // This breaks when deleting nodes
            NotifyConnectionChanged(true);
            base.Connect(edge);
        }

        public override void Disconnect(Edge edge)
        {
            Debug.LogWarning("Disconnect!");
            // This breaks when deleting nodes
            NotifyConnectionChanged(false);
            base.Disconnect(edge);
        }

        public override void DisconnectAll()
        {
            base.DisconnectAll();
        }

        public new static EditorNodePort Create<TEdge>(
            Orientation orientation,
            Direction direction,
            Capacity capacity,
            Type type)
            where TEdge : Edge, new()
        {
            DefaultEdgeConnectorListener listener = new();
            EditorNodePort port = new(orientation, direction, capacity, type)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            intentional compilation error;
            // TODO;
            // https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-manipulators.html
            port.AddManipulator(new ClickSelector());
            port.AddManipulator(port.m_EdgeConnector);
            return port;
        }

        // Does not work as intended smh
        private void NotifyConnectionChanged(bool wasConnected)
        {
            Port port = this;
            if (port.direction == Direction.Output)
            {
                OutputConnectionChanged(wasConnected);
                return;
            }
            if (port.contentContainer == null) return;
            if (m_propBindingPath == null || !HideWhenConnected) return;
            if (port.contentContainer.childCount < 3) return;
            var portContentContainer = new List<VisualElement>();
            for (var j = 0; j < port.contentContainer.childCount; j++)
                portContentContainer.Add(port.contentContainer[j]);

            var i = 2;
            if (wasConnected)
            {
                VisualElement element = port.contentContainer[i];
                if (element is PropertyField propertyField)
                {
                    // Label tempField = new Label()
                    // {
                    //     name = propertyField.name,
                    // };
                    // portContentContainer[i] = tempField;
                    propertyField.SetEnabled(false);
                    // propertyField.visible = false;
                }
            }
            else
            {
                VisualElement element = port.contentContainer[i];
                if (element is Label labelField)
                {
                    // SerializedProperty serializedPropertyOf = ((NodeSystemEditorNode)node).GetSerializedPropertyOf(LinkedPropertyName);
                    // if (serializedPropertyOf == null) return;
                    // PropertyField tempField = new( /*serializedPropertyOf*/)
                    // {
                    //     name = labelField.name,
                    //     bindingPath = m_propBindingPath
                    // };
                    // portContentContainer[i] = tempField;
                }

                if (element is PropertyField propertyField) propertyField.SetEnabled(true);
            }

            /*for (int i = 0; i < port.contentContainer.childCount; i++)
            {
                Debug.Log(port.contentContainer[i]);
                portContentContainer.Add(port.contentContainer[i]);
                if (connected)
                {
                    VisualElement element = port.contentContainer[i];
                    if (element is PropertyField propertyField)
                    {
                        TextField tempField = new TextField()
                        {
                            name = propertyField.name,
                            bindingPath = propertyField.bindingPath,
                        };
                        portContentContainer[i] = tempField;
                    }
                }
                else
                {
                    VisualElement element = port.contentContainer[i];
                    if (element is TextField textField)
                    {
                        PropertyField tempField = new PropertyField()
                        {
                            name = textField.name,
                            bindingPath = textField.bindingPath,
                        };
                        portContentContainer[i] = tempField;
                    }
                }
            }*/
            port.contentContainer.Clear();
            foreach (VisualElement visualElement in portContentContainer) port.contentContainer.Add(visualElement);
        }

        private void OutputConnectionChanged(bool wasConnected)
        {
            if (HideWhenConnected)
            {
                VisualElement element = contentContainer[2];
                element.SetEnabled(!wasConnected);
            }
        }

        public void AddField<T>(T tempField) where T : VisualElement, IBindable
        {
            contentContainer.Add(tempField);
            m_propBindingPath = tempField.bindingPath;
        }

        private class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
            private readonly List<Edge> m_EdgesToCreate;
            private readonly List<GraphElement> m_EdgesToDelete;
            private readonly GraphViewChange m_GraphViewChange;

            public DefaultEdgeConnectorListener()
            {
                m_EdgesToCreate = new List<Edge>();
                m_EdgesToDelete = new List<GraphElement>();
                m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
            }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                /*
                 // Works but not really useful rn
                GraphView firstAncestorOfType = edge.GetFirstAncestorOfType<GraphView>();
                
                Vector2 localToWorld = firstAncestorOfType.contentViewContainer.LocalToWorld(position);
                Vector2 guiToScreenPoint = GUIUtility.GUIToScreenPoint(localToWorld);

                NodeCreationContext nodeCreationContext = new()
                {
                    screenMousePosition = guiToScreenPoint,
                };
                firstAncestorOfType.nodeCreationRequest(nodeCreationContext);
                */
            }

            public void OnDrop(GraphView graphView, Edge edge)
            {
                m_EdgesToCreate.Clear();
                m_EdgesToCreate.Add(edge);
                m_EdgesToDelete.Clear();
                if (edge.input.capacity == Capacity.Single)
                    foreach (Edge connection in edge.input.connections)
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);

                if (edge.output.capacity == Capacity.Single)
                    foreach (Edge connection in edge.output.connections)
                        if (connection != edge)
                            m_EdgesToDelete.Add(connection);

                if (m_EdgesToDelete.Count > 0)
                    graphView.DeleteElements(m_EdgesToDelete);
                var edgesToCreate = m_EdgesToCreate;
                if (graphView.graphViewChanged != null)
                    edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
                foreach (Edge edge1 in edgesToCreate)
                {
                    graphView.AddElement(edge1);
                    edge.input.Connect(edge1);
                    // ((EditorNodePort)edge.input).NotifyConnectionChanged(true);
                    edge.output.Connect(edge1);
                    // ((EditorNodePort)edge.output).NotifyConnectionChanged(true);
                }
            }
        }
    }
}