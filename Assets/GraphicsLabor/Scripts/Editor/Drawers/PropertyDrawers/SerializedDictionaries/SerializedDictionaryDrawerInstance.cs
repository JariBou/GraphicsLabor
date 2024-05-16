using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Settings;
using GraphicsLabor.Scripts.Editor.Utility.GUI;
using GraphicsLabor.Scripts.Editor.Utility.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers.PropertyDrawers.SerializedDictionaries
{
    public class SerializedDictionaryDrawerInstance
    {
        private ReorderableList _reorderableList;
        private SerializedProperty _listProperty;
        private readonly SerializedProperty _property;
        private SerializedProperty _propertyList;
        private Dictionary<int, bool> _foldoutStates = new ();
        private SerializedProperty _drawAsFoldout;

        public SerializedDictionaryDrawerInstance(SerializedProperty property)
        {
            _property = property;
            _propertyList = property.FindPropertyRelative("SerializedKeyValues");
            _drawAsFoldout = property.FindPropertyRelative("_drawElementsAsFoldout");
        }
        
        public void DoGUI(Rect rect, GUIContent label)
        {
            rect.height = LaborerGUIUtility.SingleLineHeight;
            EditorGUI.BeginProperty(rect, PropertyUtility.GetLabel(_property), _property);
            
            GetReorderableList(_property).DoList(rect);
            
            EditorGUI.EndProperty();
            _property.serializedObject.ApplyModifiedProperties();
        }

        public float GetHeight()
        {
            return GetReorderableList(_property).GetHeight();
        }

        protected ReorderableList GetReorderableList(SerializedProperty property)
        {
            if (_reorderableList != null) return _reorderableList;
            
            _reorderableList = new ReorderableList(property.serializedObject, _propertyList);
            _reorderableList.onAddCallback += OnAdd;
            
            _reorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, PropertyUtility.GetLabel(property));
            };
            
            _reorderableList.drawElementCallback += OnDrawElement;

            _reorderableList.elementHeightCallback += OnElementHeight;

            _reorderableList.drawFooterCallback += OnDrawFooter;
            
            return _reorderableList;
        }

        private void OnDrawFooter(Rect rect)
        {

            Rect optionRect = new()
            {
                x = rect.x,
                y = rect.y,
                width = rect.width * 0.7f,
                height = LaborerGUIUtility.SingleLineHeight
            };

            _drawAsFoldout.boolValue = EditorGUI.Toggle(optionRect, "Draw Elements as foldout", _drawAsFoldout.boolValue);
            
            ReorderableList.defaultBehaviours.DrawFooter(rect, _reorderableList);
        }

        private float OnElementHeight(int index)
        {
            float height = LaborerGUIUtility.SingleLineHeight;
            
            SerializedProperty key = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
            SerializedProperty value = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Value");

            if (_drawAsFoldout.boolValue){
                if (!_foldoutStates.GetValueOrDefault(index, false)) return height;
            }
            
            height += Math.Max(EditorGUI.GetPropertyHeight(key), EditorGUI.GetPropertyHeight(value));
            
            return height;
        }

        private void OnDrawElement(Rect rect, int index, bool isactive, bool isfocused)
        {
            SerializedProperty key = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Key");
            SerializedProperty value = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative("Value");

            if (_drawAsFoldout.boolValue)
            {
                LaborerEditorGUI.DrawDictionaryElementAsFoldout(rect, key, value, index, ref _foldoutStates);
            } else
            {
                LaborerEditorGUI.DrawDictionaryElement(rect, key, value, index);
            }
        }
        
        private void OnAdd(ReorderableList list)
        {
            int length = _propertyList.arraySize;
            _propertyList.InsertArrayElementAtIndex(length);
        }
        
    }
}