using NodeSystem.Runtime;
using NodeSystem.Runtime.Extensions;
using NodeSystem.Runtime.NodesLibrary.Events;
using NodeSystem.Runtime.NodesLibrary.Process;
using NodeSystem.Runtime.References;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private NodeSystemAsset _graph;

    private NodeSystemAsset _graphInstance;
    
    // Start is called before the first frame update
    void Start()
    {
        _graphInstance = NodeSystemBank.GetGraphInstance(_graph);
        FlowerSetEventData fEventData = new()
        {
            flower = "Flower",
            leftFlower = "Left flower!",
            rightFlower = "Right flower!",
        };
        _graphInstance.TryCallEvent(fEventData);
        // Alternative ways:
        // EventNodeBase<FlowerSetEvent> eventNode = _graphInstance.FindEventNode<FlowerSetEvent>();
        
        //   _graphInstance.CallEvent(eventNode, fEvent);
        //   eventNode?.Invoke(new ExecContext(_graphInstance), fEvent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
