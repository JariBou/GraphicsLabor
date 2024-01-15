using GraphicsLabor.Scripts.LaborTags;
using GraphicsLabor.Scripts.LaborTags.Components;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.LaborerTags
{
    [CustomEditor(typeof(LinkedLaborTagComponent)), CanEditMultipleObjects]
    public class LinkedLaborTagInspector : UnityEditor.Editor
    {
        private SerializedProperty _tags;
        private SerializedProperty _linkedObject;
        
        void OnEnable()
        {
            // Setup the SerializedProperties.
            _tags = serializedObject.FindProperty("_tags");
            _linkedObject = serializedObject.FindProperty("_linkedGameObject");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // _tags.enumValueFlag = (int)(LaborTags)EditorGUILayout.EnumFlagsField(_tags.displayName, (LaborTags)_tags.enumValueFlag); // This works smh
            EditorGUILayout.PropertyField(_tags);
            EditorGUILayout.PropertyField(_linkedObject);

            serializedObject.ApplyModifiedProperties();
            
        }

    }
}
