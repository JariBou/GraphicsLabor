﻿using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    public class ShowIfAttributeBase : Attribute, ILaborerAttribute
    {
        public string[] Conditions { get; private set; }
        public ConditionOperator ConditionOperator { get; private set; }
        public bool Inverted { get; protected set; }
        public Enum EnumValue { get; private set; }

        public ShowIfAttributeBase(string condition)
        {
            ConditionOperator = ConditionOperator.And;
            Conditions = new[] { condition };
        }

        // Allows for showing a field if certains conditions are met (bools are true)
        public ShowIfAttributeBase(ConditionOperator conditionOperator, params string[] conditions) // params string[] equivalent to *args of python but strongly typed
        {
            this.ConditionOperator = conditionOperator;
            this.Conditions = conditions;
        }

        // Allows for showing a field if an enum field has a certain value
        public ShowIfAttributeBase(string enumName, Enum enumValue)
            : this(enumName) // Like in C++ calls ShowAttributeBase before
        {
            this.EnumValue = enumValue ?? throw new ArgumentNullException(nameof(enumValue), "This parameter must be an enum value.");
        }
    }
}