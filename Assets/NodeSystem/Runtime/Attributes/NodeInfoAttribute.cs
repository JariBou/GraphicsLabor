using System;

namespace NodeSystem.Runtime.Attributes
{
    public class NodeInfoAttribute : Attribute
    {
        private readonly FlowDirection _nodeFlowDirection;
        
        public string Title { get; }
        public string MenuItem { get; }
        public FlowDirection NodeFlowDirection => _nodeFlowDirection;
        public bool HasFlowInput => _nodeFlowDirection is FlowDirection.Input or FlowDirection.Both && !IsPure;
        public bool HasFlowOutput => _nodeFlowDirection is FlowDirection.Output or FlowDirection.Both && !IsPure;
        public bool IsPure { get; }
        public int OutputPortCount { get; }

        public NodeInfoAttribute(string nodeTitle, string menuItem = "", FlowDirection flowDirection = FlowDirection.Both, bool isPure = false, int outputPortCount = 1)
        {
            Title = nodeTitle;
            MenuItem = menuItem;
            _nodeFlowDirection = flowDirection;
            IsPure = isPure;
            OutputPortCount = outputPortCount;
        }
        
    }

    public enum FlowDirection
    {
        None, Input, Output, Both
    }
}