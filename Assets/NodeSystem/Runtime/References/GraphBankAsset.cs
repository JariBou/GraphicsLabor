using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Core.Utility;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Runtime.References
{
    [CreateAssetMenu(menuName = "NodeSystem/New Graph Bank")]
    public class GraphBankAsset : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<string, NodeSystemAsset> _graphBank;

        [SerializeField] private List<NodeSystemAsset> _nodeSystems;

        public NodeSystemAsset RegisterGraph(NodeSystemAsset graph)
        {
            string graphId = graph.GraphId;
            NodeSystemAsset newGraph = Instantiate(graph);
            newGraph.Init();
            _graphBank.Add(graphId, newGraph);
            return newGraph;
        }

        public bool TryGetGraph(string graphId, out NodeSystemAsset graph)
        {
            return _graphBank.TryGetValue(graphId, out graph);
        }

        private void OnValidate()
        {
            _graphBank = new SerializedDictionary<string, NodeSystemAsset>(_nodeSystems.Count);
            foreach (NodeSystemAsset graph in _nodeSystems)
            {
                RegisterGraph(graph);
            }
        }
    }
}