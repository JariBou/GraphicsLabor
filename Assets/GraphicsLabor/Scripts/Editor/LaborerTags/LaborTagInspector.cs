﻿using GraphicsLabor.Scripts.LaborTags;
using GraphicsLabor.Scripts.LaborTags.Components;
using UnityEditor;

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

            // _tags.enumValueFlag = (int)(LaborTags)EditorGUILayout.EnumFlagsField(_tags.displayName, (LaborTags)_tags.enumValueFlag); // This works smh
            EditorGUILayout.PropertyField(_tags);

            serializedObject.ApplyModifiedProperties();
            
        }

    }
}
