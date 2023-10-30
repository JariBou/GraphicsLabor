using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    public class HideIfAttribute : ShowIfAttributeBase
    {
        public HideIfAttribute(string condition)
            : base(condition)
        {
            inverted = true; // This means that we are hiding instead of showing
        }

        public HideIfAttribute(ConditionOperator conditionOperator, params string[] conditions)
            : base(conditionOperator, conditions)
        {
            inverted = true;
        }
        
        // override of ShowIfAttributeBase to allow for just passing conditions and defaulting to And operator
        public HideIfAttribute(params string[] conditions) 
            : base(ConditionOperator.And, conditions)
        {
            inverted = true;
        }

        public HideIfAttribute(string enumName, object enumValue)
            : base(enumName, enumValue as Enum)
        {
            inverted = true;
        }
    }
}