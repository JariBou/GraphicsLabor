using System;
using NodeSystem.Runtime.Core.PortConfigEnums;

namespace NodeSystem.Runtime.Attributes.EditorTarget
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventNodeInfoAttribute : NodeInfoAttribute
    {
        public EventNodeInfoAttribute(string nodeTitle, string menuItem = "", bool isPure = false) : base(nodeTitle,
            menuItem, FlowDirection.Output, isPure)
        {
        }
    }
}