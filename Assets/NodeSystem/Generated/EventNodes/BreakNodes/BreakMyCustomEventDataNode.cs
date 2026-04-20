//
//
// AUTO-GENERATED CODE - DO NOT MODIFY BY HAND!
//
// To regenerate this file, look at the NodeSystem's Node System Code Generator in Editor
//
//
using System;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using NodeSystem.Runtime.Attributes;
using UnityEngine;

using NodeSystem.Tests.Runtime;

namespace NodeSystem.Generated.EventNodes.BreakNodes
{
	[NodeInfo("Break MyCustomEventData Node", "BreakNodes/BreakMyCustomEventDataNode", isPure: true), Serializable]
	public class BreakMyCustomEventDataNode : NodeSystemNode
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
			MyCustomEventData eventDataValue = await GetValueOfProp<MyCustomEventData>(context, nameof(eventData));

			someInt = eventDataValue.someInt;
			someString = eventDataValue.someString;
			someBool = eventDataValue.someBool;
			return await base.OnProcessAsync(context);
		}
	}
}
