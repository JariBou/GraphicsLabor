using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideIfAttribute : ShowIfAttributeBase
    {
        public HideIfAttribute(string condition)
            : base(condition)
        {
            Inverted = true; // This means that we are hiding instead of showing
        }

        public HideIfAttribute(ConditionOperator conditionOperator, params string[] conditions)
            : base(conditionOperator, conditions)
        {
            Inverted = true;
        }
        
        // override of ShowIfAttributeBase to allow for just passing conditions and defaulting to And operator
        public HideIfAttribute(params string[] conditions) 
            : base(ConditionOperator.And, conditions)
        {
            Inverted = true;
        }

        public HideIfAttribute(string enumName, object enumValue)
            : base(enumName, enumValue as Enum)
        {
            Inverted = true;
        }
    }
}