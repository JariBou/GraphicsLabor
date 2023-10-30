using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    public class ShowIfAttributeBase : Attribute, ILaborerAttribute
    {
        public string[] conditions { get; private set; }
        public ConditionOperator conditionOperator { get; private set; }
        public bool inverted { get; protected set; }
        public Enum enumValue { get; private set; }

        public ShowIfAttributeBase(string condition)
        {
            conditionOperator = ConditionOperator.And;
            conditions = new[] { condition };
        }

        // Allows for showing a field if certains conditions are met (bools are true)
        public ShowIfAttributeBase(ConditionOperator conditionOperator, params string[] conditions) // params string[] equivalent to *args of python but strongly typed
        {
            this.conditionOperator = conditionOperator;
            this.conditions = conditions;
        }

        // Allows for showing a field if an enum field has a certain value
        public ShowIfAttributeBase(string enumName, Enum enumValue)
            : this(enumName) // Like in C++ calls ShowAttributeBase before
        {
            this.enumValue = enumValue ?? throw new ArgumentNullException(nameof(enumValue), "This parameter must be an enum value.");
        }
    }
}