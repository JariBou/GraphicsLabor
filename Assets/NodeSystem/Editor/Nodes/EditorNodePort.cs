using System;
using System.Collections.Generic;
using NodeSystem.Editor.Graph;
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
            // RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            VisualElement target = evt.target as VisualElement;
            if (target?.name == "connector")
            {
                evt.StopImmediatePropagation();
            }
            if (IsFromListView(evt.target as VisualElement, out ListView view))
            {
                // MouseDownEvent ms = new MouseDownEvent()
                // {
                    // target = evt.target,
                // };
                evt.StopImmediatePropagation();
                // view.SendEvent(ms);
            }
        }

        protected override void HandleEventTrickleDown(EventBase evt)
        {
            return;
            if (evt is MouseDownEvent @event)
            {
                OnMouseDown(@event);
            }
            // if (IsFromListView(evt.target as VisualElement))
            // {
                // evt.StopImmediatePropagation();
            // }
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (evt.eventTypeId == EventBase<MouseMoveEvent>.TypeId())
            {
                VisualElement visualElement = node.parent;
                GraphView view = null;
                while (visualElement != null)
                {
                    if (visualElement is GraphView graphView)
                    {
                        view = graphView;
                        break;
                    }

                    visualElement = visualElement.parent;
                }

                if (view != null)
                {
                    // Zoom has an influence on Y value
                    Vector2 adjustedPos = view.ChangeCoordinatesTo(this, evt.originalMousePosition);
                    Debug.Log(adjustedPos);
                    // adjustedPos *= view.panel.visualTree.layout.size;
                    Debug.Log($"IsPointInside: {ContainsPoint(adjustedPos)}");
                }
                // Debug.Log($"IsPointInside: {ContainsPoint(node.ChangeCoordinatesTo(this, evt.originalMousePosition))}");
            }
            base.HandleEventBubbleUp(evt);
            // if (m_ConnectorBox == null || m_ConnectorBoxCap == null)
            //     return;
            // if (evt.eventTypeId == EventBase<MouseEnterEvent>.TypeId())
            // {
            //     Debug.Log("Entering!");
            // }
            //
            // if (evt.eventTypeId == EventBase<MouseMoveEvent>.TypeId())
            // {
            //     Debug.Log("Moving!");
            // }
            //
            // if (highlight && ContainsPoint(evt.originalMousePosition))
            // {
            //     if (evt.eventTypeId == EventBase<MouseEnterEvent>.TypeId())
            //     {
            //         m_ConnectorBoxCap.style.backgroundColor = portColor;
            //     }
            //     else
            //     {
            //         if (evt.eventTypeId != EventBase<MouseLeaveEvent>.TypeId())
            //             return;
            //         ResetCapColor();
            //     }
            // }
            // else
            // {
            //     if (evt.eventTypeId != EventBase<MouseUpEvent>.TypeId() || layout.Contains(((MouseEventBase<MouseUpEvent>) evt).localMousePosition))
            //         return;
            //     ResetCapColor();
            // }
        }

        private void ResetCapColor()
        {
            if (portCapLit || connected)
                m_ConnectorBoxCap.style.backgroundColor = portColor;
            else
                m_ConnectorBoxCap.style.backgroundColor = StyleKeyword.Null;
        }

        private bool IsFromListView(VisualElement target, out ListView listView)
        {
            listView = null;
            VisualElement current = target;
            while (current != null)
            {
                if (current is ListView view)
                {
                    listView = view;
                    return true;
                }
                current = current.parent;
            }
            return false;
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            Rect layout = this.m_ConnectorBox.layout;
            Rect rect1 = default;
            if (this.direction == Direction.Input)
            {
                ref Rect local1 = ref rect1;
                double x = -(double) layout.xMin;
                double y = -(double) layout.yMin;
                double width1 = (double) layout.width + (double) layout.xMin;
                Rect rect2 = new Rect(0.0f, 0.0f, this.layout.width, this.layout.height);
                double height = (double) rect2.height;
                local1 = new Rect((float) x, (float) y, (float) width1, (float) height);
                ref Rect local2 = ref rect1;
                double width2 = (double) local2.width;
                rect2 = this.m_ConnectorText.layout;
                double num = (double) rect2.xMin - (double) layout.xMax;
                local2.width = (float) (width2 + num);
            }
            else
            {
                ref Rect local = ref rect1;
                double y = -(double) layout.yMin;
                Rect rect3 = new Rect(0.0f, 0.0f, this.layout.width, this.layout.height);
                double width = (double) rect3.width - (double) layout.xMin;
                rect3 = new Rect(0.0f, 0.0f, this.layout.width, this.layout.height);
                double height = (double) rect3.height;
                local = new Rect(0.0f, (float) y, (float) width, (float) height);
                double xMin = (double) layout.xMin;
                rect3 = this.m_ConnectorText.layout;
                double xMax = (double) rect3.xMax;
                float num = (float) (xMin - xMax);
                rect1.xMin -= num;
                rect1.width += num;
            }
            return rect1.Contains(this.ChangeCoordinatesTo(this.m_ConnectorBox, localPoint));
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
            // intentional compilation error;
            // TODO;
            // https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-manipulators.html
            port.AddManipulator(new ListViewSelector());
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