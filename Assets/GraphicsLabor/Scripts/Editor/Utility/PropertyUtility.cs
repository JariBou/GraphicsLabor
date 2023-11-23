using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class PropertyUtility
    {
        public static T GetAttribute<T>(SerializedProperty property) where T : class
        {
            T[] attributes = GetAttributes<T>(property);
            return attributes.Length > 0 ? attributes[0] : null;
        }
        
        public static T GetAttribute<T>(PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttributes<T>().FirstOrDefault(data => data.GetType() == typeof(T));
        }

        private static T[] GetAttributes<T>(SerializedProperty property) where T : class
        {
            FieldInfo fieldInfo = ReflectionUtility.GetField(GetTargetObjectWithProperty(property), property.name);
            if (fieldInfo == null)
            {
                return new T[] { };
            }

            return (T[])fieldInfo.GetCustomAttributes(typeof(T), true);
        }
        
        public static bool IsEnabled(SerializedProperty property)
        {
            ReadOnlyAttribute readOnlyAttribute = GetAttribute<ReadOnlyAttribute>(property);
            // TODO: Add EnableIf Attribute
            
            return readOnlyAttribute == null;
        }

        public static bool IsVisible(SerializedProperty property)
        {
            ShowIfAttributeBase showIfAttributeBase = GetAttribute<ShowIfAttributeBase>(property);
            // There is no ShowIfAttributeBase Attribute so it is visible
            if (showIfAttributeBase == null) return true;

            // We get the object where the property is go be able to get fields' values to check for conditions
            object target = GetTargetObjectWithProperty(property);

            if (showIfAttributeBase.EnumValue != null) // First check if it is via enum
            {
                Enum value = GetEnumValue(target, showIfAttributeBase.Conditions[0]);
                if (value != null)
                {
                    bool isRightEnum = showIfAttributeBase.EnumValue.Equals(value);

                    return showIfAttributeBase.Inverted ? !isRightEnum : isRightEnum;
                }
            }
            
            // now we can check for "regular" conditions
            List<bool> conditionValues = GetConditionValues(target, showIfAttributeBase.Conditions);
            if (conditionValues.Count > 0)
            {
                bool isVisible = ParseConditions(conditionValues, showIfAttributeBase.ConditionOperator,
                    showIfAttributeBase.Inverted);
                

                return isVisible;
            }
            
            // If we go here there is a problem
            Debug.Log($"{showIfAttributeBase.GetType().Name} needs valid boolean or enum fields", property.serializedObject.targetObject);
            return false;
        }

        private static bool ParseConditions(IEnumerable<bool> conditionValues, ConditionOperator conditionOperator, bool invert)
        {
            var tempCondition = conditionOperator == ConditionOperator.And ? 
                conditionValues.Aggregate(true, (current, value) => current && value) :  // And Operator
                conditionValues.Aggregate(false, (current, value) => current || value);  // OR Operator

            return invert ? !tempCondition : tempCondition;
        }

        private static Enum GetEnumValue(object target, string enumName)
        {
            FieldInfo enumField = ReflectionUtility.GetField(target, enumName);
            if (enumField != null && enumField.FieldType.IsSubclassOf(typeof(Enum)))
            {
                return (Enum)enumField.GetValue(target);
            }

            PropertyInfo enumProperty = ReflectionUtility.GetProperty(target, enumName);
            if (enumProperty != null && enumProperty.PropertyType.IsSubclassOf(typeof(Enum)))
            {
                return (Enum)enumProperty.GetValue(target);
            }

            MethodInfo enumMethod = ReflectionUtility.GetMethod(target, enumName);
            if (enumMethod != null && enumMethod.ReturnType.IsSubclassOf(typeof(Enum)))
            {
                return (Enum)enumMethod.Invoke(target, null);
            }

            return null;
        }

        private static List<bool> GetConditionValues(object target, IEnumerable<string> conditions)
        {
            List<bool> conditionValues = new List<bool>();
            foreach (var condition in conditions)
            {
                FieldInfo conditionField = ReflectionUtility.GetField(target, condition);
                if (conditionField != null &&
                    conditionField.FieldType == typeof(bool))
                {
                    conditionValues.Add((bool)conditionField.GetValue(target));
                }

                PropertyInfo conditionProperty = ReflectionUtility.GetProperty(target, condition);
                if (conditionProperty != null &&
                    conditionProperty.PropertyType == typeof(bool))
                {
                    conditionValues.Add((bool)conditionProperty.GetValue(target));
                }

                MethodInfo conditionMethod = ReflectionUtility.GetMethod(target, condition);
                if (conditionMethod != null &&
                    conditionMethod.ReturnType == typeof(bool) &&
                    conditionMethod.GetParameters().Length == 0) // Cant take parameters, same problem as Button
                {
                    conditionValues.Add((bool)conditionMethod.Invoke(target, null));
                }
            }

            return conditionValues;
        }
        
        public static Type GetPropertyType(SerializedProperty property)
        {
            object obj = GetTargetObjectOfProperty(property);
            Type objType = obj.GetType();

            return objType;
        }

        private static object GetTargetObjectOfProperty(SerializedProperty property)
        {
            return GetTargetObject(property, 0);
        }

        private static object GetTargetObjectWithProperty(SerializedProperty property)
        {
            return GetTargetObject(property, 1);
        }
 
        /// <summary>
        /// Returns the object situated at depth back in the properties path
        /// </summary>
        /// <param name="property"></param>
        /// <param name="depth">Depth represents how many objects we go back on the Path.
        /// 0 represents the property's object, 1 the parent and so on</param>
        /// <returns></returns>
        private static object GetTargetObject(SerializedProperty property, int depth)
        {
            if (property == null) return null;
            
            string path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            string[] elements = path.Split('.');
            if (depth > elements.Length) return null;

            for (int i = 0; i < elements.Length - depth; i++)
            {
                string element = elements[i];
                if (element.Contains("["))
                {
                    string elementName = element[..element.IndexOf("[", StringComparison.Ordinal)];
                    int index = Convert.ToInt32(element[element.IndexOf("[", StringComparison.Ordinal)..].Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }

            return obj;
        }
        
        public static GUIContent GetLabel(SerializedProperty property)
        {
            LabelAttribute labelAttribute = GetAttribute<LabelAttribute>(property);
            GUIContent label;
            if (labelAttribute != null)
            {
                label = new GUIContent(labelAttribute._label);
                return label;                
            }

            // ExpandableAttribute expandableAttribute = GetAttribute<ExpandableAttribute>(property);
            // if (expandableAttribute != null)
            // {
            //     return GUIContent.none;
            // }
            
            label = new GUIContent(property.displayName);
            return label;
        }

        private static object GetValue(object source, string name)
        {
            if (source == null) return null;

            Type type = source.GetType();

            while (type != null)
            {
                FieldInfo field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (field != null)
                {
                    return field.GetValue(source);
                }

                PropertyInfo property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    return property.GetValue(source, null);
                }

                type = type.BaseType;
            }

            return null;
        }

        private static object GetValue(object source, string name, int index)
        {
            if (GetValue(source, name) is not IEnumerable enumerable) return null;

            IEnumerator enumerator = enumerable.GetEnumerator();
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                {
                    return null;
                }
            }

            return enumerator.Current;
        }
    }
}
