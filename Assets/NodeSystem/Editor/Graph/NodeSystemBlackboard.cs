using System;
using System.Linq;
using NodeSystem.Editor.Graph.View;
using NodeSystem.Runtime.BlackBoard;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph
{
    [Obsolete]
    public class NodeSystemBlackboard : Blackboard
    {
        private readonly NodeSystemView _associatedGraphView;

        public Action<NodeSystemBlackboard> AddItemRequested { get; set; }

        public NodeSystemBlackboard(NodeSystemView associatedGraphView) : base(associatedGraphView)
        {
            _associatedGraphView = associatedGraphView;

            addItemRequested = blackboard => { AddItemRequested?.Invoke((NodeSystemBlackboard)blackboard); };

            Add(new BlackboardSection { title = "Exposed Variables" });

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
            string localPropertyName = blackboardProperty.propertyName;
            string localPropertyValue = blackboardProperty.propertyValue;
            if (!loadMode)
            {
                while (_associatedGraphView.ExposedProperties.Any(x => x.propertyName == localPropertyName))
                {
                    localPropertyName = $"{localPropertyName}(1)";
                }
            }

            BlackboardProperty item = new()
            {
                propertyName = localPropertyName, propertyValue = localPropertyValue,
            };


            if (!loadMode) _associatedGraphView.ExposedProperties.Add(item);
            // m_associatedGraphView.ModifyExposedProperties(exposedProperties => { exposedProperties.Add(item); });

            VisualElement container = new();
            BlackboardField field = new() { text = localPropertyName, typeText = "string" };
            container.Add(field);

            TextField propertyValueTextField = new("Value:")
            {
                value = localPropertyValue,
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                int index = _associatedGraphView.ExposedProperties.FindIndex(x => x.propertyName == item.propertyName);
                _associatedGraphView.ExposedProperties[index].propertyValue = evt.newValue;
            });
            BlackboardRow sa = new(field, propertyValueTextField);
            container.Add(sa);
            Add(container);
        }

        public void UnsubscribeFromEvents()
        {
            // moveItemRequested -= MoveItemRequested;
        }
    }
}