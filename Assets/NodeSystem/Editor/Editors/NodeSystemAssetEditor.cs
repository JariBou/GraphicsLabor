using NodeSystem.Editor.Graph;
using NodeSystem.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace NodeSystem.Editor.Editors
{
    [CustomEditor(typeof(NodeSystemAsset))]
    public class NodeSystemAssetEditor : UnityEditor.Editor
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(EntityId instanceID, int index)
        {
            Object asset = EditorUtility.EntityIdToObject(instanceID);
            if (asset.GetType() != typeof(NodeSystemAsset)) return false;

            NodeSystemEditorWindow.Open((NodeSystemAsset)asset);
            return true;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Open Graph")) NodeSystemEditorWindow.Open((NodeSystemAsset)target);
        }
    }
}