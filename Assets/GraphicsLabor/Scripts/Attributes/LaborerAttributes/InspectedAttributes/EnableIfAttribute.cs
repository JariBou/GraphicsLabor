using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class EnableIfAttribute : EnableIfAttributeBase
    {
        public EnableIfAttribute(string condition)
            : base(condition)
        {
            Inverted = false;
        }

        public EnableIfAttribute(ConditionOperator conditionOperator, params string[] conditions)
            : base(conditionOperator, conditions)
        {
            Inverted = false;
        }
        
        // override of ShowIfAttributeBase to allow for just passing conditions and defaulting to And operator
        public EnableIfAttribute(params string[] conditions) 
            : base(ConditionOperator.And, conditions)
        {
            Inverted = false;
        }

        public EnableIfAttribute(string enumName, object enumValue)
            : base(enumName, enumValue as Enum)
        {
            Inverted = false;
        }
    }
}