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
        
        /// <summary>
        /// The Comparison Predicate used to determine if a window is the same as another
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        protected virtual bool ComparisonPredicate(WindowBase editor)
        {
            bool isSame = true;
            isSame &= editor.WindowName == WindowName;
            isSame &= editor.SelfType == SelfType;
            // isSame &= editor.InstanceId == InstanceId;
            return isSame;
        }
        
        /// <summary>
        /// Creates and inits a new WindowBase
        /// </summary>
        /// <param name="displayName">The window's display name</param>
        /// <param name="desiredDockNextTo">The type of window to dock next to</param>
        /// <typeparam name="T">The type of the window</typeparam>
        /// <returns></returns>
        protected static T CreateAndInitWindow<T>(string displayName, params Type[] desiredDockNextTo) where T : WindowBase
        {
            WindowBase window = CreateWindow<T>(desiredDockNextTo);
            window.titleContent = new GUIContent(displayName);
            window.SelfType = typeof(T);
            window.WindowName = displayName;
            GetWindowSettings().OpenedCustomWindows.Add(window);
            return (T)window;
        }

        /// <summary>
        /// Returns the ScriptableObject holding Window Settings 
        /// </summary>
        /// <returns></returns>
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
        
        /// <summary>
        /// Returns the ScriptableObject holding GraphicsLabor Settings 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Sets the SelfType variable
        /// </summary>
        /// <param name="getType">The type to set to</param>
        public void SetType(Type getType)
        {
            SelfType = getType;
        }
    }
}