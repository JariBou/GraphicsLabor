using System;
using NodeSystem.Runtime.References;
using UnityEngine;

namespace NodeSystem.Runtime.Executioners
{
    public class NodeSystemExecutioner : MonoBehaviour
    {
        [SerializeField] private NodeSystemAsset m_graphAsset;

        private NodeSystemAsset graphInstance;

        private string m_currentExecNodeId;

        private async Awaitable Start()
        {
            await StartAsset();
            //ExecuteAsset(graphInstance);
        }

        public async Awaitable StartAsset()
        {
            graphInstance = NodeSystemBank.GetGraphInstance(m_graphAsset);
            await ExecuteAsset(graphInstance);
        }

        private async Awaitable ExecuteAsset(NodeSystemAsset instance)
        {
            NodeSystemNode startNode = instance.GetStartNode();
            m_currentExecNodeId = startNode.id;

            // ProcessAndMoveToNextNode(startNode);
            await TickProcess();
        }

        private void ProcessNode(NodeSystemNode startNode)
        {
        }

        public NodeSystemNode GetCurrentNode()
        {
            return graphInstance == null ? null : graphInstance.GetNode(m_currentExecNodeId);
        }

        public async Awaitable TickProcess()
        {
            ProcessInfo processInfo = await GetCurrentNode().OnProcess(new ExecContext(graphInstance));

            switch (processInfo.FlowType)
            {
                case ProcessInfo.ExecutionFlowType.ExecuteNext:
                    m_currentExecNodeId = processInfo.NextNodeId;
                    _ = TickProcess(); // TODO: discard? await? idk
                    break;
                case ProcessInfo.ExecutionFlowType.Wait:
                    m_currentExecNodeId = processInfo.NextNodeId;
                    break;
                case ProcessInfo.ExecutionFlowType.EndExecution:
                    Debug.Log("Stopped execution at " + m_currentExecNodeId);
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // private void ProcessAndMoveToNextNode(NodeSystemNode startNode)
        // {
        //     ProcessInfo nextNodeId = startNode.OnProcess(new ExecInfo(graphInstance, this));
        //
        //     if (nextNodeId.FlowType is not ProcessInfo.ExecutionFlowType.Wait and not ProcessInfo.ExecutionFlowType.EndExecution)
        //     {
        //         NodeSystemNode nexNode = graphInstance.GetNode(nextNodeId.NextNodeId);
        //         ProcessAndMoveToNextNode(nexNode);
        //     }
        // }

        public void ModifyExposedVariable(string propertyName, string newValue)
        {
            graphInstance.ModifyExposedVariable(propertyName, newValue);
        }
    }
}