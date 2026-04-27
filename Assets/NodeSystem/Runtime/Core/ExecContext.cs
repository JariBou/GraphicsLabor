using NodeSystem.Runtime.Utils;

namespace NodeSystem.Runtime.Core
{
    public class ExecContext
    {
        public string ExecId { get; }
        public NodeSystemAsset GraphInstance { get; }

        public ExecContext(NodeSystemAsset graphInstance)
        {
            GraphInstance = graphInstance;
            ExecId = GuidSystem.NewGuid();
        }
    }
}