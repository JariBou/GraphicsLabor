using NodeSystem.Runtime.Attributes;

namespace NodeSystem.Runtime.NodesLibrary.Utils
{
    [NodeInfo("TextOutput", "Utils/TextOutput")]
    public class TextOutputNode : NodeSystemNode
    {
        [ExposedProperty(portDirection: PropPortDirection.Output)]
        public string TextOutput;
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            return base.OnProcess(info);
        }
    }
}