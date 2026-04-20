using System;

namespace NodeSystem.Runtime.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerateEventNodeAttribute : Attribute
    {
        public GenerateEventNodeAttribute(string nodeTitle, string menuItem = "", bool isPure = false)
        {
            NodeTitle = nodeTitle;
            MenuItem = menuItem;
            IsPure = isPure;
        }

        public string NodeTitle { get; }
        public string MenuItem { get; }
        public bool IsPure { get; }
    }
}