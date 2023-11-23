using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Core.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class LaborerWindowGUI
    {
        #region ScriptableObject Drawer

        public static float DrawScriptableObjectNormalSerializedFields(Rect startRect, float yOffset, SerializedObject serializedObject, ref Dictionary<string, List<SerializedProperty>> tabbedSerializedProperties)
        {
            float localOffset = 0;
            using SerializedProperty iterator = serializedObject.GetIterator();
            if (!iterator.NextVisible(true)) return localOffset;
            
            do
            {
                SerializedProperty serializedProperty = serializedObject.FindProperty(iterator.name);
                            
                if (serializedProperty.name.Equals("m_Script", StringComparison.Ordinal)) continue;

                bool visible = PropertyUtility.IsVisible(serializedProperty) && PropertyUtility.IsEnabled(serializedProperty);
                if (!visible) continue;
                            
                float childHeight = GetPropertyHeight(serializedProperty);
                Rect childRect = new()
                {
                    x = startRect.x,
                    y = startRect.y + yOffset + localOffset,
                    width = startRect.width,
                    height = childHeight
                };
                if (DrawSerializedProperty(childRect, serializedProperty, ref tabbedSerializedProperties, true))
                {
                    localOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }

                //LaborerEditorGUI.PropertyField(childRect, childProperty, true);

            } while (iterator.NextVisible(false));
            return localOffset;
        }

        public static float DrawScriptableObjectNormalProperties(Rect startRect, float yOffset, SerializedObject serializedObject, ref Dictionary<string, List<PropertyInfo>> tabbedProperties)
        {
            float localOffset = 0;
            IEnumerable<PropertyInfo> properties = ReflectionUtility.GetAllProperties(serializedObject.targetObject, p => p.GetCustomAttributes(typeof(ShowPropertyAttribute), true).Length > 0);

            IEnumerable<PropertyInfo> propertyInfos = properties as PropertyInfo[] ?? properties.ToArray();
            if (!propertyInfos.Any())
            {
                Debug.Log("No properties");
                return localOffset;
            }

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Rect newRect = new()
                {
                    x = startRect.x,
                    y = startRect.y + yOffset + localOffset,
                    width = startRect.width,
                    height = LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing
                };
                if (DrawProperty(newRect, propertyInfo, serializedObject.targetObject, ref tabbedProperties, true))
                {
                    localOffset += LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }
            }
            return localOffset;
        }
        
        public static float DrawScriptableObjectTabbedSerializedFields(Rect startRect, float yOffset, List<SerializedProperty> properties)
        {
            float localOffset = 0;
            
            using (new EditorGUI.IndentLevelScope())
            {
                Rect bgRect = new()
                {
                    x = 0.0f,
                    y = startRect.y + yOffset + localOffset - LaborerGUIUtility.PropertyHeightSpacing,
                    width = startRect.width * 2.0f,
                    height = GetPropertiesHeight(properties, LaborerGUIUtility.PropertyHeightSpacing)
                };
                
                GUI.Box(bgRect, GUIContent.none);

                foreach (SerializedProperty property in properties)
                {
                    float childHeight = GetPropertyHeight(property);
                    Rect childRect = new()
                    {
                        x = startRect.x,
                        y = startRect.y + yOffset + localOffset,
                        width = startRect.width,
                        height = childHeight
                    };
                    PropertyField(childRect, property, true);
                    
                    localOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }
            }
            
            return localOffset + LaborerGUIUtility.PropertyHeightSpacing;
        }

        
        public static float DrawScriptableObjectTabbedProperties(Rect startRect, float yOffset, SerializedObject serializedObject, List<PropertyInfo> properties)
        {
            float localOffset = 0;
            
            using (new EditorGUI.IndentLevelScope())
            {
                Rect bgRect = new()
                {
                    x = 0.0f,
                    y = startRect.y + yOffset + localOffset - LaborerGUIUtility.PropertyHeightSpacing,
                    width = startRect.width * 2.0f,
                    height = (LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing) * properties.Count + LaborerGUIUtility.PropertyHeightSpacing
                };

                GUI.Box(bgRect, GUIContent.none);

                foreach (PropertyInfo property in properties)
                {
                    float childHeight = LaborerGUIUtility.SingleLineHeight;
                    Rect childRect = new()
                    {
                        x = startRect.x,
                        y = startRect.y + yOffset + localOffset,
                        width = startRect.width,
                        height = childHeight
                    };
                    DrawProperty(childRect, property, serializedObject.targetObject);
                    
                    localOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }
            }
            
            return localOffset + LaborerGUIUtility.PropertyHeightSpacing;
        }

        #endregion
        
        private static void DrawField(SerializedProperty serializedProperty, ref Dictionary<string, List<SerializedProperty>> tabbedProperties, bool checkForTab = false)
        {
            if (serializedProperty == null) return;
            if (!PropertyUtility.IsVisible(serializedProperty)) return;
            if (serializedProperty.name.Equals("m_Script", StringComparison.Ordinal)) return;
            
            if (checkForTab && PropertyUtility.GetAttribute<TabPropertyAttribute>(serializedProperty) is { } tabPropertyAttribute)
            {
                foreach (string tabName in tabPropertyAttribute.TabNames)
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

        private static void DrawField(SerializedProperty serializedProperty)
        {
            if (serializedProperty == null) return;
            if (!PropertyUtility.IsVisible(serializedProperty)) return;

            LaborerEditorGUI.LayoutPropertyField(serializedProperty);
        }

        private static bool DrawSerializedProperty(Rect rect, SerializedProperty serializedProperty, ref Dictionary<string, List<SerializedProperty>> tabbedSerializedProperties, bool checkForTab = false)
        {
            if (serializedProperty == null) return false;
            if (!PropertyUtility.IsVisible(serializedProperty)) return false;
            if (serializedProperty.name.Equals("m_Script", StringComparison.Ordinal)) return false;
            
            if (checkForTab && PropertyUtility.GetAttribute<TabPropertyAttribute>(serializedProperty) is { } tabPropertyAttribute)
            {
                foreach (string tabName in tabPropertyAttribute.TabNames)
                {
                    if (tabbedSerializedProperties.ContainsKey(tabName))
                    {
                        tabbedSerializedProperties[tabName].Add(serializedProperty);
                    }
                    else
                    {
                        tabbedSerializedProperties.Add(tabName, new List<SerializedProperty> {serializedProperty});
                    }
                }

                return false;
            }

            PropertyField(rect, serializedProperty, true);
            return true;
        }
        
        public static void PropertyField(Rect rect, SerializedProperty property, bool includeChildren)
        {
            // Check if visible
            if (!PropertyUtility.IsVisible(property)) return;
            
            bool enabled = PropertyUtility.IsEnabled(property);

            GUIContent label = PropertyUtility.GetLabel(property);

            
            if (PropertyUtility.GetAttribute<ExpandableAttribute>(property) != null)
            {
                label = GUIContent.none;
            }

            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                EditorGUI.PropertyField(rect, property, label, includeChildren);
            }
        }

        private static void DrawSerializedProperty(Rect rect,SerializedProperty serializedProperty)
        {
            if (serializedProperty == null) return;
            if (!PropertyUtility.IsVisible(serializedProperty)) return;

            LaborerEditorGUI.PropertyField(rect, serializedProperty, true);
        }

        private static bool DrawProperty(Rect rect, PropertyInfo property, Object target, ref Dictionary<string, List<PropertyInfo>> tabbedProperties, bool checkForTab = false)
        {
            if (property == null) return false;
            // if (!PropertyUtility.IsVisible(property)) return false;
            
            if (checkForTab && PropertyUtility.GetAttribute<TabPropertyAttribute>(property) is { } tabPropertyAttribute)
            {
                foreach (string tabName in tabPropertyAttribute.TabNames)
                {
                    if (tabbedProperties.ContainsKey(tabName))
                    {
                        tabbedProperties[tabName].Add(property);
                    }
                    else
                    {
                        tabbedProperties.Add(tabName, new List<PropertyInfo> {property});
                    }
                }
                return false;
            }
            DrawField(rect, property.GetValue(target, null), property.Name);
            return true;
        }

        private static void DrawProperty(Rect rect, PropertyInfo property, Object target)
        {
            if (property == null) return;
            // if (!PropertyUtility.IsVisible(property)) return false;
            
            DrawField(rect, property.GetValue(target, null), property.Name);
        }


        private static float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        private static float GetPropertiesHeight(IEnumerable<SerializedProperty> properties, float spacing = 0f)
        {
            return spacing + properties.Sum(property => GetPropertyHeight(property) + spacing);
        }
        
        public static void DrawSerializedProperty(Rect position, Object target, PropertyInfo property)
        {
            object value = property.GetValue(target, null);

            if (value == null)
            {
                string warning =
                    $"{ObjectNames.NicifyVariableName(property.Name)} is null. {nameof(ShowPropertyAttribute)} doesn't support reference types with null value";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
            else if (!DrawField(position, value, ObjectNames.NicifyVariableName(property.Name)))
            {
                string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
        }

        private static bool DrawField(Rect position, object value, string label)
        {
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                bool isDrawn = true;
                Type valueType = value.GetType();

                if (valueType == typeof(bool))
                {
                    EditorGUI.Toggle(position, label, (bool)value);
                }
                else if (valueType == typeof(short))
                {
                    EditorGUI.IntField(position, label, (short)value);
                }
                else if (valueType == typeof(ushort))
                {
                    EditorGUI.IntField(position, label, (ushort)value);
                }
                else if (valueType == typeof(int))
                {
                    EditorGUI.IntField(position, label, (int)value);
                }
                else if (valueType == typeof(uint))
                {
                    EditorGUI.LongField(position, label, (uint)value);
                }
                else if (valueType == typeof(long))
                {
                    EditorGUI.LongField(position, label, (long)value);
                }
                else if (valueType == typeof(ulong))
                {
                    EditorGUI.TextField(position, label, ((ulong)value).ToString());
                }
                else if (valueType == typeof(float))
                {
                    EditorGUI.FloatField(position, label, (float)value);
                }
                else if (valueType == typeof(double))
                {
                    EditorGUI.DoubleField(position, label, (double)value);
                }
                else if (valueType == typeof(string))
                {
                    EditorGUI.TextField(position, label, (string)value);
                }
                else if (valueType == typeof(Vector2))
                {
                    EditorGUI.Vector2Field(position, label, (Vector2)value);
                }
                else if (valueType == typeof(Vector3))
                {
                    EditorGUI.Vector3Field(position, label, (Vector3)value);
                }
                else if (valueType == typeof(Vector4))
                {
                    EditorGUI.Vector4Field(position, label, (Vector4)value);
                }
                else if (valueType == typeof(Vector2Int))
                {
                    EditorGUI.Vector2IntField(position, label, (Vector2Int)value);
                }
                else if (valueType == typeof(Vector3Int))
                {
                    EditorGUI.Vector3IntField(position, label, (Vector3Int)value);
                }
                else if (valueType == typeof(Color))
                {
                    EditorGUI.ColorField(position, label, (Color)value);
                }
                else if (valueType == typeof(Bounds))
                {
                    EditorGUI.BoundsField(position, label, (Bounds)value);
                }
                else if (valueType == typeof(Rect))
                {
                    EditorGUI.RectField(position, label, (Rect)value);
                }
                else if (valueType == typeof(RectInt))
                {
                    EditorGUI.RectIntField(position, label, (RectInt)value);
                }
                else if (typeof(Object).IsAssignableFrom(valueType))
                {
                    GLogger.LogWarning(label);
                    EditorGUI.ObjectField(position, label, (Object)value, valueType, true);
                }
                else if (valueType.BaseType == typeof(Enum))
                {
                    EditorGUI.EnumPopup(position, label, (Enum)value);
                }
                else if (valueType.BaseType == typeof(TypeInfo))
                {
                    EditorGUI.TextField(position, label, value.ToString());
                }
                else
                {
                    isDrawn = false;
                }

                return isDrawn;
            }
        }
        
    }
}