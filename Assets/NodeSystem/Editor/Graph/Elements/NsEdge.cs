using System.Collections.Generic;
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
            capabilities &= ~Capabilities.Selectable; // ClickSelector gets unregistered here

            RegisterManipulatorsBeforeDefault(); // Register Manipulators before the default ClickSelector

            capabilities |= Capabilities.Selectable; // and registered back here
        }

        protected virtual List<IManipulator> GetManipulatorsBeforeDefault()
        {
            return new List<IManipulator> { new NsClickSelector() };
        }

        private void RegisterManipulatorsBeforeDefault()
        {
            foreach (IManipulator manipulator in GetManipulatorsBeforeDefault()) this.AddManipulator(manipulator);
        }
    }
}