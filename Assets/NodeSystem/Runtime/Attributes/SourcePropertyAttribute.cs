using System;

namespace NodeSystem.Runtime.Attributes
{
    public class SourcePropertyAttribute : Attribute
    {
        public Type SourceType { get; }

        public SourcePropertyAttribute(Type sourceType)
        {
            SourceType = sourceType;
        }
    }
}