using System;
using NodeSystem.Runtime.Core.PortConfigEnums;

namespace NodeSystem.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EventExposedPropertyAttribute : ExposedPropertyAttribute
    {
        public EventExposedPropertyAttribute(Type portType = null, string overrideDisplayName = "",
            PropContainerLocation preferredLocation = PropContainerLocation.OutputContainer,
            PropPortCapacity portCapacity = PropPortCapacity.Multi, bool disableInputWhenConnected = false) : base(
            PropPortDirection.Output, portType, overrideDisplayName, preferredLocation, portCapacity,
            disableInputWhenConnected, true)
        {
        }
    }
}