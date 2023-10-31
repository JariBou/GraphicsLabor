using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Windows.Utility
{
    public static class LaborerWindowGUI
    {
        // For use with EditorGuiLayout
        public static void DrawChildProperties(ScriptableObject scriptableObject)
        {
            if (!scriptableObject) return;

            using (new EditorGUI.IndentLevelScope())
            {
                SerializedObject serializedObject = new(scriptableObject);
                serializedObject.Update();

                using (SerializedProperty iterator = serializedObject.GetIterator())
                {
                    if (iterator.NextVisible(true))
                    {
                        do
                        {
                            SerializedProperty childProperty = serializedObject.FindProperty(iterator.name);
                            
                            if (childProperty.name.Equals("m_Script", System.StringComparison.Ordinal)) continue;

                            bool visible = PropertyUtility.IsVisible(childProperty);
                            if (!visible) continue;

                            LaborerEditorGUI.LayoutField(childProperty);

                        } while (iterator.NextVisible(false));
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
        
        // For use with EditorGui
        private static void DrawChildProperties(Rect rect, SerializedProperty property)
        {
            ScriptableObject scriptableObject = property.objectReferenceValue as ScriptableObject;
            if (!scriptableObject) return;

            Rect boxRect = new()
            {
                x = 0.0f,
                y = rect.y + EditorGUIUtility.singleLineHeight,
                width = rect.width * 2.0f,
                height = rect.height - EditorGUIUtility.singleLineHeight
            };

            GUI.Box(boxRect, GUIContent.none);

            using (new EditorGUI.IndentLevelScope())
            {
                SerializedObject serializedObject = new(scriptableObject);
                serializedObject.Update();

                using (var iterator = serializedObject.GetIterator())
                {
                    float yOffset = EditorGUIUtility.singleLineHeight;

                    if (iterator.NextVisible(true))
                    {
                        do
                        {
                            SerializedProperty childProperty = serializedObject.FindProperty(iterator.name);
                            
                            if (childProperty.name.Equals("m_Script", System.StringComparison.Ordinal)) continue;

                            bool visible = PropertyUtility.IsVisible(childProperty);
                            if (!visible) continue;

                            float childHeight = GetPropertyHeight(childProperty);
                            Rect childRect = new()
                            {
                                x = rect.x,
                                y = rect.y + yOffset,
                                width = rect.width,
                                height = childHeight
                            };

                            LaborerEditorGUI.PropertyField(childRect, childProperty, true);

                            yOffset += childHeight;
                        } while (iterator.NextVisible(false));
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }

        private static float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }
    }
}