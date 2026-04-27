using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Editor.Graph.Elements;
using NodeSystem.Editor.Nodes;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Utils;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph.View
{
    public partial class NodeSystemView
    {
        private readonly List<NodeSystemConnection> _copiedConnectionsCache = new();
        private readonly List<NodeSystemNode> _copiedNodesCache = new();

        private Rect _copiedElementsCompoundRect;

        private string CopyCutCallback(IEnumerable<GraphElement> elements)
        {
            List<GraphElement> enumerable = elements.ToList();
            Debug.Log("Copy/Cut Callback: " + enumerable.Count());
            _copiedNodesCache.Clear();
            _copiedElementsCompoundRect = Rect.zero;

            Dictionary<string, string> oldNewGuid = new();
            foreach (GraphElement element in enumerable)
            {
                if (_copiedElementsCompoundRect == Rect.zero) _copiedElementsCompoundRect = element.layout;
                _copiedElementsCompoundRect = RectUtils.Encompass(element.layout, _copiedElementsCompoundRect);

                switch (element)
                {
                    case NodeSystemEditorNode node:
                    {
                        string nodeTypename = node.Node.Typename;
                        Type type = Type.GetType(nodeTypename);
                        if (type != null)
                        {
                            string newGuid = oldNewGuid.TryAddAndGet(node.Node.ID, GuidSystem.NewGuid);
                            NodeSystemNode copy = (NodeSystemNode)Activator.CreateInstance(type);
                            copy.ID = newGuid;
                            copy.CopyDataFrom(node.Node);
                            _copiedNodesCache.Add(copy);
                        }

                        break;
                    }
                    case Edge edge:
                    {
                        Port edgeInput = edge.input;
                        Port edgeOutput = edge.output;
                        NodeSystemEditorNode inputNode = (NodeSystemEditorNode)edgeInput.node;
                        NodeSystemEditorNode outputNode = (NodeSystemEditorNode)edgeOutput.node;
                        string inputGuid = oldNewGuid.TryAddAndGet(inputNode.Node.ID, GuidSystem.NewGuid);
                        string outputGuid = oldNewGuid.TryAddAndGet(outputNode.Node.ID, GuidSystem.NewGuid);

                        _copiedConnectionsCache.Add(new NodeSystemConnection(inputGuid,
                                                                             inputNode.GetIndexOfPort(edgeInput),
                                                                             outputGuid,
                                                                             outputNode.GetIndexOfPort(edgeOutput)));
                        break;
                    }
                }
            }

            return "";
        }

        private void PasteCallback(string operationName, string data)
        {
            Debug.Log("Paste callback: " + operationName);
            if (operationName != "Paste" || _copiedNodesCache.Count == 0) return;

            RecordAction(operationName);
            Vector2 compoundRectCenter = _copiedElementsCompoundRect.center;
            Vector2 displacement = this.ChangeCoordinatesTo(contentViewContainer, _mousePos) - compoundRectCenter;
            foreach (NodeSystemNode node in _copiedNodesCache)
            {
                node.Displace(displacement);
                CopyBack(node);
            }

            foreach (NsEdge edgeToCreate in
                     _copiedConnectionsCache.Select(connection => new NsEdge(connection, GetNode)))
            {
                edgeToCreate.input.Connect(edgeToCreate);
                edgeToCreate.output.Connect(edgeToCreate);
                AddElement(edgeToCreate);
                CreateConnection(edgeToCreate);
            }
        }

        private void CopyBack(NodeSystemNode node)
        {
            Undo.RecordObject(SerializedObject.targetObject, "Added Node");

            _nodeSystem.Nodes.Add(node);

            SerializedObject.Update();

            AddNodeToGraph(node);
            BindToSerializedObject();
        }

        private bool CanPasteCallback(string data)
        {
            return true;
        }

        private void SubscribeToCopyCutPaste()
        {
            canPasteSerializedData += CanPasteCallback;
            unserializeAndPaste += PasteCallback;
            serializeGraphElements += CopyCutCallback;
        }

        private void RemoveCopyCutPasteCallbacks()
        {
            canPasteSerializedData -= CanPasteCallback;
            unserializeAndPaste -= PasteCallback;
            serializeGraphElements -= CopyCutCallback;
        }
    }
}