using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SerializedDictionarySettingsAttribute : InspectedAttribute
    {
        public string KeyName { get; private set; }
        public string ValueName { get; private set; }

        public SerializedDictionarySettingsAttribute(string keyName = null, string valueName = null)
        {
            KeyName = keyName;
            ValueName = valueName;
        }
    }
}