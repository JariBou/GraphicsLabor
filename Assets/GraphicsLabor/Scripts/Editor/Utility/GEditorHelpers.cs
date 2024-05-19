using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        /// <param name="enumValue"></param>
        /// <typeparam name="TEnum"></typeparam>
        /// <returns></returns>
        public static bool IsEnumEqual<TEnum>(SerializedProperty property, TEnum enumValue) where TEnum : struct, Enum
        {
            if (property.enumValueIndex == -1) return false;

            // return (TEnum)property.enumValueIndex == enumValue;
            return Enum.ToObject(typeof(TEnum), (byte)property.enumValueIndex).Equals(enumValue);
            
            // return Enum.Parse<TEnum>(property.enumNames[property.enumValueIndex]).Equals(enumValue);

            return EnumConverter<TEnum>.Convert(property.enumValueIndex).Equals(enumValue);

        }

        public static int GetEnumIndex<TEnum>(SerializedProperty property, TEnum enumValue)where TEnum : struct, Enum
        {
            if (property.enumNames.Length == 0) throw new ArgumentException($"Property {nameof(property)} is not of enum type");
            List<string> enumNames = new List<string>(property.enumNames);
            return enumNames.FindIndex(e => e == enumValue.ToString()) - 1;
        }
    }
    
    static class EnumConverter<TEnum> where TEnum : struct, IConvertible
    {
        public static readonly Func<long, TEnum> Convert = GenerateConverter();

        static Func<long, TEnum> GenerateConverter()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(long));
            Expression<Func<long, TEnum>> dynamicMethod = Expression.Lambda<Func<long, TEnum>>(
                Expression.Convert(parameter, typeof(TEnum)),
                parameter);
            return dynamicMethod.Compile();
        }
    }
}