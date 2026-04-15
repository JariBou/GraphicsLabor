using JetBrains.Annotations;
using NodeSystem.Editor.Nodes;
using NodeSystem.Editor.Utils;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.NodesLibrary.Logic;
using UnityEditor.Experimental.GraphView;

namespace NodeSystem.Editor.Editors.NodeEditors
{
    [CustomNodeEditor(typeof(BranchNode)), UsedImplicitly]
    public sealed class BranchNodeEditor : NodeEditorBase
    {
        public override bool AddOutputPorts(NodeSystemEditorNode editorNode)
        {
            Port outputPortTrue = editorNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            outputPortTrue.portName = "True";
            outputPortTrue.tooltip = "The flow output";
            outputPortTrue.portColor = NodeSystemEditorConsts.PortColor_Out;
            editorNode.RegisterPort(outputPortTrue, PropContainerLocation.OutputContainer);
            
            Port outputPortFalse = editorNode.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            outputPortFalse.portName = "False";
            outputPortFalse.tooltip = "The flow output";
            outputPortFalse.portColor = NodeSystemEditorConsts.PortColor_Out;
            editorNode.RegisterPort(outputPortFalse, PropContainerLocation.OutputContainer);
            return true;
        }
    }
}