using System;

namespace NodeSystem.Runtime.Attributes
{
    
    public class ExposedPropertyAttribute : Attribute
    {
        public bool HasOutPort => PortDirection == PropPortDirection.Output;
        public bool HasInPort => PortDirection == PropPortDirection.Input;
        public Type PortType { get; }
        public PropPortDirection PortDirection { get; }
        public string OverrideDisplayName { get; }
        public PropContainerLocation PreferredLocation { get; }
        public PropPortCapacity PortCapacity { get; }
        public bool HideInputWhenConnected { get; }
        public bool AutoTyping { get; }

        public ExposedPropertyAttribute(
            PropPortDirection portDirection, 
            Type portType = null, 
            string overrideDisplayName = "", 
            PropContainerLocation preferredLocation = PropContainerLocation.ExtensionContainer, 
            PropPortCapacity portCapacity = PropPortCapacity.Single, 
            bool hideInputWhenConnected = true)
        {
            this.PortType = portType ?? typeof(string);
            this.PortDirection = portDirection;
            OverrideDisplayName = overrideDisplayName;
            PreferredLocation = preferredLocation;
            AutoTyping = portType == null;
            PortCapacity = portCapacity;
            HideInputWhenConnected = hideInputWhenConnected;
        }

    }

    public enum PropPortDirection
    {
        None, Input, Output
    }

    public enum PropContainerLocation
    {
        InputContainer, OutputContainer, ExtensionContainer
    }

    // editor only shenanigans
    public enum PropPortCapacity
    {
        Single, Multi
    }

   

}