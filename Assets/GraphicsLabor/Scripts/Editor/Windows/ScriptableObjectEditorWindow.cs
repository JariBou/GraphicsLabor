using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
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
        private Color _baseBgColor;
        
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
            GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;

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

        private IEnumerable<string> ConcatenateLists(IEnumerable<string> listA, IEnumerable<string> listB,
            bool allowDuplicates = false)
        {
            List<string> concatenatedList = new List<string>();
            concatenatedList.AddRange(listA);

            if (allowDuplicates)
            {
                concatenatedList.AddRange(listB);
            }
            else
            {
                foreach (string s in listB)
                {
                    if (concatenatedList.Contains(s)) continue;
                    
                    concatenatedList.Add(s);
                }
            }

            return concatenatedList;
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

                yOffset += DrawScriptableObjectNormalSerializedFields(startRect, yOffset, serializedObject, ref tabbedSerializedProperties);
                yOffset += DrawScriptableObjectNormalProperties(startRect, yOffset, serializedObject, ref tabbedProperties);

                IEnumerable<string> tabs = ConcatenateLists(tabbedSerializedProperties.Keys, tabbedProperties.Keys).ToArray();
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
                

                if (tabbedSerializedProperties.TryGetValue(_selectedTab, out var serializedProperties))
                {
                    yOffset += DrawScriptableObjectTabbedSerializedFields(startRect, yOffset, serializedProperties);
                }
                if (tabbedProperties.TryGetValue(_selectedTab, out var normalProperties))
                {
                    yOffset += DrawScriptableObjectTabbedProperties(startRect, yOffset, serializedObject, normalProperties);
                }

                yOffset += LaborerGUIUtility.PropertyHeightSpacing;
                serializedObject.ApplyModifiedProperties();
                return yOffset;
            }
        }

        private float DrawScriptableObjectNormalSerializedFields(Rect startRect, float yOffset, SerializedObject serializedObject, ref Dictionary<string, List<SerializedProperty>> tabbedSerializedProperties)
        {
            float localOffset = 0;
            using SerializedProperty iterator = serializedObject.GetIterator();
            if (!iterator.NextVisible(true)) return localOffset;
            
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
                    y = startRect.y + yOffset + localOffset,
                    width = startRect.width,
                    height = childHeight
                };
                if (LaborerWindowGUI.DrawSerializedProperty(childRect, serializedProperty, ref tabbedSerializedProperties, true))
                {
                    localOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }

                //LaborerEditorGUI.PropertyField(childRect, childProperty, true);

            } while (iterator.NextVisible(false));

            return localOffset;
        }
        
        private float DrawScriptableObjectTabbedSerializedFields(Rect startRect, float yOffset, List<SerializedProperty> properties)
        {
            float localOffset = 0;
            
            using (new EditorGUI.IndentLevelScope())
            {
                Rect bgRect = new()
                {
                    x = 0.0f,
                    y = startRect.y + yOffset + localOffset - LaborerGUIUtility.PropertyHeightSpacing,
                    width = startRect.width * 2.0f,
                    height = LaborerWindowGUI.GetPropertiesHeight(properties, LaborerGUIUtility.PropertyHeightSpacing)
                };
                
                GUI.Box(bgRect, GUIContent.none);

                foreach (SerializedProperty property in properties)
                {
                    float childHeight = LaborerWindowGUI.GetPropertyHeight(property);
                    Rect childRect = new()
                    {
                        x = startRect.x,
                        y = startRect.y + yOffset + localOffset,
                        width = startRect.width,
                        height = childHeight
                    };
                    LaborerEditorGUI.PropertyField(childRect, property, true);
                    
                    localOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }
            }
            
            return localOffset + LaborerGUIUtility.PropertyHeightSpacing;
        }

        private float DrawScriptableObjectNormalProperties(Rect startRect, float yOffset, SerializedObject serializedObject, ref Dictionary<string, List<PropertyInfo>> tabbedProperties)
        {
            float localOffset = 0;
            IEnumerable<PropertyInfo> properties = ReflectionUtility.GetAllProperties(serializedObject.targetObject, p => p.GetCustomAttributes(typeof(ShowPropertyAttribute), true).Length > 0);

            IEnumerable<PropertyInfo> propertyInfos = properties as PropertyInfo[] ?? properties.ToArray();
            if (!propertyInfos.Any())
            {
                Debug.Log("No properties");
                return localOffset;
            }

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Rect newRect = new()
                {
                    x = startRect.x,
                    y = startRect.y + yOffset + localOffset,
                    width = startRect.width,
                    height = LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing
                };
                if (LaborerWindowGUI.DrawProperty(newRect, propertyInfo, serializedObject.targetObject, ref tabbedProperties, true))
                {
                    Debug.Log($"Drawing Property {propertyInfo.Name}");
                    localOffset += LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }
            }

            return localOffset;

        }
        
        private float DrawScriptableObjectTabbedProperties(Rect startRect, float yOffset, SerializedObject serializedObject, List<PropertyInfo> properties)
        {
            float localOffset = 0;
            
            using (new EditorGUI.IndentLevelScope())
            {
                Rect bgRect = new()
                {
                    x = 0.0f,
                    y = startRect.y + yOffset + localOffset - LaborerGUIUtility.PropertyHeightSpacing,
                    width = startRect.width * 2.0f,
                    height = (LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing) * properties.Count + LaborerGUIUtility.PropertyHeightSpacing
                };

                GUI.Box(bgRect, GUIContent.none);

                foreach (PropertyInfo property in properties)
                {
                    float childHeight = LaborerGUIUtility.SingleLineHeight;
                    Rect childRect = new()
                    {
                        x = startRect.x,
                        y = startRect.y + yOffset + localOffset,
                        width = startRect.width,
                        height = childHeight
                    };
                    LaborerWindowGUI.DrawProperty(childRect, property, serializedObject.targetObject);
                    
                    localOffset += childHeight + LaborerGUIUtility.PropertyHeightSpacing;
                }
            }
            
            return localOffset + LaborerGUIUtility.PropertyHeightSpacing;
        }
    }
}