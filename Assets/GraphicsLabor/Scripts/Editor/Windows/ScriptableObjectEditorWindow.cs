using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Editor.Utility;
using GraphicsLabor.Scripts.Editor.Windows.Utility;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public sealed class ScriptableObjectEditorWindow : EditorWindowBase
    {
        // Try to put it in a non static thing maybe, be cool not to have to open a new one every time
        private static ScriptableObject _baseScriptableObject;
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
            _baseScriptableObject = null;
        }
        
        public static void ShowWindow(Object obj)
        {
            if (obj == null || !obj.InheritsFrom(typeof(ScriptableObject)))
            {
                throw new ArgumentException($"Object of type {obj.GetType()} is not assignable to ScriptableObject");
            }
            
            _window = GetWindow<ScriptableObjectEditorWindow>();
            _window.titleContent = new GUIContent("ScriptableObjectEditor");
            _baseScriptableObject = (ScriptableObject)obj;
        }

        public override void OnGUI()
        {
            // Ok so actually we shouldn't use EditorGUILayout and Instead use EditorGUI if we want to be able to draw
            // like textures or properties fields.. Or maybe not?

            Rect currentRect = EditorGUILayout.GetControlRect();
            
            EditorGUI.LabelField(currentRect,"Hello!");
            currentRect.y += EditorGUIUtility.singleLineHeight;
            
            if (_baseScriptableObject)
            {
                using (new EditorGUI.DisabledScope(disabled: true))
                {
                    _baseScriptableObject = (ScriptableObject)EditorGUI.ObjectField(currentRect, "ScriptableObjectField",
                        _baseScriptableObject, typeof(ScriptableObject), false);
                }
                currentRect.y += EditorGUIUtility.singleLineHeight;

                currentRect = DrawScriptableObjectWithRect(currentRect ,_baseScriptableObject);
        
            }
            else
            {
                _baseScriptableObject = (ScriptableObject)EditorGUI.ObjectField(currentRect,"ScriptableObjectField",
                    _baseScriptableObject, typeof(ScriptableObject), false);
                currentRect.y += EditorGUIUtility.singleLineHeight;
            }
            
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                _path = EditorGUI.TextField(currentRect,"Path", _path);
                currentRect.y += EditorGUIUtility.singleLineHeight;
            }
            if (GUI.Button(currentRect, "GetPath"))
            {
                _path = EditorUtility.OpenFolderPanel("Save ScriptableObject at:", "", "");
            }
            currentRect.y += EditorGUIUtility.singleLineHeight;
            
            
        }
        
        
        // Does not fix [Expandable] ScriptableObject drawing problem
        private Rect DrawScriptableObjectWithRect(Rect startRect, ScriptableObject scriptableObject)
        {
            if (!scriptableObject) return startRect;

            Rect boxRect = new()
            {
                x = 0.0f,
                y = startRect.y + EditorGUIUtility.singleLineHeight,
                width = startRect.width * 2.0f,
                height = startRect.height - EditorGUIUtility.singleLineHeight
            };

            GUI.Box(boxRect, GUIContent.none);
            
            Dictionary<string, List<SerializedProperty>> tabbedProperties = new Dictionary<string, List<SerializedProperty>>();

            using (new EditorGUI.IndentLevelScope())
            {
                SerializedObject serializedObject = new(scriptableObject);
                serializedObject.Update();
                float yOffset = 0;
                
                using (var iterator = serializedObject.GetIterator())
                {
                    if (iterator.NextVisible(true))
                    {
                        do
                        {
                            SerializedProperty serializedProperty = serializedObject.FindProperty(iterator.name);
                            
                            if (serializedProperty.name.Equals("m_Script", StringComparison.Ordinal)) continue;

                            bool visible = PropertyUtility.IsVisible(serializedProperty);
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
                                yOffset += childHeight;
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
                        height = EditorGUIUtility.singleLineHeight
                    };
                    if (GUI.Button(buttonRect, keyValuePair.Key, EditorStyles.toolbarButton))
                    {
                        _selectedTab = keyValuePair.Key == _selectedTab ? "" : keyValuePair.Key;
                    }

                    i++;
                }
                
                yOffset += EditorGUIUtility.singleLineHeight;


                if (tabbedProperties.TryGetValue(_selectedTab, out var properties))
                {
                    float yOffsetSave = yOffset;
                    Rect bgRect = new()
                    {
                        x = 0.0f,
                        y = startRect.y + yOffsetSave,
                        width = startRect.width * 2.0f,
                        height = yOffset + LaborerWindowGUI.GetPropertiesHeight(properties) - yOffsetSave
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
                            yOffset += childHeight;
                        }
                    }
                    
                    
                }
                
                serializedObject.ApplyModifiedProperties();
                return new Rect()
                {
                    x = startRect.x,
                    y = startRect.y + yOffset,
                    height = startRect.height,
                    width = startRect.width
                };
            }
        }
    }
}