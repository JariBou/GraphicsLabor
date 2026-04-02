using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.NodesLibrary.Process;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("TestNode", "WIP/Test Start Diff Node", FlowDirection.Output)]
    public class TestStartDiffNode : GameObjectSourceNode
    {
        // [ExposedProperty(PropPortDirection.Input, portType: typeof(TestClass), preferredLocation: PropContainerLocation.InputContainer)]
        // public SerializableRef Source = new();
        
        // [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        // public GraphBankAsset TestIn1;
        //
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public ScriptableObject TestIn2;
        
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public SerializableSourceType TestIn;
        
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer), Tooltip("HEEEEYAAAAA")]
        public SerializableCompRef<Canvas> TestCompRef;
        //
        //
        // [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        // public GraphBankAsset TestOut1;
        //
        // [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        // public ScriptableObject TestOut2;
        
        // [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        // public TestFlags TestFlags;
        
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            // GetValueOfProp<SerializableRef>(info, nameof(Source));
            return base.OnProcess(info);
        }

    }
}