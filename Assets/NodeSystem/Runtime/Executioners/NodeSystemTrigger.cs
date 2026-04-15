using System;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.Executioners
{
    [AddComponentMenu(NodeSystemConsts.AddComponentMenuCategoryName+"/Executioners/Node System Trigger")]
    public class NodeSystemTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObjectToTrigger;
        [SerializeField] private NodeSystemAsset _graph;
        [SerializeField] private bool _canBeExecutedMultipleTimes;

        private NodeSystemAsset _graphInstance;
        private NodeSystemNode _nodeToPlay;

        private string m_currentExecNodeId;

        private void Start()
        {
            _graphInstance = NodeSystemBank.GetGraphInstance(_graph);
            if (_gameObjectToTrigger == null) _gameObjectToTrigger = gameObject;
            _nodeToPlay = _graphInstance.GetNodeToPlayFromSource(_gameObjectToTrigger);
            if (_nodeToPlay == null) return;
            m_currentExecNodeId = _nodeToPlay.id;
        }

        public async Awaitable Trigger()
        {
            await TickProcess();
            // nodeToPlay.OnProcess(new ExecInfo(graphInstance, this));
        }

        public NodeSystemNode GetCurrentNode()
        {
            return _graphInstance == null ? null : _graphInstance.GetNode(m_currentExecNodeId);
        }

        public async Awaitable TickProcess()
        {
            if (_graphInstance == null || m_currentExecNodeId == "") return;
            ProcessInfo processInfo = await GetCurrentNode().OnProcess(new ExecContext(_graphInstance));
            Debug.Log("Ticking!");
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

                    // Allows for rerunning the script
                    m_currentExecNodeId = _canBeExecutedMultipleTimes ? _nodeToPlay.id : "";
                    ;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}