using System;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.NodesLibrary.Events;

namespace NodeSystem.Tests.Runtime
{
    [Serializable]
    [GenerateEventNode("ATest", "Ignore/ATest", isPure: false)]
    public class MyCustomEventData : EventData
    {
        public int someInt;
        public string someString;
        public bool someBool;
    }
}