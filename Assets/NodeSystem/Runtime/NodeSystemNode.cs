using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    [Serializable]
    public class NodeSystemNode
    {
        [SerializeField] private string m_guid;
        [SerializeField] private Rect m_position;
        [SerializeField] private bool m_isPure;


        public string typename;
        [SerializeField] private List<PortInfo> m_ports = new();

        [SerializeField] private bool m_pureExecutionDone = true;
        private string _lastExecutionId = "";

        public NodeSystemNode()
        {
            NewGUID();
        }

        public string id => m_guid;
        public Rect position => m_position;
        public List<PortInfo> PortInfos => m_ports;

        public bool PureExecutionDone
        {
            get => m_pureExecutionDone;
#if UNITY_EDITOR
            set => m_pureExecutionDone = value;
#else
            private set => m_pureExecutionDone = value;
#endif
        }

        public bool IsPure
        {
            get => m_isPure;
#if UNITY_EDITOR
            set => m_isPure = value;
#else
            private set => m_isPure = value;
#endif
        }

        protected PortInfo GetExposedPropertyPortInfo(string propName)
        {
            PortInfo portInfo = m_ports.Find(info => info.ExposedPropertyName == propName);
            return portInfo;
        }

        protected NodeSystemNode GetNodeConnectedToInputPort(NodeSystemAsset graph, PortInfo exposedPropInfo,
            out int connectedPortIndex)
        {
            bool found = graph.GetConnectionToPort(exposedPropInfo, out NodeSystemConnection connectionToInputPort);
            connectedPortIndex = found ? connectionToInputPort.OutputPort.PortIndex : -1;
            return !found ? null : graph.GetNode(connectionToInputPort.OutputPort.NodeId);
        }

        // Idealy this would be an extension on the prop
        public virtual async Awaitable<T> GetValueOfProp<T>(ExecContext context, string exposedPropName)
        {
            PortInfo exposedPropertyPortInfo = GetExposedPropertyPortInfo(exposedPropName);
            NodeSystemNode connectedNode = GetNodeConnectedToInputPort(context.GraphInstance, exposedPropertyPortInfo,
                out int connectedPortIndex);
            if (connectedNode != null)
            {
                await connectedNode.EnsurePureExecution(context);
                PortInfo connectedNodePortInfo = connectedNode.GetPort(connectedPortIndex);
                object value = connectedNode.GetType().GetField(connectedNodePortInfo.ExposedPropertyName)
                    .GetValue(connectedNode);
                // object value = connectedNode.GetValueOfProp<T>(info, connectedNodePortInfo.ExposedPropertyName);
                if (value != null) return (T)value;
            }

            return (T)GetType().GetField(exposedPropName).GetValue(this);
        }

        private async Awaitable EnsurePureExecution(ExecContext context)
        {
            // So that the same exec doesn't trigger multiple OnProcess
            if (!IsPure || /*PureExecutionDone && */context.ExecId == _lastExecutionId) return;

            // Debug.Log(GetType() + " executing " + info.ExecId);
            _lastExecutionId = context.ExecId;
            //PureExecutionDone = true;
            await OnProcess(context);
        }

        private void NewGUID()
        {
            m_guid = GuidSystem.NewGuid();
        }

        public void SetPosition(Rect newPosition)
        {
            m_position = newPosition;
        }

        public virtual async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            NodeSystemAsset graph = context.GraphInstance;
            NodeSystemNode nextNode = GetNextNode(graph);
            if (nextNode != null) return await ContinueExecution(nextNode.id);

            return await EndExecution();
        }

        public NodeSystemNode GetNextNode(NodeSystemAsset graph)
        {
            return GetNodeConnectedToPort(graph, 0);
        }

        public NodeSystemNode GetNodeConnectedToPort(NodeSystemAsset graph, int portIndex)
        {
            return graph.GetNodeFromOutputConnection(m_guid, portIndex);
        }

        public bool Equals(NodeSystemNode obj)
        {
            if (obj != null) return obj.id == id;
            return false;
        }

        public PortInfo GetPort(int portIndex)
        {
            PortInfo portInfo = m_ports.Find(info => info.PortIndex == portIndex);
            return portInfo;
        }

        public void AddPortInfo(PortInfo portInfo)
        {
            m_ports.Add(portInfo);
        }

        #region FlowControl

        protected async Awaitable<ProcessInfo> EndExecution()
        {
            return await Task.FromResult(new ProcessInfo(id, "", ProcessInfo.ExecutionFlowType.EndExecution));
        }

        protected async Awaitable<ProcessInfo> ContinueExecution(string nextNodeId)
        {
            return await Task.FromResult(new ProcessInfo(id, nextNodeId, ProcessInfo.ExecutionFlowType.ExecuteNext));
        }
        
        protected async Awaitable<ProcessInfo> ContinueExecution(ExecContext ctx)
        {
            NodeSystemAsset graph = ctx.GraphInstance;
            NodeSystemNode nextNode = GetNextNode(graph);
            if (nextNode != null)
            {
                return await ContinueExecution(nextNode.id);
            }
            return await EndExecution();
        }

        #endregion
    }

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
            EndExecution
        }
    }

    [Serializable]
    public struct PortInfo
    {
        [SerializeField] private string _exposedPropertyName;
        [SerializeField] private string _ownerId;
        [SerializeField] private int _portIndex;

        [FormerlySerializedAs("_flowType")] [FormerlySerializedAs("_portType")] [SerializeField]
        private PropPortDirection _portDirection;

        public readonly string ExposedPropertyName => _exposedPropertyName;

        public readonly string OwnerId => _ownerId;

        public readonly int PortIndex => _portIndex;

        public readonly PropPortDirection PortDirection => _portDirection;

        public PortInfo(string exposedPropertyName, string ownerId, int portIndex, PropPortDirection portDirection)
        {
            _exposedPropertyName = exposedPropertyName;
            _ownerId = ownerId;
            _portIndex = portIndex;
            _portDirection = portDirection;
        }
    }

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


    // public static class ExposedPropExtensions
    // {
    //     public static T GetValueOfProp<T>(this object obj, NodeSystemAsset graph, NodeSystemNode node)
    //     {
    //         // nameof()actually captures 'obj' and not the name of the property... sad
    //         return node.GetValueOfProp<T>(graph, nameof(obj));
    //     }
    // }
}