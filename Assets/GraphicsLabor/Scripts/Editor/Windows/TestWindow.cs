using System;
using GraphicsLabor.Scripts.Editor.Utility;
using GraphicsLabor.Scripts.Editor.Windows.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public sealed class TestWindow : EditorWindowBase
    {
        private static ScriptableObject _baseScriptableObject;
        private Texture2D _texture2D;
        private bool _foldoutState;
        private string _path;
        
        [MenuItem("Window/GraphicLabor/Test Window")]
        public static void ShowWindow()
        {
            TestWindow window = GetWindow<TestWindow>();
            window.titleContent = new GUIContent("windowName");
            _baseScriptableObject = null;
        }
        
        public static void ShowWindow(Object obj)
        {
            if (obj == null || !obj.InheritsFrom(typeof(ScriptableObject)))
            {
                throw new ArgumentException($"Object of type {obj.GetType()} is not assignable to ScriptableObject");
            }
            
            TestWindow window = GetWindow<TestWindow>();
            window.titleContent = new GUIContent("windowName");
            _baseScriptableObject = (ScriptableObject)obj;
        }

        public override void OnGUI()
        {
            // Ok so actually we shouldn't use EditorGUILayout and Instead use EditorGUI if we want to be able to draw
            // like textures or properties fields.. Or maybe not?
            
            EditorGUILayout.LabelField("Hello!");
            if (_baseScriptableObject)
            {
                using (new EditorGUI.DisabledScope(disabled: true))
                {
                    _baseScriptableObject = (ScriptableObject)EditorGUILayout.ObjectField("ScriptableObjectField",
                        _baseScriptableObject, typeof(ScriptableObject), false);
                }

                if (_baseScriptableObject)
                {
                    _foldoutState = EditorGUILayout.Foldout(_foldoutState, "Properties");
                    if (_foldoutState)
                    {
                        LaborerWindowGUI.DrawChildProperties(_baseScriptableObject);
                    }
                }
            }
            else
            {
                _baseScriptableObject = (ScriptableObject)EditorGUILayout.ObjectField("ScriptableObjectField",
                    _baseScriptableObject, typeof(ScriptableObject), false);
            }
            
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                _path = EditorGUILayout.TextField("Path", _path);
            }
            if (GUILayout.Button("GetPath"))
            {
                _path = EditorUtility.OpenFolderPanel("Save ScriptableObject at:", "", "");
            }
            
            _texture2D = (Texture2D) EditorGUILayout.ObjectField("Texture2D", _texture2D, typeof(Texture2D), false);
            
            if (_texture2D)
            {
                using (new EditorGUI.DisabledScope(disabled: false))
                {
                    Rect rect = EditorGUILayout.GetControlRect();
                    EditorGUI.DrawPreviewTexture(new Rect(rect.position, new Vector2(50, 50)), _texture2D);
                }
            }
            
        }
        
        
    }
}