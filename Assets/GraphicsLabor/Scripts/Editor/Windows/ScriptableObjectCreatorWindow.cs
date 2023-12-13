using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using String = System.String;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public sealed class ScriptableObjectCreatorWindow : EditorWindowBase
    {
        // Try to put it in a non static thing maybe, be cool not to have to open a new one every time
        private ScriptableObject _selectedScriptableObject;
        private SerializedObject _serializedObject;
        private string _path;
        private string _selectedPropTab = "";
        private string _selectedSoTab = "";
        private Vector2 _scrollPos;
        private float _totalDrawnHeight = 20f;
        private Dictionary<String, ScriptableObject> SoNameAssetDic = new();

        private List<ScriptableObject> _possibleSos;
        
        [MenuItem("Window/GraphicLabor/Test Creator Window")]
        public static void ShowWindow()
        {
            // _window = GetWindow<ScriptableObjectEditorWindow>();
            // _window.titleContent = new GUIContent("ScriptableObjectEditor");
            // _window._selectedScriptableObject = null;
            // _window.WindowName = "ScriptableObjectEditor";
            CreateNewEditorWindow<ScriptableObjectCreatorWindow>(null, "Scriptable Object Creator").GetPossibleScriptableObjects();
        }
        
        public static void ShowWindow(Object obj)
        {
            if (obj == null || !obj.InheritsFrom(typeof(ScriptableObject)))
            {
                CreateNewEditorWindow<ScriptableObjectCreatorWindow>(null, "Scriptable Object Creator").GetPossibleScriptableObjects();
                GLogger.LogWarning($"Object of type {obj.GetType()} is not assignable to ScriptableObject");
            }
            else
            {
                CreateNewEditorWindow<ScriptableObjectCreatorWindow>(obj, "Scriptable Object Creator").GetPossibleScriptableObjects();
            }
            
            // _window = GetWindow<ScriptableObjectEditorWindow>();
            // _window.titleContent = new GUIContent("ScriptableObjectEditor");
            // _window._selectedScriptableObject = (ScriptableObject)obj;
            // _window.WindowName = obj.name;
        }

        protected override void OnGUI()
        {
            Rect currentRect = EditorGUILayout.GetControlRect();

            Rect selectionTabRect = currentRect;

            int tabSelectionWidth = 150;
            selectionTabRect.width = tabSelectionWidth - 10;
            
            currentRect.x = tabSelectionWidth;
            currentRect.width -= tabSelectionWidth;
            
            Rect rect2 = currentRect;
            currentRect.width -= LaborerGUIUtility.ScrollBarWidth;
            Rect rect = currentRect;
            rect.height = _totalDrawnHeight;
            rect2.height = position.height;
            
            // SO type selector part
            GUI.Box(selectionTabRect, GUIContent.none);
            selectionTabRect.x += 3;
            EditorGUI.LabelField(selectionTabRect, "surprise");

            selectionTabRect.y += CreateSoSelectionButtons(selectionTabRect);
            
            // SO display
            
            _scrollPos = GUI.BeginScrollView(rect2, _scrollPos, rect, alwaysShowVertical:true, alwaysShowHorizontal:false);
            
            _totalDrawnHeight = DrawWithRect(currentRect);
            _totalDrawnHeight += LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing;
            
            GUI.EndScrollView();
        }

        private float CreateSoSelectionButtons(Rect selectionTabRect)
        {
            float yOffset = 0;

            for (int i = 0; i < SoNameAssetDic.Keys.Count; i++)
            {
                Rect buttonRect = new()
                {
                    x =selectionTabRect.x,
                    y = selectionTabRect.y + yOffset,
                    width = selectionTabRect.width,
                    height = LaborerGUIUtility.SingleLineHeight
                };
                
                String soName = SoNameAssetDic.Keys.ToArray()[i];
                
                if (GUI.Button(buttonRect, new GUIContent(GetTruncatedTempSoName(soName), GetTempSoName(soName)) , EditorStyles.toolbarButton))
                {
                    SelectSo(soName);
                }
                yOffset += LaborerGUIUtility.SoSelectionButtonHeight;
            }
            
            
            return yOffset;
        }

        private void SelectSo(string soName)
        {
            _selectedSoTab = soName;
            _selectedScriptableObject = SoNameAssetDic[soName];
            _serializedObject = new SerializedObject(SoNameAssetDic[soName]);
        }
        
        private String GetTruncatedTempSoName(String soName)
        {
            String newSoName = GetTempSoName(soName);
            if (newSoName.Length > 22)
            {
                return $"{newSoName.Substring(0, 20).Trim()}...";
            }

            return newSoName;
        }

        private String GetTempSoName(String soName)
        {
            String newSoName = ObjectNames.NicifyVariableName(soName);
            newSoName = newSoName.Remove(newSoName.Length - "_temp".Length);
            return newSoName;
        }

        protected override void PassInspectedObject(Object obj)
        {
            _selectedScriptableObject = (ScriptableObject)obj;
            WindowName = obj != null ? obj.name : "null";
            //_serializedObject = new SerializedObject(_selectedScriptableObject);
        }

        private float DrawWithRect(Rect currentRect)
        {
            GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;

            // Rect currentRect = EditorGUILayout.GetControlRect();
            
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
                currentRect.y += DrawScriptableObjectWithRect(currentRect, _selectedScriptableObject);
            }
            
            //TODO: Create SO Creator
            
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                EditorGUI.TextField(currentRect,"Path", GetSettings()._tempScriptableObjectsPath);
                currentRect.y += LaborerGUIUtility.SingleLineHeight;
            }
            if (GUI.Button(currentRect, "SaveAt"))
            {
                //String tempPath = EditorUtility.OpenFolderPanel("Save ScriptableObject at:", "", "");
                String tempPath =
                    EditorUtility.SaveFilePanelInProject("Save Asset", GetTempSoName(_selectedSoTab), "asset", "Enter the new SO Name");
                Debug.LogWarning(tempPath);
                Debug.LogWarning(tempPath == null); // False
                Debug.LogWarning(tempPath == ""); // True
                if (tempPath.StartsWith("Assets")) {
                    Debug.Log("we in");
                    GetSettings()._tempScriptableObjectsPath = tempPath;
                    ScriptableObject obj = SoNameAssetDic[_selectedSoTab];
                    string objName = tempPath.Split('/')[^1].Replace(".asset", "");
                    obj.name = objName;
                    AssetDatabase.CreateAsset(Instantiate(obj), tempPath);
                }
            }
            currentRect.y += LaborerGUIUtility.SingleLineHeight;

            if (GUI.Button(currentRect, "Test"))
            {
                GetPossibleScriptableObjects();
            }
            currentRect.y += LaborerGUIUtility.SingleLineHeight;

            return currentRect.y;
        }
        
        // Does not fix [Expandable] ScriptableObject drawing problem
        private float DrawScriptableObjectWithRect(Rect startRect, ScriptableObject scriptableObject)
        {
            if (!scriptableObject) return 0f;
            Dictionary<string, List<SerializedProperty>> tabbedSerializedProperties = new Dictionary<string, List<SerializedProperty>>();
            Dictionary<string, List<PropertyInfo>> tabbedProperties = new Dictionary<string, List<PropertyInfo>>();

            using (new EditorGUI.IndentLevelScope())
            {
                SerializedObject serializedObject = _serializedObject;
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
                    if (key == _selectedPropTab)
                    {
                        GUI.backgroundColor = LaborerGUIUtility.SelectedTabColor;
                    } 
                    if (GUI.Button(buttonRect, key, EditorStyles.toolbarButton))
                    {
                        _selectedPropTab = key == _selectedPropTab ? "" : key;
                    }

                    GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;
                    
                    i++;
                }
                yOffset += LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing*2;

                if (tabbedSerializedProperties.TryGetValue(_selectedPropTab, out var serializedProperties))
                {
                    // TODO: When changing values inside of serialized classes it refolds and sometimes doesn't register
                    // Ok so what happens is that when repainting it puts the foldouts in the same state as the SO
                    yOffset += LaborerWindowGUI.DrawScriptableObjectTabbedSerializedFields(startRect, yOffset, serializedProperties);
                }
                if (tabbedProperties.TryGetValue(_selectedPropTab, out var normalProperties))
                {
                    yOffset += LaborerWindowGUI.DrawScriptableObjectTabbedProperties(startRect, yOffset, serializedObject, normalProperties);
                }
                yOffset += LaborerGUIUtility.PropertyHeightSpacing;
                
                serializedObject.ApplyModifiedProperties();
                return yOffset;
            }
        }

        private List<ScriptableObject> GetPossibleScriptableObjects()
        {
            if (_possibleSos != null) return _possibleSos;
            IEnumerable<Type> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GetName().ToString().StartsWith("Unity"))
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(EditableAttribute)) && !t.IsAbstract));
// TODO: actually put the assemblies in the LaborSettings
//      hum ok so we cannot reference assembly c-sharp, need to find another way
            var test = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GetName().ToString().StartsWith("Unity") && !assembly.GetName().ToString().StartsWith("System")
                && !assembly.GetName().ToString().StartsWith("JetBrains") && !assembly.GetName().ToString().StartsWith("unity")
                && !assembly.GetName().ToString().StartsWith("Bee") && !assembly.GetName().ToString().StartsWith("Mono")
                && !assembly.GetName().ToString().StartsWith("mscorlib") && !assembly.GetName().ToString().StartsWith("netstandard")
                && !assembly.GetName().ToString().StartsWith("Psd") && !assembly.GetName().ToString().StartsWith("log4net")
                && !assembly.GetName().ToString().StartsWith("ExCSS") && !assembly.GetName().ToString().StartsWith("PPv2")
                && !assembly.GetName().ToString().StartsWith("nunit"));
            
            var test2 = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => assembly.GetName().ToString().StartsWith("Assembly-CSharp"));
            // foreach (Assembly type in test2)
            // {
            //     Debug.Log(type.ToString());
            // }
            var list = new List<ScriptableObject>();
            foreach (Type type in assemblies)
            {
                if (type.IsSubclassOf(typeof(ScriptableObject)))
                {
                    ScriptableObject so = CreateInstance(type);
                    so.name = so.GetType().Name;
                    GetSettings();
                    AssetHandler.CreateFolder(GetSettings()._tempScriptableObjectsPath);
                    AssetDatabase.CreateAsset(so, $"{GetSettings()._tempScriptableObjectsPath}/{so.name}_temp.asset");
                    list.Add(so);
                    SoNameAssetDic.Add(so.name, so);
                }
            }
            AssetDatabase.SaveAssets();
            
            
            foreach (ScriptableObject type in list)
            {
                Debug.Log(type.name);
            }
            
            SelectSo(SoNameAssetDic.Keys.ToArray()[0]);

            return list;
        }
        
    }
}