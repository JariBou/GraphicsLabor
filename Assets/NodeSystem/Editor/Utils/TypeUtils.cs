using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Editor.Utils
{
    public static class TypeUtils
    {
        /// <summary>
        /// Returns the Type of a given SerializedProperty
        /// </summary>
        /// <param name="property">The SerializedProperty</param>
        /// <returns></returns>
        public static Type GetPropertyType(SerializedProperty property)
        {
            object obj = GetTargetObjectOfProperty(property);
            if (obj == null)
            {
                Debug.LogWarning("Problem getting type of property '" + property.name + "'. Returning null.");
                return null;
            }
            Type objType = obj.GetType();

            return objType;
        }

        /// <summary>
        /// Returns the object held by the property
        /// </summary>
        /// <param name="property">The SerializedProperty</param>
        /// <returns></returns>
        private static object GetTargetObjectOfProperty(SerializedProperty property)
        {
            return GetTargetObject(property, 0);
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
        
        /// <summary>
        /// Returns value of Field or Property from object
        /// </summary>
        /// <param name="source">The object holding the Field or Property</param>
        /// <param name="name">The name of the Field or Property</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Returns value of Field or Property from object at index
        /// </summary>
        /// <param name="source">The object holding the Field or Property</param>
        /// <param name="name">The name of the Field or Property</param>
        /// <param name="index">The index to look at</param>
        /// <returns></returns>
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