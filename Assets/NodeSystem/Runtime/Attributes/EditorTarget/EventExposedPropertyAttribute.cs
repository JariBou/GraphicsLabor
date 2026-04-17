using System;

namespace NodeSystem.Runtime.Attributes.EditorTarget
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EventExposedPropertyAttribute : ExposedPropertyAttribute
    {
        public EventExposedPropertyAttribute(Type portType = null, string overrideDisplayName = "",
            PropContainerLocation preferredLocation = PropContainerLocation.OutputContainer,
            PropPortCapacity portCapacity = PropPortCapacity.Single, bool disableInputWhenConnected = false) : base(
            PropPortDirection.Output, portType, overrideDisplayName, preferredLocation, portCapacity,
            disableInputWhenConnected, true)
        {
        }
    }
}