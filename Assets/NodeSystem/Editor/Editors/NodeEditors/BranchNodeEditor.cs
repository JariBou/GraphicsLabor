using NodeSystem.Editor.Nodes;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.NodesLibrary;
using NodeSystem.Runtime.NodesLibrary.Logic;
using UnityEditor.Experimental.GraphView;

namespace NodeSystem.Editor.Editors.NodeEditors
{
    [CustomNodeEditor(typeof(BranchNode))]
    public class BranchNodeEditor : NodeEditorBase
    {
        public override bool AddOutputPorts(NodeSystemEditorNode editorNode)
        {
            Port outputPortTrue = editorNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            outputPortTrue.portName = "True";
            outputPortTrue.tooltip = "The flow output";
            editorNode.RegisterPort(outputPortTrue, PropContainerLocation.OutputContainer);
            
            Port outputPortFalse = editorNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            outputPortFalse.portName = "False";
            outputPortFalse.tooltip = "The flow output";
            editorNode.RegisterPort(outputPortFalse, PropContainerLocation.OutputContainer);
            return true;
        }

        public override bool AddInputPorts(NodeSystemEditorNode editorNode)
        {
            return base.AddInputPorts(editorNode);
        }
    }
}