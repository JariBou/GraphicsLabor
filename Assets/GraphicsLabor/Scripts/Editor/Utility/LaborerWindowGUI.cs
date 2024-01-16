using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Core.Utility;
using JetBrains.Annotations;
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

                bool visible = PropertyUtility.IsVisible(serializedProperty);
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
        
        #endregion
        
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

        private static bool DrawProperty(Rect rect, PropertyInfo property, Object target, [CanBeNull] ref Dictionary<string, List<PropertyInfo>> tabbedProperties, bool checkForTab = false)
        {
            if (property == null) return false;
            // if (!PropertyUtility.IsVisible(property)) return false;
            
            bool isVisible = PropertyUtility.IsVisible(property, new SerializedObject(target));
            
            if (!isVisible) return false;
            
            if (checkForTab && tabbedProperties != null && PropertyUtility.GetAttribute<TabPropertyAttribute>(property) is { } tabPropertyAttribute)
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
            
            bool isEnabled = PropertyUtility.IsEnabled(property, new SerializedObject(target));

            if (property.CanWrite)
            {
                if (DrawWritableField(rect, target, property, isEnabled)) return true;
                
                string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
                return false;
            } 
            
            if (!DrawNonWritableField(rect, target, property, isEnabled))
            {
                string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            } 
            
            return true;
        }

        private static void DrawProperty(Rect rect, PropertyInfo property, Object target)
        {
            Dictionary<string,List<PropertyInfo>> nVar = null;
            DrawProperty(rect, property, target, ref nVar);
        }


        private static float GetPropertyHeight(SerializedProperty property)
        {
            return EditorGUI.GetPropertyHeight(property, includeChildren: true);
        }

        private static float GetPropertiesHeight(IEnumerable<SerializedProperty> properties, float spacing = 0f)
        {
            return spacing + properties.Sum(property => GetPropertyHeight(property) + spacing);
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
        
        public static bool DrawWritableField(Rect position, Object targetObject, PropertyInfo property, bool enabled = false)
        {
            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                string label = ObjectNames.NicifyVariableName(property.Name);
                bool isDrawn = true;
                object value = property.GetValue(targetObject, null);
                
                if (value == null)
                {
                    string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    return false;
                }
                
                Type valueType = value.GetType();

                if (valueType == typeof(bool))
                {
                    property.SetValue(targetObject,EditorGUI.Toggle(position, label, (bool)value));
                }
                else if (valueType == typeof(short))
                {
                    property.SetValue(targetObject,EditorGUI.IntField(position, label, (short)value));
                }
                else if (valueType == typeof(ushort))
                {
                    property.SetValue(targetObject,EditorGUI.IntField(position, label, (ushort)value));
                }
                else if (valueType == typeof(int))
                {
                    property.SetValue(targetObject,EditorGUI.IntField(position, label, (int)value));
                }
                else if (valueType == typeof(uint))
                {
                    property.SetValue(targetObject,EditorGUI.LongField(position, label, (uint)value));
                }
                else if (valueType == typeof(long))
                {
                    property.SetValue(targetObject,EditorGUI.LongField(position, label, (long)value));
                }
                else if (valueType == typeof(ulong))
                {
                    property.SetValue(targetObject,EditorGUI.TextField(position, label, ((ulong)value).ToString()));
                }
                else if (valueType == typeof(float))
                {
                    property.SetValue(targetObject,EditorGUI.FloatField(position, label, (float)value));
                }
                else if (valueType == typeof(double))
                {
                    property.SetValue(targetObject,EditorGUI.DoubleField(position, label, (double)value));
                }
                else if (valueType == typeof(string))
                {
                    property.SetValue(targetObject,EditorGUI.TextField(position, label, (string)value));
                }
                else if (valueType == typeof(Vector2))
                {
                    property.SetValue(targetObject,EditorGUI.Vector2Field(position, label, (Vector2)value));
                }
                else if (valueType == typeof(Vector3))
                {
                    property.SetValue(targetObject,EditorGUI.Vector3Field(position, label, (Vector3)value));
                }
                else if (valueType == typeof(Vector4))
                {
                    property.SetValue(targetObject,EditorGUI.Vector4Field(position, label, (Vector4)value));
                }
                else if (valueType == typeof(Vector2Int))
                {
                    property.SetValue(targetObject,EditorGUI.Vector2IntField(position, label, (Vector2Int)value));
                }
                else if (valueType == typeof(Vector3Int))
                {
                    property.SetValue(targetObject,EditorGUI.Vector3IntField(position, label, (Vector3Int)value));
                }
                else if (valueType == typeof(Color))
                {
                    property.SetValue(targetObject,EditorGUI.ColorField(position, label, (Color)value));
                }
                else if (valueType == typeof(Bounds))
                {
                    property.SetValue(targetObject,EditorGUI.BoundsField(position, label, (Bounds)value));
                }
                else if (valueType == typeof(Rect))
                {
                    property.SetValue(targetObject,EditorGUI.RectField(position, label, (Rect)value));
                }
                else if (valueType == typeof(RectInt))
                {
                    property.SetValue(targetObject,EditorGUI.RectIntField(position, label, (RectInt)value));
                }
                else if (typeof(Object).IsAssignableFrom(valueType))
                {
                    property.SetValue(targetObject,EditorGUI.ObjectField(position, label, (Object)value, valueType, true));
                }
                else if (valueType.BaseType == typeof(Enum))
                {
                    property.SetValue(targetObject,EditorGUI.EnumPopup(position, label, (Enum)value));
                }
                else if (valueType.BaseType == typeof(TypeInfo))
                {
                    property.SetValue(targetObject,EditorGUI.TextField(position, label, value.ToString()));
                }
                else
                {
                    isDrawn = false;
                }

                return isDrawn;
            }
        }
        public static bool DrawNonWritableField(Rect position, Object targetObject, PropertyInfo property, bool enabled = false)
        {
            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                bool isDrawn = true;
                string label = ObjectNames.NicifyVariableName(property.Name);
                object value = property.GetValue(targetObject, null);

                if (value == null)
                {
                    string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    return false;
                }
                
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