using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility.GUI;
using GraphicsLabor.Scripts.Editor.Utility.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public class SerializedDictionaryDrawer : PropertyDrawerBase
    {
        private bool _test;
        // TODO: make this more beautiful, with Separate Instances in a dictionnary with property.propertyPath as key
        // And there should be no need to be careful of code compilation since we are working with Serialized Dictionaries
        private ReorderableList _reorderableList;
        private SerializedProperty _listProperty;
        private SerializedProperty _property;
        private SerializedProperty _propertyList;
        private List<SerializedProperty> _testList;
        private Dictionary<int, bool> _foldoutStates = new ();
        protected override void OnSelfGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            _property ??= property;
            rect.height = LaborerGUIUtility.SingleLineHeight;
            EditorGUI.BeginProperty(rect, PropertyUtility.GetLabel(property), property);
            
            EditorGUI.LabelField(rect, PropertyUtility.GetLabel(property));

            rect.y += LaborerGUIUtility.SingleLineHeight;
            
            GetReorderableList(property).DoList(rect);
            
            EditorGUI.EndProperty();
            property.serializedObject.ApplyModifiedProperties();
        }

        protected override float GetSelfPropertyHeight(SerializedProperty property, GUIContent label)
        {
           
            return GetReorderableList(property).GetHeight() + LaborerGUIUtility.SingleLineHeight * 2;
        }

        protected ReorderableList GetReorderableList(SerializedProperty property)
        {
            if (_reorderableList != null) return _reorderableList;
            
            _propertyList = property.FindPropertyRelative("SerializedKeyValues");
            _reorderableList = new ReorderableList(property.serializedObject, _propertyList);
            _reorderableList.onAddCallback += OnAdd;
            
            _reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Serialized Dictionary");
            };
            
            _reorderableList.drawElementCallback += OnDrawElement;

            _reorderableList.elementHeightCallback += OnElementHeight;
            
            return _reorderableList;
        }

        private float OnElementHeight(int index)
        {
            float height = LaborerGUIUtility.SingleLineHeight;
            
            if (_foldoutStates.GetValueOrDefault(index, false))
            {
                SerializedProperty key = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
                SerializedProperty value = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Value");

                height += Math.Max(EditorGUI.GetPropertyHeight(key), EditorGUI.GetPropertyHeight(value));
            }
            
            return height;
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty key = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
            SerializedProperty value = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Value");
            
            _foldoutStates[index] = EditorGUI.Foldout(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), _foldoutStates.GetValueOrDefault(index, false), index.ToString());
            if (_foldoutStates[index])
            {
                rect.y += LaborerGUIUtility.SingleLineHeight;
                rect.width /= 2;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), key, 
                    PropertyUtility.GetLabel(key), true);

                rect.x += rect.width;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), value, 
                    PropertyUtility.GetLabel(value), true);

                rect.height += LaborerGUIUtility.SingleLineHeight;
            }
        }
        private void OnAdd(ReorderableList list)
        {
            int length = _propertyList.arraySize;
            _propertyList.InsertArrayElementAtIndex(length);
        }
    }

    public static class Extensions
    {
        public static Rect CutLeft(this Rect rect, float width) => new Rect(rect.x + width, rect.y, rect.width - width, rect.height);

    }
}