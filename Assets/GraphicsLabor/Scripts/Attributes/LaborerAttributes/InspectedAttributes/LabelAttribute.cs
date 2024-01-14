namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    public class LabelAttribute: InspectedAttribute
    {
        public readonly string _label;

        public LabelAttribute(string label)
        {
            _label = label;
        }
    }
}