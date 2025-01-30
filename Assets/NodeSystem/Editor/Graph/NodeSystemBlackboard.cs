using System;
using System.Linq;
using NodeSystem.Runtime.BlackBoard;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph
{
    public class NodeSystemBlackboard : Blackboard
    {
        private NodeSystemView m_associatedGraphView;
        public new Action<NodeSystemBlackboard> addItemRequested { get; set; }
        
        public NodeSystemBlackboard(NodeSystemView associatedGraphView) : base(associatedGraphView)
        {
            m_associatedGraphView = associatedGraphView;
            
            base.addItemRequested = blackboard =>
            {
                addItemRequested?.Invoke((NodeSystemBlackboard)blackboard);
            };

            Add(new BlackboardSection {title = "Exposed Variables"});
            
            foreach (BlackboardProperty property in associatedGraphView.ExposedProperties)
            {
                AddProperty(property, true);
            }
            
            // moveItemRequested += MoveItemRequested;
        }

        // private void MoveItemRequested(Blackboard arg1, int arg2, VisualElement arg3)
        // {
        //     Debug.Log(arg3.name);
        // }


        public void AddProperty(BlackboardProperty blackboardProperty, bool loadMode)
        {
            var localPropertyName = blackboardProperty.PropertyName;
            var localPropertyValue = blackboardProperty.PropertyValue;
            if (!loadMode)
            {
                while (m_associatedGraphView.ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            BlackboardProperty item = new BlackboardProperty();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            
            
            if (!loadMode)
            {
                m_associatedGraphView.ExposedProperties.Add(item);
                // m_associatedGraphView.ModifyExposedProperties(exposedProperties => { exposedProperties.Add(item); });
            }
            

            VisualElement container = new VisualElement();
            BlackboardField field = new BlackboardField {text = localPropertyName, typeText = "string"};
            container.Add(field);

            TextField propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var index = m_associatedGraphView.ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                m_associatedGraphView.ExposedProperties[index].PropertyValue = evt.newValue;
            });
            BlackboardRow sa = new BlackboardRow(field, propertyValueTextField);
            container.Add(sa);
            Add(container);
        }

        public void UnsubscribeFromEvents()
        {
            // moveItemRequested -= MoveItemRequested;
        }
    }
}