using System;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    /// <summary>
    /// Collection of helper methods for editor use only
    /// </summary>
    public static class GEditorHelpers
    {
        /// <summary>
        /// /!\ Breaks when enum values are not linear
        /// Works with DictionaryDrawStyle since it is linear
        /// </summary>
        /// <param name="property"></param>
        /// <param name="enum"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static bool IsEnumEqual<TEnum>(SerializedProperty property, TEnum @enum) where TEnum : struct, Enum
        {
            if (property.enumValueIndex == -1) return false;

            // return (TEnum)property.enumValueIndex == @enum;
            
            return Enum.Parse<TEnum>(property.enumNames[property.enumValueIndex]).Equals(@enum);
        }
    }
}