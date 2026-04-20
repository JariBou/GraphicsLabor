using System;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.Executioners
{
    [AddComponentMenu(NodeSystemConsts.AddComponentMenuCategoryName + "/Executioners/Node System Executioner")]
    public class NodeSystemExecutioner : MonoBehaviour
    {
        [FormerlySerializedAs("m_graphAsset"), SerializeField]
        private NodeSystemAsset _graphAsset;

        private string _currentExecNodeId;

        private NodeSystemAsset _graphInstance;

        private async Awaitable Start()
        {
            await StartAsset();
            //ExecuteAsset(graphInstance);
        }

        public async Awaitable StartAsset()
        {
            _graphInstance = NodeSystemBank.GetGraphInstance(_graphAsset);
            await ExecuteAsset(_graphInstance);
        }

        private async Awaitable ExecuteAsset(NodeSystemAsset instance)
        {
            NodeSystemNode startNode = instance.GetStartNode();
            _currentExecNodeId = startNode.ID;

            // ProcessAndMoveToNextNode(startNode);
            await TickProcess();
        }

        public NodeSystemNode GetCurrentNode()
        {
            return _graphInstance == null ? null : _graphInstance.GetNode(_currentExecNodeId);
        }

        public async Awaitable TickProcess()
        {
            ProcessInfo processInfo = await GetCurrentNode().OnProcessAsync(new ExecContext(_graphInstance));

            switch (processInfo.FlowType)
            {
                case ProcessInfo.ExecutionFlowType.ExecuteNext:
                    _currentExecNodeId = processInfo.NextNodeId;
                    _ = TickProcess(); // TODO: discard? await? idk
                    break;
                case ProcessInfo.ExecutionFlowType.Wait:
                    _currentExecNodeId = processInfo.NextNodeId;
                    break;
                case ProcessInfo.ExecutionFlowType.EndExecution:
                    Debug.Log("Stopped execution at " + _currentExecNodeId);
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
            _graphInstance.ModifyExposedVariable(propertyName, newValue);
        }
    }
}