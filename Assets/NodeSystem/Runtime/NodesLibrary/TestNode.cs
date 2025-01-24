using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.Nodes
{
    // I have no clue how to tackle this
    [NodeInfo("TestNode", "TestNode", FlowDirection.None, isPure: true)]
    public class TestNode : NodeSystemNode
    {
        // [ExposedProperty(PropPortDirection.Input, portType: typeof(TestClass), preferredLocation: PropContainerLocation.InputContainer)]
        // public SerializableRef Source = new();
        
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public GraphBankAsset TestIn1;
        
        [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public ScriptableObject TestIn2;


        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public GraphBankAsset TestOut1;
        
        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public ScriptableObject TestOut2;
        
        public override ProcessInfo OnProcess(ExecInfo info)
        {
            // GetValueOfProp<SerializableRef>(info, nameof(Source));
            return base.OnProcess(info);
        }

    }

    public class TestClass : MonoBehaviour
    {
        
    }
}