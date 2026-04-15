using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils.RefSystem;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Editor.Editors.RefEditors
{
    [CustomPropertyDrawer(typeof(SerializableGameObjectRef))]
    public class SerializableGameObjectRefEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            
            EditorGUI.BeginProperty(position, label, property);
            SerializableGameObjectRef src = (SerializableGameObjectRef)property.boxedValue;
            SerializedProperty objIdProp = property.FindPropertyRelative("_objectId");
            EditorGUI.BeginChangeCheck();
            GameObject gameObject = src.Get();

            Object obj = EditorGUI.ObjectField(position, label, gameObject, typeof(GameObject), true);
            
            if (EditorGUI.EndChangeCheck())
            {
                objIdProp.stringValue = obj switch
                {
                    GameObject go => ReferenceManager.GetGuidOf(go),
                    null => ReferenceManager.NoneReference,
                    _ => objIdProp.stringValue
                };

                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
        }
    }
}