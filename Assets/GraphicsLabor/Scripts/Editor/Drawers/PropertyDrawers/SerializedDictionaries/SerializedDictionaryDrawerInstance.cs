using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility;
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
        private readonly SerializedProperty _property;
        private readonly SerializedProperty _propertyList;
        private readonly SerializedProperty _drawStyleProperty;
        private readonly GUIContent _cachedLabel;
        
        private Dictionary<int, bool> _foldoutStates = new ();


        public SerializedDictionaryDrawerInstance(SerializedProperty property)
        {
            _property = property;
            _propertyList = property.FindPropertyRelative(SerializedDictionary.SerializedListPropName);
            _drawStyleProperty = property.FindPropertyRelative(SerializedDictionary.DrawStylePropName);
            _cachedLabel = PropertyUtility.GetLabel(property);
        }
        
        public void DoGUI(Rect rect)
        {
            rect.height = LaborerGUIUtility.SingleLineHeight;
            EditorGUI.BeginProperty(rect, _cachedLabel, _property);
            
            GetReorderableList(_property).DoList(rect);
            
            EditorGUI.EndProperty();
            _property.serializedObject.ApplyModifiedProperties();
        }

        public float GetHeight()
        {
            return GetReorderableList(_property).GetHeight();
        }

        private ReorderableList GetReorderableList(SerializedProperty property)
        {
            if (_reorderableList != null) return _reorderableList;
            
            _reorderableList = new ReorderableList(property.serializedObject, _propertyList);
            
            _reorderableList.onAddCallback += OnAdd;

            _reorderableList.drawHeaderCallback = OnDrawHeader;
            
            _reorderableList.drawElementCallback += OnDrawElement;

            _reorderableList.drawElementBackgroundCallback += OnDrawElementBackground;
            
            _reorderableList.elementHeightCallback += OnElementHeight;

            _reorderableList.drawFooterCallback += OnDrawFooter;
            
            return _reorderableList;
        }

        private void OnDrawElementBackground(Rect rect, int index, bool isActive, bool isFocused)
        {
            Color prevColor = GUI.color;
            if (isFocused)
            {
                GUI.color = LaborerGUIUtility.DictionarySelectedElementBackgroundColor;
            }
            
            GUI.Box(rect, GUIContent.none);

            GUI.color = prevColor;
            // ReorderableList.defaultBehaviours.DrawElementBackground(rect, index, isActive, isFocused, true);
        }

        private void OnDrawHeader(Rect rect)
        {
            Rect labelRect = new()
            {
                x = rect.x,
                y = rect.y,
                width = rect.width*0.7f,
                height = LaborerGUIUtility.SingleLineHeight
            };
            
            EditorGUI.LabelField(labelRect, _cachedLabel);
            
            Rect optionRect = new()
            {
                x = rect.width*0.7f,
                y = rect.y,
                width = rect.width*0.25f,
                height = LaborerGUIUtility.SingleLineHeight
            };
            
            string tooltipText = "How to draw this Dictionary's elements";
            
            _drawStyleProperty.enumValueIndex = (int)LaborerEditorGUI.DrawEnumPopup(optionRect, (DictionaryDrawStyle)_drawStyleProperty.enumValueIndex, tooltipText);
        }

        private void OnDrawFooter(Rect rect)
        {
            ReorderableList.defaultBehaviours.DrawFooter(rect, _reorderableList);
        }

        private float OnElementHeight(int index)
        {
            float height = LaborerGUIUtility.SingleLineHeight;
            
            SerializedProperty key = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative(SerializedKeyValuePair.KeyPropName);
            SerializedProperty value = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative(SerializedKeyValuePair.ValuePropName);

            if (GEditorHelpers.IsEnumEqual(_drawStyleProperty, DictionaryDrawStyle.Foldout)){
                if (!_foldoutStates.GetValueOrDefault(index, false)) return height;
            }
            
            // if ((DictionaryDrawStyle)_drawStyleProperty.enumValueIndex == DictionaryDrawStyle.Foldout){
            //     if (!_foldoutStates.GetValueOrDefault(index, false)) return height;
            // }
            
            height += LaborerEditorGUI.GetDictionaryElementHeight(key, value);
            
            return height;
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty key = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative(SerializedKeyValuePair.KeyPropName);
            SerializedProperty value = _propertyList.GetArrayElementAtIndex(index).FindPropertyRelative(SerializedKeyValuePair.ValuePropName);

            if (!isFocused)
            {
                Color prevColor = GUI.color;
                GUI.color = index % 2 == 0
                    ? LaborerGUIUtility.EvenDictionaryElementBackgroundColor
                    : LaborerGUIUtility.OddDictionaryElementBackgroundColor;
                Rect boxRect = new()
                {
                    x = rect.x - LaborerGUIUtility.DictionaryHandleWidth,
                    y = rect.y,
                    width = rect.width + LaborerGUIUtility.DictionaryHandleWidth + LaborerGUIUtility.DictionaryElementTrailingWidth,
                    height = OnElementHeight(index) + LaborerGUIUtility.PropertyHeightSpacing
                };
                GUI.Box(boxRect, GUIContent.none);
                GUI.color = prevColor;
            }
            
            if (GEditorHelpers.IsEnumEqual(_drawStyleProperty, DictionaryDrawStyle.Foldout))
            {
                LaborerEditorGUI.DrawDictionaryElementAsFoldout(rect, key, value, index, ref _foldoutStates);
            } else
            {
                LaborerEditorGUI.DrawDictionaryElement(rect, key, value, index);
            }
            
            // if ((DictionaryDrawStyle)_drawAsFoldout.enumValueIndex == DictionaryDrawStyle.Foldout)
            // {
            //     LaborerEditorGUI.DrawDictionaryElementAsFoldout(rect, key, value, index, ref _foldoutStates);
            // } else
            // {
            //     LaborerEditorGUI.DrawDictionaryElement(rect, key, value, index);
            // }
        }
        
        private void OnAdd(ReorderableList list)
        {
            int length = _propertyList.arraySize;
            _propertyList.InsertArrayElementAtIndex(length);
        }
        
        
        
    }
}