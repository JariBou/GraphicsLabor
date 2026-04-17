using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime
{
    [Serializable]
    public abstract partial class NodeSystemNode
    {
        [SerializeField] protected internal string m_guid;
        [SerializeField] protected bool m_isPure;


        public string typename;
        public string Typename => GetType().AssemblyQualifiedName;
        [SerializeField] protected internal List<PortInfo> m_ports = new();

        [SerializeField] private bool m_pureExecutionDone = true;
        private string _lastExecutionId = "";

        public NodeSystemNode()
        {
            NewGUID();
        }

        public string id => m_guid;
        public List<PortInfo> PortInfos => m_ports;
   

        public bool PureExecutionDone
        {
            get => m_pureExecutionDone;
#if UNITY_EDITOR
            set => m_pureExecutionDone = value;
#else
            protected set => m_pureExecutionDone = value;
#endif
        }

        public bool IsPure
        {
            get => m_isPure;
#if UNITY_EDITOR
            set => m_isPure = value;
#else
            protected set => m_isPure = value;
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

        // public abstract NodeSystemNode CopyWithNewGuid();

        // public static TNode CopyFrom<TNode>(TNode src) where TNode : NodeSystemNode, new()
        // {
        //     TNode copy = new();
        //     return src.CopyToWithNewGuid(copy);
        // }
        //
        // public NodeSystemNode CopyToWithNewGuid(NodeSystemNode target)
        // {
        //     string newGuid = GuidSystem.NewGuid();
        //     target.m_guid = newGuid;
        //     target.m_ports = m_ports.Select(portInfo =>
        //             new PortInfo(portInfo.ExposedPropertyName, newGuid, portInfo.PortIndex, portInfo.PortDirection))
        //         .ToList();
        //     target.IsPure = m_isPure;
        //     target._position = _position;
        //     return target;
        // }
        //
        // public TNode CopyToWithNewGuid<TNode>(TNode target) where TNode : NodeSystemNode, new()
        // {
        //     string newGuid = GuidSystem.NewGuid();
        //     target.m_guid = newGuid;
        //     target.m_ports = m_ports.Select(portInfo =>
        //             new PortInfo(portInfo.ExposedPropertyName, newGuid, portInfo.PortIndex, portInfo.PortDirection))
        //         .ToList();
        //     target.IsPure = m_isPure;
        //     target._position = _position;
        //     return target;
        // }

        // public NodeSystemNode CopyWithNewGuid()
        // {
        //     string newGuid = GuidSystem.NewGuid();
        //     NodeSystemNode copy = new()
        //     {
        //         m_guid = newGuid,
        //         _position = _position,
        //         IsPure = m_isPure,
        //         m_ports = m_ports.Select(portInfo => new PortInfo(portInfo.ExposedPropertyName, newGuid, portInfo.PortIndex, portInfo.PortDirection)).ToList(),
        //     };
        //     return copy;
        // }
    }

    // public abstract class NodeSystemNode<TNode> : NodeSystemNode where TNode : NodeSystemNode, new()
    // {
    //     public override NodeSystemNode CopyWithNewGuid()
    //     {
    //         return CopyWithNewGuid_Impl();
    //     }
    //
    //     public virtual TNode CopyWithNewGuid_Impl()
    //     {
    //         TNode copy = new();
    //         return CopyToWithNewGuid(copy);
    //     }
    // }
}