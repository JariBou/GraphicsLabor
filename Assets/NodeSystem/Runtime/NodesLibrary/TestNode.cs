using System;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Core.PortConfigEnums;
using UnityEngine;
using UnityEngine.Serialization;

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

        [FormerlySerializedAs("TestFlags"),
         ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public TestFlags testFlags;
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