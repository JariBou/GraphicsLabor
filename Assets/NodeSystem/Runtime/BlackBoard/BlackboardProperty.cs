using System;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.BlackBoard
{
    [Serializable]
    public class BlackboardProperty
    {
        [FormerlySerializedAs("PropertyName")] public string propertyName = "New String";

        [FormerlySerializedAs("PropertyValue")]
        public string propertyValue = "New Value";
    }
}