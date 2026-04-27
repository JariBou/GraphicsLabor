using System;

namespace NodeSystem.Runtime.Core
{
    [Serializable]
    public struct ProcessInfo
    {
        public string NextNodeId { get; }
        public string PrevNodeId { get; }
        public ExecutionFlowType FlowType { get; }

        public ProcessInfo(string prevNodeId, string nextNodeId, ExecutionFlowType flowType)
        {
            PrevNodeId = prevNodeId;
            NextNodeId = nextNodeId;
            FlowType = flowType;
        }

        public enum ExecutionFlowType
        {
            ExecuteNext,
            Wait,
            EndExecution,
        }
    }
}