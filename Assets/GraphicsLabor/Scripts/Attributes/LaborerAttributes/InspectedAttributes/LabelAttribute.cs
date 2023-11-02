namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    public abstract class LabelAttribute: InspectedAttribute
    {
        public readonly string _label;

        protected LabelAttribute(string label)
        {
            _label = label;
        }
    }
}