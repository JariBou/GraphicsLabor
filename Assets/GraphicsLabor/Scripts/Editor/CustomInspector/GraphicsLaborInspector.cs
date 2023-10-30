using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.CustomInspector
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public class GraphicsLaborInspector : UnityEditor.Editor
    {
        private List<SerializedProperty> _serializedProperties = new();
        private IEnumerable<MethodInfo> _methods;

        protected virtual void OnEnable()
        {
            _methods = ReflectionUtility.GetAllMethods(
                target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);
        }

        public override void OnInspectorGUI()
        {
            GetSerializedProperties(ref _serializedProperties);

            bool anyLaborerAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<ILaborerAttribute>(p) != null);
            if (!anyLaborerAttribute)
            {
                DrawDefaultInspector();
            }
            else
            {
                DrawSerializedProperties();
            }
            // TODO: Draw Properties maybe with a Custom ShowProperty Attribute?
            DrawButtons();
        }

        protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
        {
            outSerializedProperties.Clear();
            using (SerializedProperty iterator = serializedObject.GetIterator())
            {
                if (iterator.NextVisible(true))
                {
                    do
                    {
                        outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
                    }
                    while (iterator.NextVisible(false));
                }
            }
        }

        protected void DrawSerializedProperties()
        {
            serializedObject.Update();
            
            foreach (SerializedProperty property in _serializedProperties)
            {
                if (property.name.Equals("m_Script", System.StringComparison.Ordinal))
                {
                    using (new EditorGUI.DisabledScope(disabled: true))
                    {
                        EditorGUILayout.PropertyField(property);
                    } continue;
                }
                LaborerEditorGUI.PropertyField(property);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawButtons(bool drawHeader = false)
        {
            if (!_methods.Any()) return;
            
            foreach (MethodInfo method in _methods)
            {
                LaborerEditorGUI.Button(serializedObject.targetObject, method);
            }
        }
    }
}

