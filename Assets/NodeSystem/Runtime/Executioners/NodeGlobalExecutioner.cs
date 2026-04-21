using System;
using NodeSystem.Runtime.Core;
using UnityEngine;

namespace NodeSystem.Runtime.Executioners
{
    public class NodeGlobalExecutioner
    {
        private static NodeGlobalExecutioner _instance;

        public static NodeGlobalExecutioner Instance
        {
            get
            {
                _instance ??= new NodeGlobalExecutioner();
                return _instance;
            }
        }

        public async Awaitable RunNodeNoAwaitAsync(ExecContext ctx, NodeSystemNode node)
        {
            NodeSystemAsset graphInstance = ctx.GraphInstance;
            ExecContext newExecContext = new(graphInstance);
            ProcessInfo processInfo = await node.OnProcessAsync(newExecContext);
            switch (processInfo.FlowType)
            {
                case ProcessInfo.ExecutionFlowType.ExecuteNext:
                    _ = RunNodeNoAwaitAsync(newExecContext, graphInstance.GetNode(processInfo.NextNodeId));
                    break;
                case ProcessInfo.ExecutionFlowType.Wait:
                    break;
                case ProcessInfo.ExecutionFlowType.EndExecution:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Awaitable RunNodeAwaitAsync(ExecContext ctx, NodeSystemNode node)
        {
            NodeSystemAsset graphInstance = ctx.GraphInstance;
            ExecContext newExecContext = new(graphInstance);

            // Exec first
            ProcessInfo processInfo = await node.OnProcessAsync(newExecContext);
            NodeSystemNode nodeToPlay = graphInstance.GetNode(processInfo.NextNodeId);

            // Exec next if any
            while (nodeToPlay != null)
            {
                processInfo = await nodeToPlay.OnProcessAsync(newExecContext);
                nodeToPlay = graphInstance.GetNode(processInfo.NextNodeId);
            }
        }
    }
}