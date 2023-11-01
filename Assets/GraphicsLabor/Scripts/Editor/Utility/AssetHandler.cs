using GraphicsLabor.Scripts.Editor.ScriptableObjectParents;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEditor;
using UnityEditor.Callbacks;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class AssetHandler
    {
        [OnOpenAsset]
        private static bool OpenEditor(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is EditableScriptableObject obj) ScriptableObjectEditorWindow.ShowWindow(obj);
            
            // not handled by us
            return false;
        }
    }
}