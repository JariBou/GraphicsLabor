using GraphicsLabor.Scripts.Editor.Utility;
using GraphicsLabor.Scripts.Windows.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphicsLabor.Scripts.Windows
{
    public sealed class TestWindow : EditorWindowBase
    {
        private ScriptableObject _scriptableObject;
        private Texture2D _texture2D;
        private bool _foldoutState;
        private string _path;
        
        [MenuItem("Window/GraphicLabor/Test Window")]
        public static void ShowWindow()
        {
            TestWindow window = GetWindow<TestWindow>();
            window.titleContent = new GUIContent("windowName");
        }

        public override void OnGUI()
        {
            // Ok so actually we shouldn't use EditorGUILayout and Instead use EditorGUI if we want to be able to draw
            // like textures or properties fields
            
            EditorGUILayout.LabelField("Hello!");
            _scriptableObject = (ScriptableObject) EditorGUILayout.ObjectField("ScriptableObjectField", _scriptableObject, typeof(ScriptableObject), false);
            if (_scriptableObject)
            {
                _foldoutState = EditorGUILayout.Foldout(_foldoutState, "Properties");
                if (_foldoutState)
                {
                    LaborerWindowGUI.DrawChildProperties(_scriptableObject);
                }
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