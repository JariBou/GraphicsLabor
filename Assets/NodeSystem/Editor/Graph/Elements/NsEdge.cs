using System;
using NodeSystem.Editor.Graph.Manipulators;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph.Elements
{
    public class NsEdge : Edge
    {
        public NsEdge()
        {
            // Workaround to get NsClickSelector to be registered before the default ClickSelector
            this.capabilities &= ~Capabilities.Selectable; // ClickSlector gets unregistered here
            RegisterManipulatorsBeforeDefault(); // Register Manipulators before the default ClickSelector
            this.capabilities |= Capabilities.Selectable; // and registered back here
        }

        public virtual void RegisterManipulatorsBeforeDefault()
        {
            this.AddManipulator(new NsClickSelector()); // Register NsClickSelector            
        }
    }
}