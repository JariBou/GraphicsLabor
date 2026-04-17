using System;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Extensions;
using NodeSystem.Runtime.References;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NodeSystem.Tests.Runtime
{
    public class MyCustomEventTriggerer : MonoBehaviour
    {
        [SerializeField] private TMP_InputField someStringInputField;
        [SerializeField] private TMP_InputField someIntInputField;
        [SerializeField] private Toggle sommeBoolToggle;
        
        [SerializeField] private NodeSystemAsset m_graphAsset;


        public void TriggerEvent()
        {
            NodeSystemAsset graphInstance = NodeSystemBank.GetGraphInstance(m_graphAsset);
            MyCustomEventData eventData = new MyCustomEventData()
            {
                someString = someStringInputField.text,
                someInt = Convert.ToInt32(someIntInputField.text),
                someBool = sommeBoolToggle.isOn,
            };
            graphInstance.TryCallEvent(eventData);
        }
    }
}