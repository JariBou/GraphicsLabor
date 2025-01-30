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
        public static bool OnOpenAsset(int instanceID, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if (asset.GetType() == typeof(NodeSystemAsset))
            {
                NodeSystemEditorWindow.Open((NodeSystemAsset)asset);
                return true;
            }
            
            return false;
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open Graph"))
            {
                NodeSystemEditorWindow.Open((NodeSystemAsset)target);
            }
        }
    }
}
