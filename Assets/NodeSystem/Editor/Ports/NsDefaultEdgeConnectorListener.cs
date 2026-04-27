using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSystem.Editor.Ports
{
    public class NsDefaultEdgeConnectorListener : IEdgeConnectorListener
    {
        private readonly List<Edge> _edgesToCreate;
        private readonly List<GraphElement> _edgesToDelete;
        private readonly GraphViewChange _graphViewChange;

        public NsDefaultEdgeConnectorListener()
        {
            _edgesToCreate = new List<Edge>();
            _edgesToDelete = new List<GraphElement>();
            _graphViewChange.edgesToCreate = _edgesToCreate;
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
            _edgesToCreate.Clear();
            _edgesToCreate.Add(edge);
            _edgesToDelete.Clear();
            if (edge.input.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge) _edgesToDelete.Add(connection);
                }
            }

            if (edge.output.capacity == Port.Capacity.Single)
            {
                foreach (Edge connection in edge.output.connections)
                {
                    if (connection != edge) _edgesToDelete.Add(connection);
                }
            }

            if (_edgesToDelete.Count > 0) graphView.DeleteElements(_edgesToDelete);
            List<Edge> edgesToCreate = _edgesToCreate;
            if (graphView.graphViewChanged != null) edgesToCreate = graphView.graphViewChanged(_graphViewChange).edgesToCreate;
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