using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowPropertyAttribute : InspectedAttribute
    {
        public bool Enabled { get; private set; }
        public ShowPropertyAttribute(bool enabled = false)
        {
            Enabled = enabled;
        }
    }
}