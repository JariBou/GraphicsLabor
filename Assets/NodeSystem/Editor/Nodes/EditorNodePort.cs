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
        private string m_propBindingPath;
        public bool HideWhenConnected = true;
        public string LinkedPropertyName { get; set; }

        protected EditorNodePort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type)
        {
        }



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
            // Debug.LogWarning("Connect!");
            // This breaks when deleting nodes
            // NotifyConnectionChanged(true);
            base.Connect(edge);
        }

        public override void Disconnect(Edge edge)
        {
            // Debug.LogWarning("Disconnect!");
            // This breaks when deleting nodes
            // NotifyConnectionChanged(false);
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
            EditorNodePort ele = new(orientation, direction, capacity, type)
            {
                m_EdgeConnector = new EdgeConnector<TEdge>(listener)
            };
            ele.AddManipulator(ele.m_EdgeConnector);
            return ele;
        }

        public class DefaultEdgeConnectorListener : IEdgeConnectorListener
        {
          private GraphViewChange m_GraphViewChange;
          private List<Edge> m_EdgesToCreate;
          private List<GraphElement> m_EdgesToDelete;
    
          public DefaultEdgeConnectorListener()
          {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;
          }
    
          public void OnDropOutsidePort(Edge edge, Vector2 position)
          {
          }
    
          public void OnDrop(GraphView graphView, Edge edge)
          {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single)
            {
              foreach (Edge connection in edge.input.connections)
              {
                if (connection != edge)
                  m_EdgesToDelete.Add(connection);
              }
            }
            if (edge.output.capacity == Capacity.Single)
            {
              foreach (Edge connection in edge.output.connections)
              {
                if (connection != edge)
                  m_EdgesToDelete.Add(connection);
              }
            }
            if (m_EdgesToDelete.Count > 0)
              graphView.DeleteElements(m_EdgesToDelete);
            List<Edge> edgesToCreate = m_EdgesToCreate;
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

        // Does not work as intended smh
        private void NotifyConnectionChanged(bool connected)
        {
            Port port = this;
            if (port.contentContainer == null) return;
            if (m_propBindingPath == null || !HideWhenConnected) return;
            if (port.contentContainer.childCount < 3) return;
            List<VisualElement> portContentContainer = new List<VisualElement>();
            for (int j = 0; j < port.contentContainer.childCount; j++)
            {
                portContentContainer.Add(port.contentContainer[j]);
            }

            int i = 2;
            if (connected)
            {
                VisualElement element = port.contentContainer[i];
                if (element is PropertyField propertyField)
                {
                    // Label tempField = new Label()
                    // {
                    //     name = propertyField.name,
                    // };
                    // portContentContainer[i] = tempField;
                    propertyField.visible = false;
                } 
            }
            else
            {
                VisualElement element = port.contentContainer[i];
                if (element is Label labelField)
                {
                    // SerializedProperty serializedPropertyOf = ((NodeSystemEditorNode)node).GetSerializedPropertyOf(LinkedPropertyName);
                    // if (serializedPropertyOf == null) return;
                    PropertyField tempField = new PropertyField(/*serializedPropertyOf*/)
                    {
                        name = labelField.name,
                        bindingPath = m_propBindingPath,
                    };
                    portContentContainer[i] = tempField;
                }

                if (element is PropertyField propertyField)
                {
                    propertyField.visible = true;
                }
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
            foreach (VisualElement visualElement in portContentContainer)
            {
                port.contentContainer.Add(visualElement);
            }
        }

        public void AddField<T>(T tempField) where T : VisualElement, IBindable
        {
            contentContainer.Add(tempField);
            m_propBindingPath = tempField.bindingPath;
        }
    }
    
    
}