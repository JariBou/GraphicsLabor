using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Editor.Nodes;
using NodeSystem.Runtime;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph.View
{
    public partial class NodeSystemView
    {
        private Rect _copiedElementsCompoundRect;
        private List<NodeSystemNode> m_copiedNodesCache = new();

        private string CopyCutCallback(IEnumerable<GraphElement> elements)
        {
            List<GraphElement> enumerable = elements.ToList();
            Debug.Log("Copy/Cut Callback: " + enumerable.Count());
            m_copiedNodesCache.Clear();
            _copiedElementsCompoundRect = Rect.zero;
            foreach (GraphElement element in enumerable)
            {
                if (_copiedElementsCompoundRect == Rect.zero)
                {
                    _copiedElementsCompoundRect = element.layout;
                }
                _copiedElementsCompoundRect = RectUtils.Encompass(element.layout, _copiedElementsCompoundRect);
                
                if (element is NodeSystemEditorNode node)
                {
                    string nodeTypename = node.Node.Typename;
                    Type type = Type.GetType(nodeTypename);
                    if (type != null)
                    {
                        NodeSystemNode copy = (NodeSystemNode)(Activator.CreateInstance(type));
                        copy.CopyFrom(node.Node);
                        m_copiedNodesCache.Add(copy);
                    }

                    // NodeSystemNode.CopyFrom(node.Node);
                    // m_copiedNodesCache.Add(node.Node.CopyWithNewGuid());
                } else if (element is Edge edge)
                {
                    // TODO: actually we should traverse everything and update node id's 
                }
            }
            return "";
        }

        private void PasteCallback(string operationName, string data)
        {
            Debug.Log("Paste callback: " + operationName);
            if (operationName != "Paste" || m_copiedNodesCache.Count == 0) return;
            
            Vector2 compoundRectCenter = _copiedElementsCompoundRect.center;
            Vector2 displacement = this.ChangeCoordinatesTo(contentViewContainer, _mousePos) - compoundRectCenter;
            foreach (NodeSystemNode node in m_copiedNodesCache)
            {
                node.Displace(displacement);
                CopyBack(node);
            }
        }

        private NodeSystemEditorNode CopyBack(NodeSystemNode node)
        {
            Undo.RecordObject(SerializedObject.targetObject, "Added Node");
            
            m_nodeSystem.Nodes.Add(node);
            
            SerializedObject.Update();

            NodeSystemEditorNode nodeSystemEditorNode = AddNodeToGraph(node);
            BindToSerializedObject();
            return nodeSystemEditorNode;
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