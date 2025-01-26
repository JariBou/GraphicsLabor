using System;
using System.Collections.Generic;
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
        private string _lastExecutionId = "";


        public string typename;

        public string id => m_guid;
        public Rect position => m_position;
        public List<PortInfo> PortInfos => m_ports;
        [SerializeField] private List<PortInfo> m_ports = new();

        [SerializeField] private bool m_pureExecutionDone = true;
        
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

        public NodeSystemNode()
        {
            NewGUID();
        }

        protected PortInfo GetExposedPropertyPortInfo(string propName)
        {
            PortInfo portInfo = m_ports.Find(info => info.ExposedPropertyName == propName);
            return portInfo;
        }
        
        protected NodeSystemNode GetNodeConnectedToInputPort(NodeSystemAsset graph, PortInfo exposedPropInfo, out int connectedPortIndex)
        {
            bool found = graph.GetConnectionToInputPort(exposedPropInfo, out NodeSystemConnection connectionToInputPort);
            connectedPortIndex = found ? connectionToInputPort.outputPort.portIndex : -1;
            return !found ? null : graph.GetNode(connectionToInputPort.outputPort.nodeId);
        }
        
        // Idealy this would be an extension on the prop
        public virtual T GetValueOfProp<T>(ExecInfo info, string exposedPropName)
        {
            PortInfo exposedPropertyPortInfo = GetExposedPropertyPortInfo(exposedPropName);
            NodeSystemNode connectedNode = GetNodeConnectedToInputPort(info.GraphInstance, exposedPropertyPortInfo, out int connectedPortIndex);
            if (connectedNode != null)
            {
                connectedNode.EnsurePureExecution(info);
                PortInfo connectedNodePortInfo = connectedNode.GetPort(connectedPortIndex);
                object value = connectedNode.GetType().GetField(connectedNodePortInfo.ExposedPropertyName).GetValue(connectedNode);
                // object value = connectedNode.GetValueOfProp<T>(info, connectedNodePortInfo.ExposedPropertyName);
                if (value != null)
                {
                    return (T)value;
                }
            }
            
            return (T)GetType().GetField(exposedPropName).GetValue(this);
        }

        private void EnsurePureExecution(ExecInfo info)
        {
            // So that the same exec doesn't trigger multiple OnProcess
            if (!IsPure || (/*PureExecutionDone && */info.ExecId == _lastExecutionId)) return;
            
            // Debug.Log(GetType() + " executing " + info.ExecId);
            _lastExecutionId = info.ExecId;
            //PureExecutionDone = true;
            OnProcess(info);
        }

        private void NewGUID()
        {
            m_guid = GuidSystem.NewGuid();
        }

        public void SetPosition(Rect position)
        {
            m_position = position;
        }

        public virtual ProcessInfo OnProcess(ExecInfo info)
        {
            NodeSystemAsset graph = info.GraphInstance;
            NodeSystemNode nextNode = GetNextNode(graph);
            if (nextNode != null)
            {
                return new ProcessInfo(id, nextNode.id, ProcessInfo.ExecutionFlowType.ExecuteNext);
            }
            return new ProcessInfo(id, "", ProcessInfo.ExecutionFlowType.EndExecution);;
        }

        public NodeSystemNode GetNextNode(NodeSystemAsset graph)
        {
            return graph.GetNodeFromOutputConnection(m_guid, 0);
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
            ExecuteNext, Wait, EndExecution
        }
    }

    [Serializable]
    public struct PortInfo
    {
        [SerializeField] private string _exposedPropertyName;
        [SerializeField] private string _ownerId;
        [SerializeField] private int _portIndex;
        [FormerlySerializedAs("_flowType")] [FormerlySerializedAs("_portType")] [SerializeField] private PropPortDirection _portDirection;

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

    public class ExecInfo
    {
        public string ExecId { get; private set; }
        public NodeSystemAsset GraphInstance { get; }
        public INodeSystemExecutioner NodeSystemExecutioner { get; }

        public ExecInfo(NodeSystemAsset graphInstance, INodeSystemExecutioner nodeSystemExecutioner)
        {
            GraphInstance = graphInstance;
            NodeSystemExecutioner = nodeSystemExecutioner;
            ExecId = GuidSystem.NewGuid();
        }
    }

    public interface INodeSystemExecutioner
    {
        public void TickProcess();

        public MonoBehaviour GetObject();
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
