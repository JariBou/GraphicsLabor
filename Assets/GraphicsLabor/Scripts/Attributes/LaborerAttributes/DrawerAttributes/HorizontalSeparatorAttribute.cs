using System;
using GraphicsLabor.Scripts.Attributes.Utility;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class HorizontalSeparatorAttribute : DrawerAttribute
    {
        private const float DefaultHeight = 2.0f;
        private const LaborColor DefaultColor = LaborColor.Gray;

        public float Height { get; private set; }
        public LaborColor Color { get; private set; }

        public HorizontalSeparatorAttribute(float height = DefaultHeight, LaborColor color = DefaultColor)
        {
            Height = height;
            Color = color;
        }
    }
}