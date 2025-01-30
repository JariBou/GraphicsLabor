using System;
using NodeSystem.Runtime.References;
using UnityEngine;

namespace NodeSystem.Runtime.Executionners
{
    public class NodeSystemExecutioner : MonoBehaviour, INodeSystemExecutioner
    {
        [SerializeField]
        private NodeSystemAsset m_graphAsset;

        private NodeSystemAsset graphInstance;

        private string m_currentExecNodeId;

        private void Start()
        {
            StartAsset();
            //ExecuteAsset(graphInstance);
        }

        public void StartAsset()
        {
            graphInstance = NodeSystemBank.GetGraphInstance(m_graphAsset);
            ExecuteAsset(graphInstance);
        }
        private void ExecuteAsset(NodeSystemAsset instance)
        {
            NodeSystemNode startNode = instance.GetStartNode();
            m_currentExecNodeId = startNode.id;

            // ProcessAndMoveToNextNode(startNode);
            TickProcess();
        }

        private void ProcessNode(NodeSystemNode startNode)
        {
            
        }

        public NodeSystemNode GetCurrentNode()
        {
            return graphInstance == null ? null : graphInstance.GetNode(m_currentExecNodeId);
        }

        public void TickProcess()
        {
            ProcessInfo processInfo = GetCurrentNode().OnProcess(new ExecInfo(graphInstance, this));

            switch (processInfo.FlowType)
            {
                case ProcessInfo.ExecutionFlowType.ExecuteNext:
                    m_currentExecNodeId = processInfo.NextNodeId;
                    TickProcess();
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

        public MonoBehaviour GetObject()
        {
            return this;
        }

        private void ProcessAndMoveToNextNode(NodeSystemNode startNode)
        {
            ProcessInfo nextNodeId = startNode.OnProcess(new ExecInfo(graphInstance, this));

            if (nextNodeId.FlowType is not ProcessInfo.ExecutionFlowType.Wait and not ProcessInfo.ExecutionFlowType.EndExecution)
            {
                NodeSystemNode nexNode = graphInstance.GetNode(nextNodeId.NextNodeId);
                ProcessAndMoveToNextNode(nexNode);
            }
        }

        public void ModifyExposedVariable(string propertyName, string newValue)
        {
            graphInstance.ModifyExposedVariable(propertyName, newValue);
        }
        
        #if UNITY_EDITOR

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TickProcess();
            }
        }
        
        #endif
    }
}