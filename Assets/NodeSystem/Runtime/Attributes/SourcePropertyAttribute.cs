using System;

namespace NodeSystem.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field), Obsolete]
    public sealed class SourcePropertyAttribute : Attribute
    {
        public Type SourceType { get; }

        public SourcePropertyAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }
    }
}