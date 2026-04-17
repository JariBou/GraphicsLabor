using System.Collections.Generic;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.References
{
    [CreateAssetMenu(menuName = NodeSystemConsts.AddComponentMenuCategoryName + "/New Graph Bank")]
    public sealed class GraphBankAsset : ScriptableObject
    {
        [SerializeField] private List<NodeSystemAsset> _nodeSystems;
        private Dictionary<string, NodeSystemAsset> _graphBank;

        public NodeSystemAsset RegisterGraph(NodeSystemAsset graph)
        {
            // TODO: shouldn't  be a concern for now but since GraphId is serialized, there is a possibility that, on creation, a graph gets the same Id as another one
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
                if (graph != null)
                    RegisterGraph(graph);
        }
    }
}