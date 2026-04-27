using System;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.Executioners
{
    [AddComponentMenu(NodeSystemConsts.AddComponentMenuCategoryName + "/Executioners/Node System Trigger")]
    public class NodeSystemTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject _gameObjectToTrigger;
        [SerializeField] private NodeSystemAsset _graph;
        [SerializeField] private bool _canBeExecutedMultipleTimes;

        private string _currentExecNodeId;

        private NodeSystemAsset _graphInstance;
        private NodeSystemNode _nodeToPlay;

        private void Start()
        {
            _graphInstance = NodeSystemBank.GetGraphInstance(_graph);
            if (_gameObjectToTrigger == null) _gameObjectToTrigger = gameObject;

            _nodeToPlay = _graphInstance.GetNodeToPlayFromSource(_gameObjectToTrigger);
            if (_nodeToPlay == null) return;

            _currentExecNodeId = _nodeToPlay.ID;
        }

        public async Awaitable Trigger()
        {
            await TickProcess();
            // nodeToPlay.OnProcess(new ExecInfo(graphInstance, this));
        }

        public NodeSystemNode GetCurrentNode()
        {
            return _graphInstance == null ? null : _graphInstance.GetNode(_currentExecNodeId);
        }

        public async Awaitable TickProcess()
        {
            if (_graphInstance == null || _currentExecNodeId == "") return;

            ProcessInfo processInfo = await GetCurrentNode().OnProcessAsync(new ExecContext(_graphInstance));
            Debug.Log("Ticking!");
            switch (processInfo.FlowType)
            {
                case ProcessInfo.ExecutionFlowType.ExecuteNext:
                {
                    _currentExecNodeId = processInfo.NextNodeId;
                    _ = TickProcess(); // TODO: discard? await? idk
                    break;
                }
                case ProcessInfo.ExecutionFlowType.Wait:
                {
                    _currentExecNodeId = processInfo.NextNodeId;
                    break;
                }
                case ProcessInfo.ExecutionFlowType.EndExecution:
                {
                    Debug.Log("Stopped execution at " + _currentExecNodeId);

                    // Allows for rerunning the script
                    _currentExecNodeId = _canBeExecutedMultipleTimes ? _nodeToPlay.ID : "";
                    return;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}