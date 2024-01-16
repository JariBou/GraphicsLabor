using GraphicsLabor.Scripts.Core.Tags.Components;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Tags
{
    [CustomEditor(typeof(LinkedLaborTagComponent)), CanEditMultipleObjects]
    public class LinkedLaborTagInspector : UnityEditor.Editor
    {
        private SerializedProperty _tags;
        private SerializedProperty _linkedObject;
        private SerializedProperty _linkedScript;
        
        void OnEnable()
        {
            // Setup the SerializedProperties.
            _tags = serializedObject.FindProperty("_tags");
            _linkedObject = serializedObject.FindProperty("_linkedGameObject");
            _linkedScript = serializedObject.FindProperty("_linkedMonoBehaviour");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // _tags.enumValueFlag = (int)(LaborTags)EditorGUILayout.EnumFlagsField(_tags.displayName, (LaborTags)_tags.enumValueFlag); // This works smh
            EditorGUILayout.PropertyField(_tags);
            EditorGUILayout.PropertyField(_linkedObject);
            EditorGUILayout.PropertyField(_linkedScript);

            serializedObject.ApplyModifiedProperties();
            
        }

    }
}
