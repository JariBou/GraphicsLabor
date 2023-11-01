using System;
using System.Collections.Generic;
using System.Linq;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Windows.Utility
{
    public static class LaborerWindowGUI
    {
        
        #region Old

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
                            
                            if (childProperty.name.Equals("m_Script", StringComparison.Ordinal)) continue;

                            bool visible = PropertyUtility.IsVisible(childProperty);
                            if (!visible) continue;

                            LaborerEditorGUI.LayoutPropertyField(childProperty);

                        } while (iterator.NextVisible(false));
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
        
        public static void DrawEditableScriptableObject(ScriptableObject scriptableObject, ref string selectedTab)
        {
            if (!scriptableObject) return;

            Dictionary<string, List<SerializedProperty>> tabbedProperties = new Dictionary<string, List<SerializedProperty>>();

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
                            
                            if (childProperty.name.Equals("m_Script", StringComparison.Ordinal)) continue;
                            
                            DrawField(childProperty, ref tabbedProperties, true); 

                            // LaborerEditorGUI.LayoutField(childProperty);

                        } while (iterator.NextVisible(false));
                    }
                }
                
                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();
                foreach (KeyValuePair<string,List<SerializedProperty>> keyValuePair in tabbedProperties)
                {
                    if (GUILayout.Button(keyValuePair.Key, EditorStyles.toolbarButton))
                    {
                        selectedTab = keyValuePair.Key == selectedTab ? "" : keyValuePair.Key;
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                if (tabbedProperties.TryGetValue(selectedTab, out var tabbedProperty))
                {
                    EditorGUILayout.BeginVertical("box");
                    foreach (SerializedProperty property in tabbedProperty)
                    {
                        LaborerWindowGUI.DrawField(property);
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
                
                
                serializedObject.ApplyModifiedProperties();
            }
        }

        #endregion
        
        public static void DrawField(SerializedProperty serializedProperty, ref Dictionary<string, List<SerializedProperty>> tabbedProperties, bool checkForTab = false)
        {
            if (serializedProperty == null) return;
            if (!PropertyUtility.IsVisible(serializedProperty)) return;
            if (serializedProperty.name.Equals("m_Script", System.StringComparison.Ordinal)) return;
            
            if (checkForTab && PropertyUtility.GetAttribute<TabPropertyAttribute>(serializedProperty) is { } tabPropertyAttribute)
            {
                foreach (string tabName in tabPropertyAttribute._tabNames)
                {
                    if (tabbedProperties.ContainsKey(tabName))
                    {
                        tabbedProperties[tabName].Add(serializedProperty);
                    }
                    else
                    {
                        tabbedProperties.Add(tabName, new List<SerializedProperty> {serializedProperty});
                    }
                }
                
            }
            else
            {
                DrawField(serializedProperty);
            }
            
        }
        public static void DrawField(SerializedProperty serializedProperty)
        {
            if (serializedProperty == null) return;
            if (!PropertyUtility.IsVisible(serializedProperty)) return;

            LaborerEditorGUI.LayoutPropertyField(serializedProperty);
        }
        
        public static bool DrawProperty(Rect rect, SerializedProperty serializedProperty, ref Dictionary<string, List<SerializedProperty>> tabbedProperties, bool checkForTab = false)
        {
            if (serializedProperty == null) return false;
            if (!PropertyUtility.IsVisible(serializedProperty)) return false;
            if (serializedProperty.name.Equals("m_Script", System.StringComparison.Ordinal)) return false;
            
            if (checkForTab && PropertyUtility.GetAttribute<TabPropertyAttribute>(serializedProperty) is { } tabPropertyAttribute)
            {
                foreach (string tabName in tabPropertyAttribute._tabNames)
                {
                    if (tabbedProperties.ContainsKey(tabName))
                    {
                        tabbedProperties[tabName].Add(serializedProperty);
                    }
                    else
                    {
                        tabbedProperties.Add(tabName, new List<SerializedProperty> {serializedProperty});
                    }
                }

                return false;
            }

            DrawProperty(rect, serializedProperty);
            return true;

        }

        private static void DrawProperty(Rect rect,SerializedProperty serializedProperty)
        {
            if (serializedProperty == null) return;
            if (!PropertyUtility.IsVisible(serializedProperty)) return;

            LaborerEditorGUI.PropertyField(rect, serializedProperty, true);
        }
        
        
        public static float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        public static float GetPropertiesHeight(IEnumerable<SerializedProperty> properties)
        {
            return properties.Sum(GetPropertyHeight);
        }
        
    }
}