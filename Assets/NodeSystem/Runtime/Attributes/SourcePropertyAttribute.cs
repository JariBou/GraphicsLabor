using System;

namespace NodeSystem.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field), Obsolete]
    public sealed class SourcePropertyAttribute : Attribute
    {
        public SourcePropertyAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }

        public Type SourceType { get; }
    }
}