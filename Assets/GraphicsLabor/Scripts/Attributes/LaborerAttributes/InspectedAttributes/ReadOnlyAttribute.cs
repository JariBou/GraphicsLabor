using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    /// <summary>
    /// Allows to display an attribute as ReadOnly
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnlyAttribute : InspectedAttribute
    {
    }
    
 

}