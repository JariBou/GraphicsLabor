using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public class GraphicsLaborSettingsWindow : EditorWindowBase
    {
        private ScriptableObject _selectedScriptableObject;
        private SerializedObject _serializedObject;
        private string _selectedTab = "";

        // [MenuItem("Window/GraphicLabor/Settings")]
        // public static void ShowWindow()
        // {
        //     WindowBase.CreateAndInitWindow<GraphicsLaborSettingsWindow>("GL Settings");
        // }
        [MenuItem("Window/GraphicLabor/SettingsOLD")]
        public static void ShowSettings()
        {
            CreateNewEditorWindow<GraphicsLaborSettingsWindow>(GetSettings(), "GL Settings");

        }
        protected override void OnGUI()
        {
            DrawWithRect();
        }

        private void DrawWithRect()
        {
            GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;

            Rect currentRect = EditorGUILayout.GetControlRect();

            // For now dont allow change of SO if set
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                EditorGUI.ObjectField(currentRect, "ScriptableObjectField", GetSettings(), typeof(ScriptableObject), false);
                currentRect.y += LaborerGUIUtility.SingleLineHeight;
            }

            // foreach (FieldInfo field in ReflectionUtility.GetAllFields(GetSettings(), info => true))
            // {
            //     LaborerEditorGUI.DrawField(field.GetValue(GetSettings()), field.Name);
            // }
            currentRect.y += DrawScriptableObjectWithRect(currentRect ,GetSettings());
            
            if (GUI.Button(currentRect, "Save As"))
            {
                //String tempPath = EditorUtility.OpenFolderPanel("Save ScriptableObject at:", "", "");
                String tempPath =
                    EditorUtility.OpenFolderPanel("Select Path", "Assets", "");
                if (tempPath != null)
                {
                    tempPath = Path.GetRelativePath(Application.dataPath, tempPath);
                    Debug.LogWarning(tempPath);
                    if (tempPath.StartsWith(".."))
                    {
                        // Needs to be in Assets Folder
                    } else
                    {
                        tempPath = "Assets\\" + tempPath;
                        GetSettings()._tempScriptableObjectsPath = tempPath;
                    }
                }
                
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
                

                if (tabbedSerializedProperties.TryGetValue(_selectedTab, out var serializedProperties))
                {
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

        protected override void PassInspectedObject(Object obj)
        {
            WindowName = obj != null ? obj.name : "null";
        }
    }
}