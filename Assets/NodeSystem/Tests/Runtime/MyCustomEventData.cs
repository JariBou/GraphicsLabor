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
    
    // [Serializable]
    // [GenerateEventNode("ATest2", "Ignore/ATest2", isPure: false)]
    // public class MyCustomEventData2 : EventData
    // {
    //     public int someInt;
    //     public string someString;
    //     public bool someBool;
    // }
    //
    // [Serializable]
    // [GenerateEventNode("ATest3", "Ignore/ATest3", isPure: false)]
    // public class MyCustomEventData3 : EventData
    // {
    //     public int someInt;
    //     public string someString;
    //     public bool someBool;
    // }
    //
    // [Serializable]
    // [GenerateEventNode("ATest4", "Ignore/ATest2", isPure: false)]
    // public class MyCustomEventData4 : EventData
    // {
    //     public int someInt;
    //     public string someString;
    //     public bool someBool;
    // }
    //
    // [Serializable]
    // [GenerateEventNode("ATest5", "Ignore/ATest2", isPure: false)]
    // public class MyCustomEventData5 : EventData
    // {
    //     public int someInt;
    //     public string someString;
    //     public bool someBool;
    // }
}