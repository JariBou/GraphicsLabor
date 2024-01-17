using GraphicsLabor.Scripts.Editor.Utility;
using GraphicsLabor.Scripts.Editor.Utility.GUI;
using GraphicsLabor.Scripts.Editor.Utility.Reflection;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers
{
    public abstract class PropertyDrawerBase : PropertyDrawer
    {
        // TODO: Resizeable text area
        public sealed override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            bool visible = PropertyUtility.IsVisible(property);
            if (!visible) return;
            
            bool enabled = PropertyUtility.IsEnabled(property);

            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                OnSelfGUI(rect, property, PropertyUtility.GetLabel(property));
            }
        }

        protected abstract void OnSelfGUI(Rect rect, SerializedProperty property, GUIContent label);

        public sealed override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool visible = PropertyUtility.IsVisible(property);
            if (!visible) return 0.0f;

            return GetSelfPropertyHeight(property, label);
        }

        protected virtual float GetSelfPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        protected float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        protected virtual float GetHelpBoxHeight()
        {
            return LaborerGUIUtility.SingleLineHeight * 2.0f;
        }

        protected void DrawDefaultPropertyAndHelpBox(Rect rect, SerializedProperty property, string message, MessageType messageType)
        {
            float indentLength = LaborerEditorGUI.GetIndentLength(rect);
            Rect helpBoxRect = new(
                rect.x + indentLength,
                rect.y,
                rect.width - indentLength,
                GetHelpBoxHeight());

            LaborerEditorGUI.HelpBox(helpBoxRect, message, MessageType.Warning, context: property.serializedObject.targetObject);

            Rect propertyRect = new(
                rect.x,
                rect.y + GetHelpBoxHeight(),
                rect.width,
                GetPropertyHeight(property));

            EditorGUI.PropertyField(propertyRect, property, true);
        }
    }
}