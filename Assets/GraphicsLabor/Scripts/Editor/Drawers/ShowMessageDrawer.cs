using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Editor.CustomInspector;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(ShowMessage))]
    public class ShowMessageDrawer : DecoratorDrawer {
    
        public override float GetHeight()
        {
            return GetHelpBoxHeight();
        }
        
        public override void OnGUI(Rect rect)
        {
            if (attribute is not ShowMessage showMessageAttribute)
            {
                return;
            }

            float indent = LaborerEditorGUI.GetIndentLength(rect);
            Rect infoBoxRect = new Rect(
                rect.x + indent,
                rect.y,
                rect.width - indent,
                GetHelpBoxHeight());
            
            EditorGUI.HelpBox(infoBoxRect  , showMessageAttribute.message, showMessageAttribute.messageType);
        }
        
        private float GetHelpBoxHeight()
        {
            ShowMessage showMessageAttribute = (ShowMessage)attribute;
            float minHeight = EditorGUIUtility.singleLineHeight * 2.0f;
            float desiredHeight = GUI.skin.box.CalcHeight(new GUIContent(showMessageAttribute.message), EditorGUIUtility.currentViewWidth);
            float height = Mathf.Max(minHeight, desiredHeight);

            return height;
        }
    }
}
