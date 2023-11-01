using System;
using System.Collections.Generic;
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
        private static ScriptableObjectEditorWindow _window;
        private Texture2D _texture2D;
        private bool _foldoutState;
        private string _path;
        private string _selectedTab = "";
        
        [MenuItem("Window/GraphicLabor/Test Window")]
        public static void ShowWindow()
        {
            _window = GetWindow<ScriptableObjectEditorWindow>();
            _window.titleContent = new GUIContent("ScriptableObjectEditor");
            _window._selectedScriptableObject = null;
            _window.WindowName = "ScriptableObjectEditor";
        }
        
        public static void ShowWindow(Object obj)
        {
            if (obj == null || !obj.InheritsFrom(typeof(ScriptableObject)))
            {
                throw new ArgumentException($"Object of type {obj.GetType()} is not assignable to ScriptableObject");
            }
            
            // _window = GetWindow<ScriptableObjectEditorWindow>();
            // _window.titleContent = new GUIContent("ScriptableObjectEditor");
            // _window._selectedScriptableObject = (ScriptableObject)obj;
            // _window.WindowName = obj.name;
            
            _window = CreateNewEditorWindow<ScriptableObjectEditorWindow>(obj, "ScriptableObjectEditor");
            
        }

        public override void OnGUI()
        {
            DrawWithRect();
        }

        protected override void PassInspectedObject(Object obj)
        {
            _selectedScriptableObject = (ScriptableObject)obj;
        }

        private void DrawWithRect()
        {
            Rect currentRect = EditorGUILayout.GetControlRect();
            
            EditorGUI.LabelField(currentRect,"Hello!");
            currentRect.y += LaborerGUIUtility.SingleLineHeight;


            // For now dont allow change of SO if set
            using (new EditorGUI.DisabledScope(disabled: _selectedScriptableObject))
            {
                _selectedScriptableObject = (ScriptableObject)EditorGUI.ObjectField(currentRect, "ScriptableObjectField",
                    _selectedScriptableObject, typeof(ScriptableObject), false);
                currentRect.y += LaborerGUIUtility.SingleLineHeight;
            }

            if (_selectedScriptableObject)
            {
                currentRect.y += DrawScriptableObjectWithRect(currentRect ,_selectedScriptableObject);
            }
            
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

        private void DrawWithLayout()
        {
            EditorGUILayout.LabelField("Hello!");
            
            if (_selectedScriptableObject)
            {
                using (new EditorGUI.DisabledScope(disabled: true))
                {
                    _selectedScriptableObject = (ScriptableObject)EditorGUILayout.ObjectField("ScriptableObjectField",
                        _selectedScriptableObject, typeof(ScriptableObject), false);
                }
                
                LaborerWindowGUI.DrawEditableScriptableObject(_selectedScriptableObject, ref _selectedTab);
        
            }
            else
            {
                _selectedScriptableObject = (ScriptableObject)EditorGUILayout.ObjectField("ScriptableObjectField",
                    _selectedScriptableObject, typeof(ScriptableObject), false);
            }
            
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                _path = EditorGUILayout.TextField("Path", _path);
            }
            if (GUILayout.Button( "GetPath"))
            {
                _path = EditorUtility.OpenFolderPanel("Save ScriptableObject at:", "", "");
            }
        }
        
        // Does not fix [Expandable] ScriptableObject drawing problem
        private float DrawScriptableObjectWithRect(Rect startRect, ScriptableObject scriptableObject)
        {
            if (!scriptableObject) return 0f;

            Rect boxRect = new()
            {
                x = 0.0f,
                y = startRect.y + LaborerGUIUtility.SingleLineHeight,
                width = startRect.width * 2.0f,
                height = startRect.height - LaborerGUIUtility.SingleLineHeight
            };

            GUI.Box(boxRect, GUIContent.none);
            
            Dictionary<string, List<SerializedProperty>> tabbedProperties = new Dictionary<string, List<SerializedProperty>>();

            using (new EditorGUI.IndentLevelScope())
            {
                SerializedObject serializedObject = new(scriptableObject);
                serializedObject.Update();
                float yOffset = 0;
                
                using (SerializedProperty iterator = serializedObject.GetIterator())
                {
                    if (iterator.NextVisible(true))
                    {
                        do
                        {
                            SerializedProperty serializedProperty = serializedObject.FindProperty(iterator.name);
                            
                            if (serializedProperty.name.Equals("m_Script", StringComparison.Ordinal)) continue;

                            bool visible = PropertyUtility.IsVisible(serializedProperty) && PropertyUtility.IsEnabled(serializedProperty);
                            if (!visible) continue;
                            
                            float childHeight = LaborerWindowGUI.GetPropertyHeight(serializedProperty);
                            Rect childRect = new()
                            {
                                x = startRect.x,
                                y = startRect.y + yOffset,
                                width = startRect.width,
                                height = childHeight
                            };
                            if (LaborerWindowGUI.DrawProperty(childRect, serializedProperty, ref tabbedProperties, true))
                            {
                                yOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                            }

                            //LaborerEditorGUI.PropertyField(childRect, childProperty, true);

                        } while (iterator.NextVisible(false));
                    }
                }

                float buttonWidth = startRect.width / tabbedProperties.Count;
                int i = 0;
                foreach (KeyValuePair<string,List<SerializedProperty>> keyValuePair in tabbedProperties)
                {
                    Rect buttonRect = new()
                    {
                        x =startRect.x + buttonWidth * i,
                        y = startRect.y + yOffset,
                        width = buttonWidth,
                        height = LaborerGUIUtility.SingleLineHeight
                    };
                    if (GUI.Button(buttonRect, keyValuePair.Key, EditorStyles.toolbarButton))
                    {
                        _selectedTab = keyValuePair.Key == _selectedTab ? "" : keyValuePair.Key;
                    }

                    i++;
                }
                yOffset += LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing*2;
                

                if (tabbedProperties.TryGetValue(_selectedTab, out var properties))
                {
                    using (new EditorGUI.IndentLevelScope())
                    {
                        Rect bgRect = new()
                        {
                            x = 0.0f,
                            y = startRect.y + yOffset - LaborerGUIUtility.PropertyHeightSpacing,
                            width = startRect.width,
                            height = LaborerWindowGUI.GetPropertiesHeight(properties, LaborerGUIUtility.PropertyHeightSpacing)
                        };
                        GUI.Box(bgRect, GUIContent.none);

                        foreach (SerializedProperty property in properties)
                        {
                            float childHeight = LaborerWindowGUI.GetPropertyHeight(property);
                            Rect childRect = new()
                            {
                                x = startRect.x,
                                y = startRect.y + yOffset,
                                width = startRect.width,
                                height = childHeight
                            };
                            if (LaborerWindowGUI.DrawProperty(childRect, property, ref tabbedProperties, false))
                            {
                                yOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                            }
                        }
                    }
                }
                
                serializedObject.ApplyModifiedProperties();
                return yOffset;
            }
        }
    }
}