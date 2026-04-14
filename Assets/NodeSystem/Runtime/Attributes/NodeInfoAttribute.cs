using System;

namespace NodeSystem.Runtime.Attributes
{
    public class NodeInfoAttribute : Attribute
    {
        public NodeInfoAttribute(string nodeTitle, string menuItem = "",
            FlowDirection flowDirection = FlowDirection.Both, bool isPure = false, int outputPortCount = 1)
        {
            Title = nodeTitle;
            MenuItem = menuItem;
            NodeFlowDirection = flowDirection;
            IsPure = isPure;
            OutputPortCount = outputPortCount;
        }

        public string Title { get; }
        public string MenuItem { get; }
        public FlowDirection NodeFlowDirection { get; }

        public bool HasFlowInput => NodeFlowDirection is FlowDirection.Input or FlowDirection.Both && !IsPure;
        public bool HasFlowOutput => NodeFlowDirection is FlowDirection.Output or FlowDirection.Both && !IsPure;
        public bool IsPure { get; }
        public int OutputPortCount { get; }
    }

    public enum FlowDirection
    {
        None,
        Input,
        Output,
        Both
    }
}