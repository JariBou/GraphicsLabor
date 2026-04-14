using System;
using NodeSystem.Runtime.Attributes;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("TestNode", "WIP/TestNode", FlowDirection.None, true)]
    public class TestNode : NodeSystemNode
    {
        // [ExposedProperty(PropPortDirection.Input, portType: typeof(TestClass), preferredLocation: PropContainerLocation.InputContainer)]
        // public SerializableRef Source = new();

        // [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        // public GraphBankAsset TestIn1;
        //
        // [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        // public ScriptableObject TestIn2;
        //
        //
        // [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        // public GraphBankAsset TestOut1;
        //
        // [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        // public ScriptableObject TestOut2;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public TestFlags TestFlags;

        public override Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            // GetValueOfProp<SerializableRef>(info, nameof(Source));
            return base.OnProcess(context);
        }
    }

    public class TestClass : MonoBehaviour
    {
    }

    [Flags]
    public enum TestFlags
    {
        None = 0,
        Test1 = 1 << 0,
        Test2 = 1 << 1,
        Test4 = 1 << 2,
        Test16 = 1 << 3
    }
}