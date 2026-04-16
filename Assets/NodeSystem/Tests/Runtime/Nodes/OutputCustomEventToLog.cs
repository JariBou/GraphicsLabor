using System;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Utils.RefSystem;
using TMPro;
using UnityEngine;

namespace NodeSystem.Tests.Runtime
{
    [Serializable, NodeInfo("Output Custom Event To Log Field", "Node System Tests/Output Custom Event To Log Field")]
    public class OutputCustomEventToLog : NodeSystemNode
    {
        [SerializeField, ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public SerializableCompRef<TMP_Text> logField;
        
        [SerializeField, ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]
        public MyCustomEventData eventData;

        public override async Awaitable<ProcessInfo> OnProcess(ExecContext context)
        {
            TMP_Text tmpText = (await GetValueOfProp<SerializableCompRef<TMP_Text>>(context, nameof(logField))).Get();
            MyCustomEventData data = await GetValueOfProp<MyCustomEventData>(context, nameof(eventData));
            tmpText.text = $"SomeString: {data.someString}\nSomeInt: {data.someInt}\nSomeBool: {data.someBool}";
            return await ContinueExecution(context);
        }
    }
}