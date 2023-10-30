using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    public class LabelAttribute: Attribute, ILaborerAttribute
    {
        public readonly string Label;

        public LabelAttribute(string label)
        {
            Label = label;
        }
    }
}