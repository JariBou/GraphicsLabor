using System;
using System.Collections.Generic;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TabPropertyAttribute : DrawerAttribute
    {
        // TODO: adapt to regularInspector maybe?
        public readonly string[] TabNames;
        
        public TabPropertyAttribute(string tabName, params string[] tabNames)
        {
            List<string> tempList = new List<string> { tabName };
            tempList.AddRange(tabNames);
            TabNames = tempList.ToArray();
        }
    }
    
}