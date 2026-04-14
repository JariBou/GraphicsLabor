using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Utility;
using UnityEngine;

namespace NodeSystem.Runtime.References
{
    [CreateAssetMenu(menuName = "NodeSystem/New Graph Bank")]
    public class GraphBankAsset : ScriptableObject
    {
        private Dictionary<string, NodeSystemAsset> _graphBank;

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

        public void Initialize()
        {
            _graphBank = new Dictionary<string, NodeSystemAsset>(_nodeSystems.Count);
            foreach (NodeSystemAsset graph in _nodeSystems)
            {
                if (graph != null)
                {
                    RegisterGraph(graph);
                }
            }
        }

        private void OnValidate()
        {
            // _graphBank = new Dictionary<string, NodeSystemAsset>(_nodeSystems.Count);
            // foreach (NodeSystemAsset graph in _nodeSystems)
            // {
            //     if (graph != null)
            //     {
            //         RegisterGraph(graph);
            //     }
            // }
        }
    }
}