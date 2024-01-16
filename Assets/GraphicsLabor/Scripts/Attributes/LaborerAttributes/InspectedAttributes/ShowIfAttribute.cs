using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : ShowIfAttributeBase
    {
        public ShowIfAttribute(string condition)
            : base(condition)
        {
            Inverted = false;
        }

        public ShowIfAttribute(ConditionOperator conditionOperator, params string[] conditions)
            : base(conditionOperator, conditions)
        {
            Inverted = false;
        }
        
        // override of ShowIfAttributeBase to allow for just passing conditions and defaulting to And operator
        public ShowIfAttribute(params string[] conditions) 
            : base(ConditionOperator.And, conditions)
        {
            Inverted = false;
        }

        public ShowIfAttribute(string enumName, object enumValue)
            : base(enumName, enumValue as Enum)
        {
            Inverted = false;
        }
    }
}