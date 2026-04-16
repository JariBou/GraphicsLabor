using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils.RefSystem;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

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
            // EditorGUI.LabelField(position, objIdProp.stringValue);
            EditorGUI.BeginChangeCheck();
            GameObject gameObject = src.Get();

            GUIContent labelContent = new()
            {
                tooltip = objIdProp.stringValue,
                text = label.text,
            };

            Object obj = EditorGUI.ObjectField(position, labelContent, gameObject, typeof(GameObject), true);
            
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

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty objectIdProp = property.FindPropertyRelative("_objectId");
            
            GameObject ownerGo = objectIdProp.stringValue == ReferenceManager.NoneReference
                ? null
                : ReferenceManager.GetGameObject<GameObject>(objectIdProp.stringValue);

            ObjectField objectField = new()
            {
                objectType = typeof(GameObject),
                value = ownerGo,
                focusable = true,
                name = property.displayName,
                
                tooltip = property.tooltip,
                label = property.displayName,
                style =
                {
                    flexGrow = 1,
                }
            };

            objectField.RegisterValueChangedCallback(evt =>
            {
                property.serializedObject.Update();
                objectIdProp.serializedObject.Update();
                
                Object obj = evt.newValue;
                switch (obj)
                {
                    case GameObject go:
                        string stringValue = ReferenceManager.GetGuidOf(go.gameObject);
                        objectIdProp.stringValue = stringValue;
                        break;
                    case null:
                        objectIdProp.stringValue = ReferenceManager.NoneReference;
                        break;
                    default:
                        objectIdProp.stringValue = objectIdProp.stringValue;
                        break;
                }

                objectIdProp.serializedObject.ApplyModifiedProperties();
                property.serializedObject.ApplyModifiedProperties();
            });
            
            return objectField;
        }
    }
}