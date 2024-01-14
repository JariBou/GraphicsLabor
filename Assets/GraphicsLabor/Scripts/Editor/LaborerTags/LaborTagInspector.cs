using GraphicsLabor.Scripts.Core.LaborerTags;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.LaborerTags
{
    [CustomEditor(typeof(LaborTagComponent)), CanEditMultipleObjects]
    public class LaborTagInspector : UnityEditor.Editor
    {
        private SerializedProperty _tags;
        
        void OnEnable()
        {
            // Setup the SerializedProperties.
            _tags = serializedObject.FindProperty("_tags");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_tags);

            serializedObject.ApplyModifiedProperties();
            
        }
    }
}
