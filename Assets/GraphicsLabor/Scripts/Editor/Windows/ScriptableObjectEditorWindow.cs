﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public sealed class ScriptableObjectEditorWindow : EditorWindowBase
    {
        // Try to put it in a non static thing maybe, be cool not to have to open a new one every time
        private ScriptableObject _selectedScriptableObject;
        private string _path;
        private string _selectedTab = "";
        
        [MenuItem("Window/GraphicLabor/Test Window")]
        public static void ShowWindow()
        {
            // _window = GetWindow<ScriptableObjectEditorWindow>();
            // _window.titleContent = new GUIContent("ScriptableObjectEditor");
            // _window._selectedScriptableObject = null;
            // _window.WindowName = "ScriptableObjectEditor";
            CreateNewEditorWindow<ScriptableObjectEditorWindow>(null, "ScriptableObjectEditor");
        }
        

        
        public static void ShowWindow(Object obj)
        {
            if (obj == null || !obj.InheritsFrom(typeof(ScriptableObject)))
            {
                CreateNewEditorWindow<ScriptableObjectEditorWindow>(null, "ScriptableObjectEditor");
                GLogger.LogWarning($"Object of type {obj.GetType()} is not assignable to ScriptableObject");
            }
            else
            {
                CreateNewEditorWindow<ScriptableObjectEditorWindow>(obj, "ScriptableObjectEditor");
            }
            
            // _window = GetWindow<ScriptableObjectEditorWindow>();
            // _window.titleContent = new GUIContent("ScriptableObjectEditor");
            // _window._selectedScriptableObject = (ScriptableObject)obj;
            // _window.WindowName = obj.name;
        }

        protected override void OnGUI()
        {
            DrawWithRect();
        }

        protected override void PassInspectedObject(Object obj)
        {
            _selectedScriptableObject = (ScriptableObject)obj;
            WindowName = obj != null ? obj.name : "null";
        }

        private void DrawWithRect()
        {
            GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;

            Rect currentRect = EditorGUILayout.GetControlRect();
            
            EditorGUI.LabelField(currentRect,"Hello!");
            currentRect.y += LaborerGUIUtility.SingleLineHeight;


            // For now dont allow change of SO if set
            using (new EditorGUI.DisabledScope(disabled: _selectedScriptableObject))
            {
                EditorGUI.BeginChangeCheck();
                _selectedScriptableObject = (ScriptableObject)EditorGUI.ObjectField(currentRect, "ScriptableObjectField",
                    _selectedScriptableObject, typeof(ScriptableObject), false);
                if (EditorGUI.EndChangeCheck())
                {
                    PassInspectedObject(_selectedScriptableObject);
                }
                currentRect.y += LaborerGUIUtility.SingleLineHeight;
            }

            if (_selectedScriptableObject)
            {
                currentRect.y += DrawScriptableObjectWithRect(currentRect ,_selectedScriptableObject);
            }
            
            //TODO: Create SO Creator
            
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                _path = EditorGUI.TextField(currentRect,"Path", _path);
                currentRect.y += LaborerGUIUtility.SingleLineHeight;
            }
            if (GUI.Button(currentRect, "GetPath"))
            {
                _path = EditorUtility.OpenFolderPanel("Save ScriptableObject at:", "", "");
            }
            currentRect.y += LaborerGUIUtility.SingleLineHeight;
        }
        
        // Does not fix [Expandable] ScriptableObject drawing problem
        private float DrawScriptableObjectWithRect(Rect startRect, ScriptableObject scriptableObject)
        {
            if (!scriptableObject) return 0f;
            Dictionary<string, List<SerializedProperty>> tabbedSerializedProperties = new Dictionary<string, List<SerializedProperty>>();
            Dictionary<string, List<PropertyInfo>> tabbedProperties = new Dictionary<string, List<PropertyInfo>>();

            using (new EditorGUI.IndentLevelScope())
            {
                SerializedObject serializedObject = new(scriptableObject);
                serializedObject.Update();
                float yOffset = LaborerGUIUtility.PropertyHeightSpacing;

                yOffset += LaborerWindowGUI.DrawScriptableObjectNormalSerializedFields(startRect, yOffset, serializedObject, ref tabbedSerializedProperties);
                yOffset += LaborerWindowGUI.DrawScriptableObjectNormalProperties(startRect, yOffset, serializedObject, ref tabbedProperties);

                IEnumerable<string> tabs = GHelpers.ConcatenateLists(tabbedSerializedProperties.Keys, tabbedProperties.Keys).ToArray();
                float buttonWidth = startRect.width / tabs.Count();
                int i = 0;
                foreach (string key in tabs)
                {
                    Rect buttonRect = new()
                    {
                        x =startRect.x + buttonWidth * i,
                        y = startRect.y + yOffset,
                        width = buttonWidth,
                        height = LaborerGUIUtility.SingleLineHeight
                    };
                    if (key == _selectedTab)
                    {
                        GUI.backgroundColor = LaborerGUIUtility.SelectedTabColor;
                    } 
                    if (GUI.Button(buttonRect, key, EditorStyles.toolbarButton))
                    {
                        _selectedTab = key == _selectedTab ? "" : key;
                    }

                    GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;
                    
                    i++;
                }
                yOffset += LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing*2;
                
                EditorGUI.BeginChangeCheck();

                if (tabbedSerializedProperties.TryGetValue(_selectedTab, out var serializedProperties))
                {
                    // TODO: When changing values inside of serialized classes it refolds and sometimes doesn't register
                    // Ok so what happens is that when repainting it puts the foldouts in the same state as the SO
                    yOffset += LaborerWindowGUI.DrawScriptableObjectTabbedSerializedFields(startRect, yOffset, serializedProperties);
                }
                if (tabbedProperties.TryGetValue(_selectedTab, out var normalProperties))
                {
                    yOffset += LaborerWindowGUI.DrawScriptableObjectTabbedProperties(startRect, yOffset, serializedObject, normalProperties);
                }
                yOffset += LaborerGUIUtility.PropertyHeightSpacing;
                
                serializedObject.ApplyModifiedProperties();
                return yOffset;
            }
        }
    }
}