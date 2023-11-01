using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEditor.Profiling;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Object), true)]
    public sealed class GraphicsLaborInspector : UnityEditor.Editor
    {
        private List<SerializedProperty> _serializedProperties = new();
        private IEnumerable<PropertyInfo> _properties;
        private IEnumerable<MethodInfo> _methods;
        private bool _isEditableScriptableObject;

        private void OnEnable()
        {
            _properties = ReflectionUtility.GetAllProperties(
                target, p => p.GetCustomAttributes(typeof(ShowPropertyAttribute), true).Length > 0);
            
            _methods = ReflectionUtility.GetAllMethods(
                target, m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0);

            _isEditableScriptableObject = ReflectionUtility
                .GetAllAttributesOfObject(target, c => c.AttributeType == typeof(EditableAttribute), true).Any();
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
            DrawProperties();
            DrawButtons();
            if (_isEditableScriptableObject)
            {
                Debug.Log("_isEditableScriptableObject");
                LaborerEditorGUI.EditableSoButton(serializedObject.targetObject, "Show Editor");
            }
        }

        private void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
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

        private void DrawSerializedProperties()
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
                LaborerEditorGUI.LayoutPropertyField(property);
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void DrawProperties()
        {
            if (!_properties.Any()) return;
            
            foreach (var property in _properties)
            {
                LaborerEditorGUI.LayoutProperty(serializedObject.targetObject, property);
            }
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

