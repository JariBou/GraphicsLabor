// using System;
// using System.Reflection;
// using UnityEditor;
//
// namespace NodeSystem.Runtime.NodesLibrary.Utils
// {
//     public static class SerializedPropertyExtensions
//     {
//         public static T GetAttribute<T>(this SerializedProperty prop, bool inherit = true) where T : Attribute
//         {
//             if (prop == null) return null;
//
//             Type t = prop.serializedObject.targetObject.GetType();
//
//             FieldInfo f = null;
//             PropertyInfo p = null;
//             foreach (string name in prop.propertyPath.Split('.'))
//             {
//                 f = t.GetField(name, (BindingFlags)(-1));
//
//                 if (f == null)
//                 {
//                     p = t.GetProperty(name, (BindingFlags)(-1));
//                     if (p == null) return null;
//                     t = p.PropertyType;
//                 }
//                 else
//                 {
//                     t = f.FieldType;
//                 }
//             }
//
//             T[] attributes;
//
//             if (f != null)
//                 attributes = f.GetCustomAttributes(typeof(T), inherit) as T[];
//             else if (p != null)
//                 attributes = p.GetCustomAttributes(typeof(T), inherit) as T[];
//             else
//                 return null;
//             return attributes is { Length: > 0 } ? attributes[0] : null;
//         }
//     }
// }

