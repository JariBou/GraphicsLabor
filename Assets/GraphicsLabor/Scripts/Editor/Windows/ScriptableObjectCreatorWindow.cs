using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility.GUI;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public sealed class ScriptableObjectCreatorWindow : EditorWindowBase
    {
        [SerializeField]private ScriptableObject _selectedScriptableObject;
        [SerializeField]private SerializedObject _serializedObject;
        private string _path;
        [SerializeField]private string _selectedPropTab = "";
        private string _selectedSoTab = "";
        private Vector2 _scrollPos;
        private float _totalDrawnHeight = 20f;
        private readonly Dictionary<string, ScriptableObject> _soNameAssetDic = new();

        private List<ScriptableObject> _possibleSos = new();

        private Vector2 _cachedTextSize = Vector2.zero;
        
        [MenuItem("Window/GraphicLabor/SO Creator")]
        public static void ShowWindow()
        {
            // _window = GetWindow<ScriptableObjectEditorWindow>();
            // _window.titleContent = new GUIContent("ScriptableObjectEditor");
            // _window._selectedScriptableObject = null;
            // _window.WindowName = "ScriptableObjectEditor";
            CreateNewEditorWindow<ScriptableObjectCreatorWindow>(null, "Scriptable Object Creator").GetPossibleScriptableObjects();
        }
        
        private SerializedObject GetSerializedObject()
        {
            return _serializedObject ??= new SerializedObject(_selectedScriptableObject);
        }

        private Vector2 GetCachedTextSize()
        {
            if (_cachedTextSize == Vector2.zero)
            {
                _cachedTextSize = GUI.skin.box.CalcSize(new GUIContent("No Manageable Scriptable Objects"));
            }

            return _cachedTextSize;
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

            if (GetPossibleScriptableObjects().Count == 0)
            {
                Rect middleText = currentRect;
                Vector2 textSize = GetCachedTextSize();
                middleText.x = middleText.width / 2 - textSize.x/2;
                middleText.y = position.height / 2 - textSize.y / 2;
                
                EditorGUI.LabelField(middleText, "No Manageable Scriptable Objects");
            }
            else
            {
                Rect selectionTabRect = currentRect;

                int tabSelectionWidth = 150;
                selectionTabRect.width = tabSelectionWidth - 10;
            
                currentRect.x = tabSelectionWidth;
                currentRect.width -= tabSelectionWidth;
            
                Rect scrollViewRect = currentRect;
                currentRect.width -= LaborerGUIUtility.ScrollBarWidth;
                Rect viewRect = currentRect;
                viewRect.height = _totalDrawnHeight;
                scrollViewRect.height = position.height;
                
                selectionTabRect.y += CreateSoSelectionButtons(selectionTabRect);
            
                // SO display
            
                _scrollPos = GUI.BeginScrollView(scrollViewRect, _scrollPos, viewRect, alwaysShowVertical:true, alwaysShowHorizontal:false);
            
                _totalDrawnHeight = DrawWithRect(currentRect);
                _totalDrawnHeight += LaborerGUIUtility.SingleLineHeight + LaborerGUIUtility.PropertyHeightSpacing;
            
                GUI.EndScrollView();
            }
            
        }

        private float CreateSoSelectionButtons(Rect selectionTabRect)
        {
            float yOffset = 0;

            for (int i = 0; i < _soNameAssetDic.Keys.Count; i++)
            {
                Rect buttonRect = new()
                {
                    x =selectionTabRect.x,
                    y = selectionTabRect.y + yOffset,
                    width = selectionTabRect.width,
                    height = LaborerGUIUtility.SingleLineHeight
                };
                
                string soName = _soNameAssetDic.Keys.ToArray()[i];
                
                GUI.SetNextControlName(soName);
                if (_selectedSoTab == soName)
                {
                    GUI.backgroundColor = LaborerGUIUtility.SelectedSoTabColor;
                }
                if (GUI.Button(buttonRect, new GUIContent(GetTruncatedTempSoName(soName), GetTempSoName(soName)) , EditorStyles.toolbarButton))
                {
                    SelectSo(soName);
                    GUI.FocusControl(soName);
                }

                GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;
                
                yOffset += LaborerGUIUtility.SoSelectionButtonHeight;
            }
            
            return yOffset;
        }

        private void SelectSo(string soName)
        {
            _selectedSoTab = soName;
            _selectedScriptableObject = _soNameAssetDic[soName];
            _serializedObject = new SerializedObject(_soNameAssetDic[soName]);
        }
        
        private static string GetTruncatedTempSoName(string soName)
        {
            string newSoName = GetTempSoName(soName);
            if (newSoName.Length > 22)
            {
                return $"{newSoName[..20].Trim()}...";
            }

            return newSoName;
        }

        private static string GetTempSoName(string soName)
        {
            string newSoName = ObjectNames.NicifyVariableName(soName);
            newSoName = newSoName.Remove(newSoName.Length - "_temp".Length);
            return newSoName;
        }

        protected override void PassInspectedObject(Object obj)
        {
            if (obj == null) return;

            string soTypeName = obj.GetType().Name;

            if (GetPossibleScriptableObjects().Exists(so => so.GetType().Name == soTypeName))
            {
                SelectSo(soTypeName);
            }
        }

        private void ButtonFunc()
        {
            ScriptableObject obj = _soNameAssetDic[_selectedSoTab];
            ScriptableObject so = CreateInstance(obj.GetType());
            so.name = so.GetType().Name;
            IOHelper.CreateFolder(GetSettings()._tempScriptableObjectsPath);
            IOHelper.CreateAssetIfNeeded(so, $"{GetSettings()._tempScriptableObjectsPath}/{so.name}_temp.asset");
            _soNameAssetDic[so.name] = so;
            SelectSo(so.name);
            GUI.FocusControl(null);
        }

        private float DrawWithRect(Rect currentRect)
        {
            GUI.backgroundColor = LaborerGUIUtility.BaseBackgroundColor;
            
            LaborerWindowGUI.DrawSoFieldAndButton(currentRect, _selectedScriptableObject, "Reset Values", ButtonFunc);
            currentRect.y += LaborerGUIUtility.SingleLineHeight;
            
            if (_selectedScriptableObject)
            {
                currentRect.y += DrawScriptableObjectWithRect(currentRect, _selectedScriptableObject);
            }
            
            if (GUI.Button(currentRect, "Save As"))
            {
                //String tempPath = EditorUtility.OpenFolderPanel("Save ScriptableObject at:", "", "");
                string tempPath =
                    EditorUtility.SaveFilePanelInProject("Save Asset", GetTempSoName(_selectedSoTab), "asset", "Enter the new SO Name");
                if (tempPath.StartsWith("Assets")) {
                    //GetSettings()._tempScriptableObjectsPath = tempPath;
                    ScriptableObject obj = Instantiate(_soNameAssetDic[_selectedSoTab]);
                    string objName = tempPath.Split('/')[^1].Replace(".asset", "");
                    obj.name = objName;
                    AssetDatabase.CreateAsset(obj, tempPath);
                    AssetDatabase.SaveAssets();   
                }
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

            SerializedObject serializedObject = GetSerializedObject();
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

        private List<ScriptableObject> GetPossibleScriptableObjects()
        {
            if (_possibleSos != null && _possibleSos.Count != 0)
            {
                foreach (ScriptableObject so in _possibleSos.Where(so => !_soNameAssetDic.ContainsKey(so.name)))
                {
                    _soNameAssetDic.Add(so.name, so);
                }
                return _possibleSos;
            }
            
            IEnumerable<Type> assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GetName().ToString().StartsWith("Unity"))
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(ManageableAttribute)) && !t.IsAbstract));

            IEnumerable<Type> types = assemblies as Type[] ?? assemblies.ToArray();
            
            _possibleSos = new List<ScriptableObject>(types.Count());
            List<Type> typesList = new List<Type>();
            List<string> soPaths = new List<string>();
            foreach (Type type in types)
            {
                if (!type.IsSubclassOf(typeof(ScriptableObject)) || typesList.Contains(type)) continue;
                
                ScriptableObject so = CreateInstance(type);
                so.name = so.GetType().Name;
                    
                IOHelper.CreateFolder(GetSettings()._tempScriptableObjectsPath);
                IOHelper.CreateAssetIfNeeded(so, $"{GetSettings()._tempScriptableObjectsPath}/{so.name}_temp.asset", false);
                    
                soPaths.Add($"{GetSettings()._tempScriptableObjectsPath}/{so.name}_temp.asset");
                _possibleSos.Add(so);
                typesList.Add(type);
                _soNameAssetDic.Add(so.name, so);
            }
            AssetDatabase.SaveAssets();
            
            // Remove unused SOs
            string[] folders = { GetSettings()._tempScriptableObjectsPath };
            IOHelper.DeleteAssets(folders, soPaths);

            if (_soNameAssetDic.Keys.Count == 0) return new List<ScriptableObject>();
            
            // Default select the first SO, should go somewhere else
            SelectSo(_soNameAssetDic.Keys.ToArray()[0]);

            return _possibleSos;
        }
        
    }
}