using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ShowIfAttribute : ShowIfAttributeBase
    {
        public ShowIfAttribute(string condition)
            : base(condition)
        {
            inverted = false;
        }

        public ShowIfAttribute(ConditionOperator conditionOperator, params string[] conditions)
            : base(conditionOperator, conditions)
        {
            inverted = false;
        }
        
        // override of ShowIfAttributeBase to allow for just passing conditions and defaulting to And operator
        public ShowIfAttribute(params string[] conditions) 
            : base(ConditionOperator.And, conditions)
        {
            inverted = false;
        }

        public ShowIfAttribute(string enumName, object enumValue)
            : base(enumName, enumValue as Enum)
        {
            inverted = false;
        }
    }
}