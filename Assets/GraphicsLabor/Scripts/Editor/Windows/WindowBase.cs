using System;
using GraphicsLabor.Scripts.Core.Settings;
using GraphicsLabor.Scripts.Editor.Settings;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public abstract class WindowBase : EditorWindow
    {
        public string WindowName { get; protected set; }
        public Type SelfType { get; private set; }
        
        private static GraphicsLaborSettings _glSettings;
        private static WindowSettings _windowSettings;

        protected abstract void OnGUI();
        
        private void OnInspectorUpdate()
        {
            Repaint();
        }
        
        protected virtual void OnDestroy()
        {
            GetWindowSettings().OpenedCustomWindows.Remove(GetWindowSettings().FindWindowWhere(ComparisonPredicate));
        }
        
        protected virtual bool ComparisonPredicate(WindowBase editor)
        {
            bool isSame = true;
            isSame &= editor.WindowName == WindowName;
            isSame &= editor.SelfType == SelfType;
            // isSame &= editor.InstanceId == InstanceId;
            return isSame;
        }
        
        protected static T CreateAndInitWindow<T>(string displayName, params Type[] desiredDockNextTo) where T : WindowBase
        {
            WindowBase window = CreateWindow<T>(desiredDockNextTo);
            window.titleContent = new GUIContent(displayName);
            window.SelfType = typeof(T);
            window.WindowName = displayName;
            GetWindowSettings().OpenedCustomWindows.Add(window);
            return (T)window;
        }

        public static WindowSettings GetWindowSettings()
        {
            if (_windowSettings != null) return _windowSettings;
            
            WindowSettings settings = AssetDatabase.LoadAssetAtPath<WindowSettings>("Assets/GraphicsLabor/Scripts/Editor/Settings/WindowSettingsSo.asset");
            if (settings == null)
            {
                settings = CreateInstance<WindowSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/GraphicsLabor/Scripts/Editor/Settings/WindowSettingsSo.asset");
                AssetDatabase.SaveAssets();
            }

            _windowSettings = settings;
            return settings;
        }
        
        public static GraphicsLaborSettings GetSettings()
        {
            if (_glSettings != null) return _glSettings;
            
            GraphicsLaborSettings settings = AssetDatabase.LoadAssetAtPath<GraphicsLaborSettings>("Assets/GraphicsLabor/Scripts/Core/Settings/GraphicsLaborSettings.asset");
            if (settings == null)
            {
                settings = CreateInstance<GraphicsLaborSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/GraphicsLabor/Scripts/Core/Settings/GraphicsLaborSettings.asset");
                AssetDatabase.SaveAssets();
            }

            _glSettings = settings;
            return settings;
        }

        public void SetType(Type getType)
        {
            SelfType = getType;
        }
    }
}