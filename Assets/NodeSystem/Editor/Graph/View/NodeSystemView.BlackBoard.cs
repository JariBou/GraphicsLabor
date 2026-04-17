using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Runtime.BlackBoard;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace NodeSystem.Editor.Graph.View
{
    public partial class NodeSystemView
    {
        public void AddBlackboardProperty(BlackboardProperty blackboardProperty, bool b)
        {
            // doesn't work for some reason?
            Undo.RecordObject(SerializedObject.targetObject, "Add BlackboardProperty");
            m_blackboard.AddProperty(blackboardProperty, b);
        }

        private void GenerateBlackBoard()
        {
            NodeSystemBlackboard blackboard = new(this)
            {
                addItemRequested = _ =>
                {
                    Debug.Log("ahah");
                    AddBlackboardProperty(new BlackboardProperty(), false);
                },
                editTextRequested = (_, element, newValue) =>
                {
                    string oldPropertyName = ((BlackboardField)element).text;
                    if (ExposedProperties.Any(x => x.PropertyName == newValue))
                    {
                        EditorUtility.DisplayDialog("Error",
                            "This property name already exists, please chose another one.",
                            "OK");
                        return;
                    }

                    int targetIndex = ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                    ExposedProperties[targetIndex].PropertyName = newValue;

                    // m_currentView.ModifyExposedProperties(exposedProperties =>
                    // {
                    //     exposedProperties[targetIndex].PropertyName = newValue;
                    // });
                    ((BlackboardField)element).text = newValue;
                }
            };

            blackboard.SetPosition(new Rect(10, 30, 200, 300));
            Add(blackboard);
            m_blackboard = blackboard;
        }

        // Not used anymore
        public NodeSystemBlackboard GetNodeSystemBlackboard()
        {
            return m_blackboard;
        }

        public void ClearBlackBoardAndExposedProperties()
        {
            //ExposedProperties.Clear();
            m_blackboard.Clear();
        }

        [Obsolete]
        public void ModifyExposedProperties(Action<List<BlackboardProperty>> action)
        {
            action.Invoke(ExposedProperties);
            // Wait actually we don't need this, let's keep it still as an event possible source
            // action.Invoke(m_nodeSystem.ExposedProperties);
            // or if we create a setter
            // m_nodeSystem.ExposedProperties = ExposedProperties;
        }
    }
}