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

        public async Awaitable RunNode(ExecContext ctx, NodeSystemNode node)
        {
            NodeSystemAsset graphInstance = ctx.GraphInstance;
            ExecContext newExecContext = new(graphInstance);
            ProcessInfo processInfo = await node.OnProcess(newExecContext);
            switch (processInfo.FlowType)
            {
                case ProcessInfo.ExecutionFlowType.ExecuteNext:
                    _ = RunNode(newExecContext, graphInstance.GetNode(processInfo.NextNodeId));
                    break;
                case ProcessInfo.ExecutionFlowType.Wait:
                    break;
                case ProcessInfo.ExecutionFlowType.EndExecution:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}