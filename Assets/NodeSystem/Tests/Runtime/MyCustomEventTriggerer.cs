using System;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Extensions;
using NodeSystem.Runtime.References;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NodeSystem.Tests.Runtime
{
    public class MyCustomEventTriggerer : MonoBehaviour
    {
        [FormerlySerializedAs("someStringInputField"), SerializeField]
        private TMP_InputField _someStringInputField;

        [FormerlySerializedAs("someIntInputField"), SerializeField]
        private TMP_InputField _someIntInputField;

        [FormerlySerializedAs("sommeBoolToggle"), SerializeField]
        private Toggle _sommeBoolToggle;

        [FormerlySerializedAs("m_graphAsset"), SerializeField]
        private NodeSystemAsset _graphAsset;


        public void TriggerEvent()
        {
            NodeSystemAsset graphInstance = NodeSystemBank.GetGraphInstance(_graphAsset);
            MyCustomEventData eventData = new()
            {
                someString = _someStringInputField.text,
                someInt = Convert.ToInt32(_someIntInputField.text),
                someBool = _sommeBoolToggle.isOn
            };
            graphInstance.TryCallEvent(eventData);
            // Alternative ways:
            // EventNodeBase<MyCustomEventData> eventNode = _graphInstance.FindEventNode<MyCustomEventData>();
        
            // >  _graphInstance.CallEvent(eventNode, eventData);
            // >  eventNode?.Invoke(new ExecContext(_graphInstance), eventData);
            
        }
    }
}