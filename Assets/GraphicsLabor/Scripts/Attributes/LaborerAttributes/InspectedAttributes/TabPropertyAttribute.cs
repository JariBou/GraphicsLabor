using System;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TabPropertyAttribute : DrawerAttribute
    {
        // TODO: adapt to regularInspector maybe?
        public readonly string[] _tabNames;

        public TabPropertyAttribute(string tabName)
        {
            _tabNames = new[] { tabName };
        }
        
        public TabPropertyAttribute(params string[] tabNames)
        {
            _tabNames = tabNames;
        }
    }
    
}