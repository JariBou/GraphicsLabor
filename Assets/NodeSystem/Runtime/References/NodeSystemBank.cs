using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.References
{
    [AddComponentMenu(NodeSystemConsts.AddComponentMenuCategoryName + "/Node System Bank")]
    public class NodeSystemBank : MonoBehaviour
    {
        private static NodeSystemBank _instance;

        [SerializeField] private GraphBankAsset _bank;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                _bank.Initialize();
            }
        }

        public static NodeSystemAsset GetGraphInstance(string guid)
        {
            if (_instance._bank.TryGetGraph(guid, out NodeSystemAsset graph)) return graph;
            Debug.LogError("No Graph with id '" + guid + "' was found.");
            return null;
        }

        public static NodeSystemAsset GetGraphInstance(NodeSystemAsset baseGraph)
        {
            string guid = baseGraph.GraphId;
            if (_instance._bank.TryGetGraph(guid, out NodeSystemAsset graph)) return graph;
            Debug.Log("Registering graph");
            return _instance._bank.RegisterGraph(baseGraph);
        }

        public bool HasBankAsset()
        {
            return _bank != null;
        }
    }
}