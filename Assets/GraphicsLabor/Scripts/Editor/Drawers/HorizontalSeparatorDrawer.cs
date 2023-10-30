using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Attributes.Utility;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(HorizontalSeparatorAttribute))]
    public class HorizontalSeparatorDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            HorizontalSeparatorAttribute lineAttr = (HorizontalSeparatorAttribute)attribute;
            return EditorGUIUtility.singleLineHeight + lineAttr.height;
        }

        public override void OnGUI(Rect position)
        {
            Rect rect = EditorGUI.IndentedRect(position);
            rect.y += EditorGUIUtility.singleLineHeight / 3.0f;
            HorizontalSeparatorAttribute lineAttr = (HorizontalSeparatorAttribute)attribute;
            LaborerEditorGUI.HorizontalLine(rect, lineAttr.height, lineAttr.color.GetColor());
        }
    }
}