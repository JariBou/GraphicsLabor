using NodeSystem.Editor.Graph.View;
using NodeSystem.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSystem.Editor.Graph
{
    public class NodeSystemEditorWindow : EditorWindow
    {
    
        [SerializeField]
        private NodeSystemAsset m_currentGraph;
        public NodeSystemAsset CurrentGraph => m_currentGraph;
        [SerializeField]
        private SerializedObject m_serializedObject;
        [SerializeField]
        private NodeSystemView m_currentView;
    
        public static void Open(NodeSystemAsset graph)
        {
            NodeSystemEditorWindow[] windows = Resources.FindObjectsOfTypeAll<NodeSystemEditorWindow>();
            foreach (NodeSystemEditorWindow window in windows)
            {
                if (window.CurrentGraph == graph)
                {
                    window.Focus();
                    return;
                }
            }
        
            NodeSystemEditorWindow newWindow = CreateWindow<NodeSystemEditorWindow>(typeof(NodeSystemEditorWindow), typeof(SceneView));
            newWindow.titleContent = new GUIContent($"{graph.name}");
            newWindow.Load(graph);
        }

        private void Load(NodeSystemAsset graph)
        {   
            m_currentGraph = graph;
            DrawGraph();
        }

        private void DrawGraph()
        {
            m_serializedObject = new SerializedObject(m_currentGraph);
            m_currentView = new NodeSystemView(m_serializedObject, this);
            m_currentView.graphViewChanged += OnChange;
            rootVisualElement.Add(m_currentView);
        }

        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            hasUnsavedChanges = true;
            EditorUtility.SetDirty(m_currentGraph);
            return graphViewChange;
        }

        private void OnGUI()
        {
            if (m_currentGraph is not null)
            {
                hasUnsavedChanges = EditorUtility.IsDirty(m_currentGraph);
            }
        }

        private void OnEnable()
        {
            if (m_currentGraph != null)
            {
                Load(m_currentGraph);
            }
        }

        private void OnDisable()
        {
            m_currentView?.UnsubscribeFromEvents();
        }
    }
}
