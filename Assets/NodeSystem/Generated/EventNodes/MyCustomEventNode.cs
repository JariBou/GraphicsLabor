//
//
// AUTO-GENERATED CODE - DO NOT MODIFY BY HAND!
//
// To regenerate this file, look at the NodeSystem's Node System Code Generator in Editor
//
//
using System;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.NodesLibrary.Events;
using UnityEngine;

using NodeSystem.Tests.Runtime;

namespace NodeSystem.Generated.EventNodes
{
	[EventNodeInfo("ATest", "Ignore/ATest", isPure: false), Serializable]
	public class MyCustomEventNode : EventNodeBase<MyCustomEventData>
	{
		[EventExposedProperty]
		public MyCustomEventData eventData;


		public override Awaitable Invoke(ExecContext ctx, MyCustomEventData data)
		{
			eventData = data;
			return DefaultInvoke(ctx);
		}
	}
}
