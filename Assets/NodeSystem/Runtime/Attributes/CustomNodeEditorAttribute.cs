using System;

namespace NodeSystem.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomNodeEditorAttribute : Attribute
    {
        public Type TargetType { get; }

        public CustomNodeEditorAttribute(Type targetType)
        {
            TargetType = targetType;
        }
    }
}