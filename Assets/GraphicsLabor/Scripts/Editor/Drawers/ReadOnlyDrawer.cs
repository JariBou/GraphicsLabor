using System;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ReadOnly))]
    public class ReadOnlyDrawer : PropertyDrawer {
    
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not ReadOnly readOnlyAttribute)
            {
                return;
            }

            if (readOnlyAttribute.OverrideName != null) label.text = readOnlyAttribute.OverrideName;
        
            GUI.enabled = false;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    EditorGUI.LabelField(position, label, new GUIContent(property.boolValue.ToString()));
                    break;
                case SerializedPropertyType.Enum:
                    EditorGUI.LabelField(position, label, new GUIContent(property.enumDisplayNames[property.enumValueIndex]));
                    break;
                case SerializedPropertyType.Float:
                    EditorGUI.FloatField(position, label, property.floatValue);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.LabelField(position, label, new GUIContent(property.intValue.ToString()));
                    break;
                case SerializedPropertyType.Quaternion:
                    EditorGUI.Vector4Field(position, label, Utils.GetQuaternionAsVector4(property.quaternionValue));
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.LabelField(position, label, new GUIContent(property.stringValue));
                    break;
                case SerializedPropertyType.Vector2:
                    EditorGUI.Vector2Field(position, label, property.vector2Value);
                    break;
                case SerializedPropertyType.Vector3:
                    EditorGUI.Vector3Field(position, label, property.vector3Value);
                    break;
                case SerializedPropertyType.Vector4:
                    EditorGUI.Vector4Field(position, label, property.vector4Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"ReadOnly Attribute cannot be used on {property.propertyType.ToString()}");
            }
            GUI.enabled = true;
        }
    }
}
