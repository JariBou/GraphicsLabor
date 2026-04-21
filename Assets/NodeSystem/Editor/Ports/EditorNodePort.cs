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
        private string _propBindingPath;


        private EditorNodePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type)
            : base(portOrientation, portDirection, portCapacity, type)
        {
            // RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        public string LinkedPropertyName { get; set; }

        public bool HideWhenConnected { get; set; } = true;

        protected override void HandleEventTrickleDown(EventBase evt)
        {
        }

        protected override void HandleEventBubbleUp(EventBase evt)
        {
            if (m_ConnectorBox == null || m_ConnectorBoxCap == null)
                return;
            if (!highlight || evt is not IMouseEvent) return;

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

            if (view == null) return;

            Vector2 mouseAdjustedPos = new(evt.originalMousePosition.x,
                evt.originalMousePosition.y - 25 /* Offset of the title bar */);
            Vector2 adjustedPos = view.ChangeCoordinatesTo(this, mouseAdjustedPos);

            if (ContainsPoint(adjustedPos) || portCapLit || connected)
                m_ConnectorBoxCap.style.backgroundColor = portColor;
            else
                m_ConnectorBoxCap.style.backgroundColor = StyleKeyword.Null;
        }

        public static EditorNodePort Create(
            Orientation orientation,
            Direction direction,
            Capacity capacity,
            Type type)
        {
            return Create<NsEdge>(orientation, direction, capacity, type);
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
            if (_propBindingPath == null || !HideWhenConnected) return;
            if (port.contentContainer.childCount < 3) return;
            List<VisualElement> portContentContainer = new();
            for (int j = 0; j < port.contentContainer.childCount; j++)
                portContentContainer.Add(port.contentContainer[j]);

            const int i = 2;

            VisualElement element = port.contentContainer[i];

            if (element is PropertyField propertyField) propertyField.SetEnabled(!wasConnected);

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
            _propBindingPath = tempField.bindingPath;
            // if (IsFromListView(tempField, out _))
            // {
            //     ((NsEdgeConnector<Edge>)edgeConnector).IsFromListView = true;
            // }
        }
    }
}