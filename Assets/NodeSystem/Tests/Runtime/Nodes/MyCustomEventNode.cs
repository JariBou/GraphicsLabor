using System;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.NodesLibrary.Events;
using UnityEngine;

namespace NodeSystem.Tests.Runtime
{
    [EventNodeInfo("My Custom Event Node", "Node System Tests/My Custom Event Node"), Serializable]
    public class MyCustomEventNode : EventNodeBase<MyCustomEventData>
    {
        [EventExposedProperty]
        public int someInt;
        [EventExposedProperty]
        public string someString;
        [EventExposedProperty]
        public bool someBool;
        
        [EventExposedProperty]
        public MyCustomEventData  eventData;
        
        public override Awaitable Invoke(ExecContext ctx, MyCustomEventData eventData)
        {
            someInt = eventData.someInt;
            someString = eventData.someString;
            someBool = eventData.someBool;
            
            this.eventData= eventData;
            
            return DefaultInvoke(ctx);
        }
    }
}