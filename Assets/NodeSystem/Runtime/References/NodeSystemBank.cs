using System;
using UnityEngine;

namespace NodeSystem.Runtime.References
{
    public class NodeSystemBank : MonoBehaviour
    {
        private static NodeSystemBank _instance;
        
        [SerializeField] private GraphBankAsset _bank;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }

        public static NodeSystemAsset GetGraphInstance(string guid)
        {
            if (_instance._bank.TryGetGraph(guid, out NodeSystemAsset graph)) return graph;
            else
            {
                Debug.LogError("No Graph with id '"+guid+"' was found.");
                return null;
            }
        }
        
        public static NodeSystemAsset GetGraphInstance(NodeSystemAsset baseGraph)
        {
            string guid = baseGraph.GraphId;
            if (_instance._bank.TryGetGraph(guid, out NodeSystemAsset graph)) return graph;
            else
            {
                return _instance._bank.RegisterGraph(baseGraph);
            }
        }
    }
}