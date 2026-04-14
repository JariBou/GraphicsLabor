using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Executioners;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary.Events
{
    [NodeInfo("Flower Set Event Node", "Events/Flower set", FlowDirection.Output)]
    public class FlowerSetEventNode : EventNodeBase<FlowerSetEventData>
    {
        // Strings for mental representation
        [ExposedProperty(PropPortDirection.Output, labelOnly: true)]
        public string flower;

        [ExposedProperty(PropPortDirection.Output, labelOnly: true)]
        public string leftFlower;

        [ExposedProperty(PropPortDirection.Output, labelOnly: true)]
        public string rightFlower;


        public override async Awaitable Invoke(ExecContext ctx, FlowerSetEventData eventData)
        {
            flower = eventData.flower;
            leftFlower = eventData.leftFlower;
            rightFlower = eventData.rightFlower;

            await NodeGlobalExecutioner.Instance.RunNode(ctx, this);
        }
    }
}