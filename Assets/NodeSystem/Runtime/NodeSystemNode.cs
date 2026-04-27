using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    [Serializable]
    public abstract partial class NodeSystemNode
    {
        [FormerlySerializedAs("m_guid"), SerializeField]
        protected internal string guid;

        [FormerlySerializedAs("m_isPure"), SerializeField]
        protected bool isPure;

        [FormerlySerializedAs("m_ports"), SerializeField]
        protected internal List<PortInfo> ports = new();

        [FormerlySerializedAs("pureExecutionDone"), FormerlySerializedAs("m_pureExecutionDone"), SerializeField]
        private bool _pureExecutionDone = true;

        private string _lastExecutionId = "";

        public string Typename => GetType().AssemblyQualifiedName;

        public string ID
        {
            get => guid;
        #if UNITY_EDITOR
            set => guid = value;
        #endif
        }

        public List<PortInfo> PortInfos => ports;


        public bool PureExecutionDone
        {
            get => _pureExecutionDone;
        #if UNITY_EDITOR
            set => _pureExecutionDone = value;
        #else
            protected set => m_pureExecutionDone = value;
        #endif
        }

        public bool IsPure
        {
            get => isPure;
        #if UNITY_EDITOR
            set => isPure = value;
        #else
            protected set => m_isPure = value;
        #endif
        }

        protected NodeSystemNode()
        {
            NewGuid();
        }

        protected PortInfo GetExposedPropertyPortInfo(string propName)
        {
            PortInfo portInfo = ports.Find(info => info.ExposedPropertyName == propName);
            return portInfo;
        }

        protected NodeSystemNode GetNodeConnectedToInputPort(NodeSystemAsset graph, PortInfo exposedPropInfo,
                                                             out int connectedPortIndex)
        {
            bool found = graph.GetConnectionToPort(exposedPropInfo, out NodeSystemConnection connectionToInputPort);
            connectedPortIndex = found ? connectionToInputPort.outputPort.portIndex : -1;
            return !found ? null : graph.GetNode(connectionToInputPort.outputPort.nodeId);
        }

        // Idealy this would be an extension on the prop
        public virtual async Awaitable<T> GetValueOfProp<T>(ExecContext context, string exposedPropName)
        {
            PortInfo exposedPropertyPortInfo = GetExposedPropertyPortInfo(exposedPropName);
            NodeSystemNode connectedNode = GetNodeConnectedToInputPort(context.GraphInstance, exposedPropertyPortInfo,
                                                                       out int connectedPortIndex);
            
            if (connectedNode == null) return (T)GetType().GetField(exposedPropName).GetValue(this);

            await connectedNode.EnsurePureExecution(context);
            PortInfo connectedNodePortInfo = connectedNode.GetPort(connectedPortIndex);
            object value = connectedNode.GetType().GetField(connectedNodePortInfo.ExposedPropertyName)
                                        .GetValue(connectedNode);
            // object value = connectedNode.GetValueOfProp<T>(info, connectedNodePortInfo.ExposedPropertyName);
            if (value != null) return (T)value;

            return (T)GetType().GetField(exposedPropName).GetValue(this);
        }

        private async Awaitable EnsurePureExecution(ExecContext context)
        {
            // So that the same exec doesn't trigger multiple OnProcess
            if (!IsPure || /*PureExecutionDone && */context.ExecId == _lastExecutionId) return;

            // Debug.Log(GetType() + " executing " + info.ExecId);
            _lastExecutionId = context.ExecId;
            //PureExecutionDone = true;
            await OnProcessAsync(context);
        }

        private void NewGuid()
        {
            guid = GuidSystem.NewGuid();
        }

        public virtual async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            NodeSystemAsset graph = context.GraphInstance;
            NodeSystemNode nextNode = GetNextNode(graph);
            if (nextNode != null) return await ContinueExecution(nextNode.ID);

            return await EndExecution();
        }

        public NodeSystemNode GetNextNode(NodeSystemAsset graph)
        {
            //TODO: currently this doesn't check if it's a flow port...
            return GetNodeConnectedToPort(graph, 0);
        }

        public NodeSystemNode GetNodeConnectedToPort(NodeSystemAsset graph, int portIndex)
        {
            return graph.GetNodeFromOutputConnection(guid, portIndex);
        }

        public bool Equals(NodeSystemNode obj)
        {
            if (obj != null) return obj.ID == ID;

            return false;
        }

        public PortInfo GetPort(int portIndex)
        {
            PortInfo portInfo = ports.Find(info => info.PortIndex == portIndex);
            return portInfo;
        }

        public void AddPortInfo(PortInfo portInfo)
        {
            ports.Add(portInfo);
        }

        #region FlowControl

        protected async Awaitable<ProcessInfo> EndExecution()
        {
            return await Task.FromResult(new ProcessInfo(ID, "", ProcessInfo.ExecutionFlowType.EndExecution));
        }

        protected async Awaitable<ProcessInfo> ContinueExecution(string nextNodeId)
        {
            return await Task.FromResult(new ProcessInfo(ID, nextNodeId, ProcessInfo.ExecutionFlowType.ExecuteNext));
        }

        protected async Awaitable<ProcessInfo> ContinueExecution(ExecContext ctx)
        {
            NodeSystemAsset graph = ctx.GraphInstance;
            NodeSystemNode nextNode = GetNextNode(graph);
            if (nextNode != null) return await ContinueExecution(nextNode.ID);

            return await EndExecution();
        }

        #endregion
    }
}