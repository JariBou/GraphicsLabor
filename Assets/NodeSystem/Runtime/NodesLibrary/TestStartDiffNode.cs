using System.Collections.Generic;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core.PortConfigEnums;
using NodeSystem.Runtime.Core.RefSystem;
using NodeSystem.Runtime.NodesLibrary.Process;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.NodesLibrary
{
    // I have no clue how to tackle this
    [NodeInfo("Test Start Diff Node", "WIP/Test Start Diff Node", FlowDirection.Output)]
    public class TestStartDiffNode : GameObjectSourceNode
    {
        [FormerlySerializedAs("TestCompRef"),
         ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer),
         Tooltip("HEEEEYAAAAA")]
        public SerializableCompRef<Canvas> testCompRef;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer),
         Tooltip("HEEEEYAAAAA")]
        public SerializableCompRef<Transform> testCompRef2;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public SerializableGameObjectRef testIn;
        // [ExposedProperty(PropPortDirection.Input, portType: typeof(TestClass), preferredLocation: PropContainerLocation.InputContainer)]
        // public SerializableRef Source = new();

        // [ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        // public GraphBankAsset TestIn1;
        //
        [FormerlySerializedAs("TestIn2"),
         ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.OutputContainer)]
        public ScriptableObject testIn2;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.ExtensionContainer),
         Tooltip("HEEEEYAAAAA")]
        public List<string> testList;

        [ExposedProperty(PropPortDirection.Output, preferredLocation: PropContainerLocation.ExtensionContainer),
         Tooltip("HEEEEYAAAAA")]
        public List<SerializableCompRef<Transform>> testList2;
    }
}