using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.References;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("TestNode", "WIP/TestNode", FlowDirection.None, isPure: true)]
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
        
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            // GetValueOfProp<SerializableRef>(info, nameof(Source));
            return base.OnProcess(info);
        }

    }

    public class TestClass : MonoBehaviour
    {
        
    }

    [System.Flags]
    public enum TestFlags
    {
        None = 0,
        Test1 = 1 << 0,
        Test2 = 1 << 1,
        Test4 = 1 << 2,
        Test16 = 1 << 3,
    }
}