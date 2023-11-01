using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ShowMessageAttribute))]
    public class ShowMessageDrawer : DecoratorDrawer {
    
        public override float GetHeight() => GetHelpBoxHeight();
        
        public override void OnGUI(Rect rect)
        {
            if (attribute is not ShowMessageAttribute showMessageAttribute) return;

            float indent = LaborerEditorGUI.GetIndentLength(rect);
            Rect infoBoxRect = new(
                rect.x + indent,
                rect.y,
                rect.width - indent,
                GetHelpBoxHeight());
            
            EditorGUI.HelpBox(infoBoxRect  , showMessageAttribute.Message, showMessageAttribute.MessageType);
        }
        
        private float GetHelpBoxHeight()
        {
            ShowMessageAttribute showMessageAttributeAttribute = (ShowMessageAttribute)attribute;
            float minHeight = EditorGUIUtility.singleLineHeight * 2.0f;
            float desiredHeight = GUI.skin.box.CalcHeight(new GUIContent(showMessageAttributeAttribute.Message), EditorGUIUtility.currentViewWidth);
            float height = Mathf.Max(minHeight, desiredHeight);

            return height;
        }
    }
}
