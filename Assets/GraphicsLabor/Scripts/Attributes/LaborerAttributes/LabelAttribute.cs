using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    public class LabelAttribute: Attribute, ILaborerAttribute
    {
        public readonly string _label;

        public LabelAttribute(string label)
        {
            _label = label;
        }
    }
}