using System;
using GraphicsLabor.Scripts.Attributes.Utility;
using UnityEngine;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HorizontalSeparatorAttribute : PropertyAttribute, ILaborerAttribute
    {
        private const float DefaultHeight = 2.0f;
        private const LaborColor DefaultColor = LaborColor.Gray;

        public float height { get; private set; }
        public LaborColor color { get; private set; }

        public HorizontalSeparatorAttribute(float height = DefaultHeight, LaborColor color = DefaultColor)
        {
            this.height = height;
            this.color = color;
        }
    }
}