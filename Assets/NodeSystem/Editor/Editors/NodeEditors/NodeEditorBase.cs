using NodeSystem.Editor.Nodes;

namespace NodeSystem.Editor.Editors.NodeEditors
{
    public abstract class NodeEditorBase
    {
        /// <summary>
        /// Output Ports are added first, so index: 0, 1, 2, ....
        /// </summary>
        /// <param name="editorNode"></param>
        /// <returns></returns>
        public virtual bool AddOutputPorts(NodeSystemEditorNode editorNode)
        {
            
            return false;
        }
        
        /// <summary>
        /// Input Ports are added after input ports, so index: last_output_index+1, +2, +3, ...
        /// </summary>
        /// <param name="editorNode"></param>
        /// <returns></returns>
        public virtual bool AddInputPorts(NodeSystemEditorNode editorNode)
        {
            return false;
        }
    }
}