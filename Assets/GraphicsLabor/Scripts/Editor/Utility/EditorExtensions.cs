using System;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class SerializedPropertyExtensions
    {

        public static bool IsEnumValue<TEnum>(this SerializedProperty self, TEnum enumValue) where TEnum : struct, Enum
        {
            if (self.enumValueIndex == -1) return false;

            // return (TEnum)self.enumValueIndex == enumValue;
            
            // return Enum.Parse<TEnum>(self.enumNames[self.enumValueIndex]).Equals(enumValue);
            return EnumConverter<TEnum>.Convert(self.enumValueIndex).Equals(enumValue);

        }
        
    }
}