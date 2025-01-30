using System;

namespace NodeSystem.Runtime.Attributes
{
    public class NodeInfoAttribute : Attribute
    {
        private string m_nodeTitle;
        private string m_menuItem;
        private FlowDirection m_nodeFlowDirection;
        private bool m_isPure;
        private int _outputPortCount;


        public string title => m_nodeTitle;
        public string menuItem => m_menuItem;
        public FlowDirection NodeFlowDirection => m_nodeFlowDirection;
        public bool hasFlowInput => m_nodeFlowDirection is FlowDirection.Input or FlowDirection.Both && !m_isPure;
        public bool hasFlowOutput => m_nodeFlowDirection is FlowDirection.Output or FlowDirection.Both && !m_isPure;

        public bool IsPure => m_isPure;
        public int OutputPortCount => _outputPortCount;

        public NodeInfoAttribute(string nodeTitle, string menuItem = "", FlowDirection flowDirection = FlowDirection.Both, bool isPure = false, int outputPortCount = 1)
        {
            m_nodeTitle = nodeTitle;
            m_menuItem = menuItem;
            m_nodeFlowDirection = flowDirection;
            m_isPure = isPure;
            _outputPortCount = outputPortCount;
        }
        
    }

    public enum FlowDirection
    {
        None, Input, Output, Both
    }
}