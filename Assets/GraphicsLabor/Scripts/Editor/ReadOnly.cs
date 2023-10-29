using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor
{
    /// <summary>
    /// Allows to display an attribute as ReadOnly
    /// </summary>
    public class ReadOnly : PropertyAttribute
    {

        public readonly string _overrideName = "reserved_null-1";
        public ReadOnly()
        {
 
        }
        public ReadOnly(string overrideName)
        {
            _overrideName = overrideName;
        }
    }
    
 
[CustomPropertyDrawer(typeof(ReadOnly))]
public class ReadOnlyDrawer : PropertyDrawer {
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        string possibleName = (attribute as ReadOnly)?._overrideName;
        if (possibleName != "reserved_null-1")
        {
            label.text = possibleName;
        }
        
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