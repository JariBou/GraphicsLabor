//
//
// AUTO-GENERATED CODE - DO NOT MODIFY BY HAND!
//
// To regenerate this file, look at the NodeSystem's Node System Code Generator in Editor
//
//
using System;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core.PortConfigEnums;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Core;
using UnityEngine;

using NodeSystem.Tests.Runtime;

namespace NodeSystem.Generated.EventNodes.BreakNodes
{
	//[NodeInfo("BreakMyCustomEventDataNode", "BreakNodes/BreakMyCustomEventDataNode", isPure: true), Serializable]
	public class BreakMyCustomEventDataNode_Model : NodeSystemNode
	{
		// Input
		[ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
		public MyCustomEventData eventData;

		// Outputs
		[EventExposedProperty]
		public int someInt;
		[EventExposedProperty]
		public string someString;
		[EventExposedProperty]
		public bool someBool;

        public override async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)
        {
            MyCustomEventData customEventData = await GetValueOfProp<MyCustomEventData>(context, nameof(eventData));
            
            someInt = customEventData.someInt;
            someString = customEventData.someString;
            someBool = customEventData.someBool;
            
            return await base.OnProcessAsync(context);
        }
    }
}
