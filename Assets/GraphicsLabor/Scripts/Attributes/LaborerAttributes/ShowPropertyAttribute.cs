using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowPropertyAttribute : Attribute, ILaborerAttribute
    {
        
    }
}