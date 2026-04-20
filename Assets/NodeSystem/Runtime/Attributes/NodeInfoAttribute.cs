using System;
using NodeSystem.Runtime.Core.PortConfigEnums;

namespace NodeSystem.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeInfoAttribute : Attribute
    {
        public string Title { get; }
        public string MenuItem { get; }
        public FlowDirection NodeFlowDirection { get; }
        public bool HasFlowInput => (NodeFlowDirection & FlowDirection.Input) > 0 && !IsPure;
        public bool HasFlowOutput => (NodeFlowDirection & FlowDirection.Output) > 0 && !IsPure;
        public bool IsPure { get; }
        public int OutputPortCount { get; }
        
        
        public NodeInfoAttribute(string nodeTitle, string menuItem = "",
            FlowDirection flowDirection = FlowDirection.Both, bool isPure = false, int outputPortCount = 1)
        {
            Title = nodeTitle;
            MenuItem = menuItem;
            NodeFlowDirection = flowDirection;
            IsPure = isPure;
            OutputPortCount = outputPortCount;
        }
    }
}