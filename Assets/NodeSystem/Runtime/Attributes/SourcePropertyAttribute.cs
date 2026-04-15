using System;

namespace NodeSystem.Runtime.Attributes
{
    public sealed class SourcePropertyAttribute : Attribute
    {
        public SourcePropertyAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }

        public Type SourceType { get; }
    }
}