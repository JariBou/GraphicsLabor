using System;
using System.Collections.Generic;
using NodeSystem.Editor.Graph.Manipulators;
using NodeSystem.Editor.Nodes;
using NodeSystem.Runtime;
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

        public NsEdge(NodeSystemConnection connection, Func<string, NodeSystemEditorNode> nodeGetter) : this()
        {
            input = nodeGetter(connection.inputPort.nodeId).Ports[connection.inputPort.portIndex];
            output = nodeGetter(connection.outputPort.nodeId).Ports[connection.outputPort.portIndex];
        }

        protected virtual List<IManipulator> GetManipulatorsBeforeDefault()
        {
            return new List<IManipulator> { new NsEdgeClickSelector() };
        }

        private void RegisterManipulatorsBeforeDefault()
        {
            foreach (IManipulator manipulator in GetManipulatorsBeforeDefault())
            {
                this.AddManipulator(manipulator);
            }
        }
    }
}