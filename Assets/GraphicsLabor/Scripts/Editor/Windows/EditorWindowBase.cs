using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GraphicsLabor.Scripts.Editor.Windows
{
    public abstract class EditorWindowBase : EditorWindow
    {
        protected string WindowName { get; set; }
        private Type SelfType { get; set; }
        private int InstanceId { get; set; }

        private static WindowSettings _settings;
        
        protected abstract void OnGUI();
        protected abstract void PassInspectedObject(Object obj);
        
        private void OnInspectorUpdate()
        {
            Repaint();
        }

        // TODO: fix opening and closing of windows
        private void OnDestroy()
        {
            GetWindowsSetting().OpenedCustomEditors.Remove(GetWindowsSetting().OpenedCustomEditors.Find(StrictComparisonPredicate));
        }

        private bool StrictComparisonPredicate(EditorWindowBase editor)
        {
            bool isSame = true;
            isSame &= editor.WindowName == (WindowName ?? "null");
            isSame &= editor.SelfType == SelfType;
            // isSame &= editor.InstanceId == InstanceId;
            return isSame;
        }

        protected static T CreateNewEditorWindow<T>(Object obj, string displayName = "EditorWindowBase") where T : EditorWindowBase
        {
            EditorWindowBase window = null;
            WindowSettings settings = GetWindowsSetting();
            
            bool found = false;
            if (settings.OpenedCustomEditors.Count != 0)
            {
                GLogger.Log(settings.OpenedCustomEditors.Count.ToString());
                foreach (EditorWindowBase customEditor in settings.OpenedCustomEditors
                             .Where(customEditor => customEditor.WindowName == (obj != null ? obj.name : "null") && customEditor.SelfType == typeof(T)))
                {
                    window = customEditor;
                    found = true;
                    break;
                }
            }
            
            if (!found)
            {
                window = CreateAndInitWindow<T>(obj, displayName, typeof(T));
            }
            
            window.Focus();
            return window as T;
        }

        private static T CreateAndInitWindow<T>(Object obj, string displayName, params Type[] desiredDockNextTo) where T : EditorWindowBase
        {
            GLogger.Log("Initing new window");
            EditorWindowBase window = CreateWindow<T>(desiredDockNextTo);
            window.titleContent = new GUIContent(displayName);
            window.SelfType = typeof(T);
            window.InstanceId = window.GetInstanceID();
            window.PassInspectedObject(obj);
            GetWindowsSetting().OpenedCustomEditors.Add(window);
            return (T)window;
        }

        private static WindowSettings GetWindowsSetting()
        {
            if (_settings != null) return _settings;
            
            WindowSettings settings = AssetDatabase.LoadAssetAtPath<WindowSettings>("Assets/GraphicsLabor/Settings/WindowSettingsSo.asset");
            if (settings == null)
            {
                settings = CreateInstance<WindowSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/GraphicsLabor/Settings/WindowSettingsSo.asset");
                AssetDatabase.SaveAssets();
            }

            _settings = settings;
            return settings;
        }
    }
}