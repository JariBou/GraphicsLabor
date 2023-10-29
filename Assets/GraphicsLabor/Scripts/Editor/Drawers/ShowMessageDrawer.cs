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
            ShowMessage info = attribute as ShowMessage;

            float indent = LaborerEditorGUI.GetIndentLength(rect);
            Rect infoBoxRect = new Rect(
                rect.x + indent,
                rect.y,
                rect.width - indent,
                GetHelpBoxHeight());
            
            EditorGUI.HelpBox(infoBoxRect  , info?.Message, info?.MessageType ?? MessageType.None);
        }
        
        private float GetHelpBoxHeight()
        {
            ShowMessage infoMessageAttribute = (ShowMessage)attribute;
            float minHeight = EditorGUIUtility.singleLineHeight * 2.0f;
            float desiredHeight = GUI.skin.box.CalcHeight(new GUIContent(infoMessageAttribute.Message), EditorGUIUtility.currentViewWidth);
            float height = Mathf.Max(minHeight, desiredHeight);

            return height;
        }
    }
}
