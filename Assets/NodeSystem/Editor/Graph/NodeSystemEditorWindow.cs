using NodeSystem.Editor.Graph.View;
using NodeSystem.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Editor.Graph
{
    public class NodeSystemEditorWindow : EditorWindow
    {
        [FormerlySerializedAs("m_currentGraph"), SerializeField]
        private NodeSystemAsset _currentGraph;

        private NodeSystemView _currentView;

        private SerializedObject _serializedObject;

        public NodeSystemAsset CurrentGraph => _currentGraph;

        private void OnEnable()
        {
            if (_currentGraph != null) Load(_currentGraph);
        }

        private void OnDisable()
        {
            _currentView?.UnsubscribeFromEvents();
        }

        private void OnGUI()
        {
            if (_currentGraph is not null) hasUnsavedChanges = EditorUtility.IsDirty(_currentGraph);
        }

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

            NodeSystemEditorWindow newWindow =
                CreateWindow<NodeSystemEditorWindow>(typeof(NodeSystemEditorWindow), typeof(SceneView));
            newWindow.titleContent = new GUIContent($"{graph.name}");
            newWindow.Load(graph);
        }

        private void Load(NodeSystemAsset graph)
        {
            _currentGraph = graph;
            DrawGraph();
        }

        private void DrawGraph()
        {
            _serializedObject = new SerializedObject(_currentGraph);
            _currentView = new NodeSystemView(_serializedObject, this);
            _currentView.graphViewChanged += OnChange;
            rootVisualElement.Add(_currentView);
        }

        private GraphViewChange OnChange(GraphViewChange graphViewChange)
        {
            hasUnsavedChanges = true;
            EditorUtility.SetDirty(_currentGraph);
            return graphViewChange;
        }
    }
}