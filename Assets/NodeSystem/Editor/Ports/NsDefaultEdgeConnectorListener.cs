using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSystem.Editor.Ports
{
    public class NsDefaultEdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly List<Edge> m_EdgesToCreate;
        private readonly List<GraphElement> m_EdgesToDelete;
        private readonly GraphViewChange m_GraphViewChange;

        public NsDefaultEdgeConnectorListener()
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
            if (edge.input.capacity == Port.Capacity.Single)
                foreach (Edge connection in edge.input.connections)
                    if (connection != edge)
                        m_EdgesToDelete.Add(connection);

            if (edge.output.capacity == Port.Capacity.Single)
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