using System;
using NodeSystem.Runtime.NodesLibrary.Events;

namespace NodeSystem.Tests.Runtime
{
    [Serializable]
    public class MyCustomEventData : EventData
    {
        public int someInt;
        public string someString;
        public bool someBool;
    }
}