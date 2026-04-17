using NodeSystem.Runtime.Utils;

namespace NodeSystem.Runtime
{
    public class ExecContext
    {
        public ExecContext(NodeSystemAsset graphInstance)
        {
            GraphInstance = graphInstance;
            ExecId = GuidSystem.NewGuid();
        }

        public string ExecId { get; }
        public NodeSystemAsset GraphInstance { get; }
    }
}