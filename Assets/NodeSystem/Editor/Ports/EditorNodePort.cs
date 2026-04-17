using System;
using System.Collections.Generic;
using NodeSystem.Editor.Graph.Elements;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Ports
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

        public string LinkedPropertyName { get; set; }

        private void OnMouseDown(MouseDownEvent evt)
        {
            VisualElement target = evt.target as VisualElement;
            if (target?.name == "connector") evt.StopImmediatePropagation();
            if (IsFromListView(evt.target as VisualElement, out ListView view))
                // MouseDownEvent ms = new MouseDownEvent()
                // {
                // target = evt.target,
                // };
                evt.StopImmediatePropagation();
            // view.SendEvent(ms);
        }

        protected override void HandleEventTrickleDown(EventBase evt)
        {
            return;
            if (evt is MouseDownEvent @event) OnMouseDown(@event);
            // if (IsFromListView(evt.target as VisualElement))
            // {
            // evt.StopImmediatePropagation();
            // }
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (m_ConnectorBox == null || m_ConnectorBoxCap == null)
                return;
            if (highlight && evt is IMouseEvent)
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
                    Vector2 mouseAdjustedPos = new(evt.originalMousePosition.x,
                        evt.originalMousePosition.y - 25 /* Offset of the title bar */);
                    Vector2 adjustedPos = view.ChangeCoordinatesTo(this, mouseAdjustedPos);

                    if (ContainsPoint(adjustedPos))
                    {
                        m_ConnectorBoxCap.style.backgroundColor = portColor;
                    }
                    else
                    {
                        if (portCapLit || connected)
                            m_ConnectorBoxCap.style.backgroundColor = portColor;
                        else
                            m_ConnectorBoxCap.style.backgroundColor = StyleKeyword.Null;
                    }
                }
            }
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
            return base.ContainsPoint(localPoint);
            Rect layout1 = m_ConnectorBox.layout;
            Rect rect1 = default;
            if (direction == Direction.Input)
            {
                ref Rect local1 = ref rect1;
                double x = -(double)layout1.xMin;
                double y = -(double)layout1.yMin;
                double width1 = layout1.width + (double)layout1.xMin;
                Rect rect2 = new(0.0f, 0.0f, layout.width, layout.height);
                double height = rect2.height;
                local1 = new Rect((float)x, (float)y, (float)width1, (float)height);
                ref Rect local2 = ref rect1;
                double width2 = local2.width;
                rect2 = m_ConnectorText.layout;
                double num = rect2.xMin - (double)layout1.xMax;
                local2.width = (float)(width2 + num);
            }
            else
            {
                ref Rect local = ref rect1;
                double y = -(double)layout1.yMin;
                Rect rect3 = new(0.0f, 0.0f, layout.width, layout.height);
                double width = rect3.width - (double)layout1.xMin;
                rect3 = new Rect(0.0f, 0.0f, layout.width, layout.height);
                double height = rect3.height;
                local = new Rect(0.0f, (float)y, (float)width, (float)height);
                double xMin = layout1.xMin;
                rect3 = m_ConnectorText.layout;
                double xMax = rect3.xMax;
                float num = (float)(xMin - xMax);
                rect1.xMin -= num;
                rect1.width += num;
            }

            Vector2 changeCoordinatesTo = this.ChangeCoordinatesTo(m_ConnectorBox, localPoint);
            return rect1.Contains(changeCoordinatesTo);
        }


        public static EditorNodePort Create(
            Orientation orientation,
            Direction direction,
            Capacity capacity,
            Type type)
        {
            return Create<NsEdge>(orientation, direction, capacity, type);
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
            NsDefaultEdgeConnectorListener listener = new();
            EditorNodePort port = new(orientation, direction, capacity, type)
            {
                m_EdgeConnector = new NsEdgeConnector<TEdge>(listener)
            };
            // intentional compilation error;
            // TODO;
            // https://docs.unity3d.com/6000.3/Documentation/Manual/UIE-manipulators.html
            // port.AddManipulator(new ListViewSelector());
            // port.AddManipulator(new ClickSelector());
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
            for (int j = 0; j < port.contentContainer.childCount; j++)
                portContentContainer.Add(port.contentContainer[j]);

            int i = 2;
            if (wasConnected)
            {
                VisualElement element = port.contentContainer[i];
                if (element is PropertyField propertyField)
                    // Label tempField = new Label()
                    // {
                    //     name = propertyField.name,
                    // };
                    // portContentContainer[i] = tempField;
                    propertyField.SetEnabled(false);
                // propertyField.visible = false;
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
            if (HideWhenConnected && contentContainer.childCount > 2)
            {
                VisualElement element = contentContainer[2];
                element.SetEnabled(!wasConnected);
            }
        }

        public void AddField<T>(T tempField) where T : VisualElement, IBindable
        {
            contentContainer.Add(tempField);
            m_propBindingPath = tempField.bindingPath;
            // if (IsFromListView(tempField, out _))
            // {
            //     ((NsEdgeConnector<Edge>)edgeConnector).IsFromListView = true;
            // }
        }
    }
}