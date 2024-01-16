using System;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Settings;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GraphicsLabor.Scripts.Editor.Windows
{
    public abstract class EditorWindowBase : WindowBase
    {
        protected abstract void PassInspectedObject(Object obj);
        
        protected static T CreateNewEditorWindow<T>(Object obj, string displayName = "EditorWindowBase") where T : EditorWindowBase
        {
            WindowBase window = null;
            WindowSettings settings = GetWindowSettings();

            if (settings.OpenedCustomWindows.Count != 0)
            {
                window = settings.FindWindowWhere(customEditor =>
                    (customEditor.WindowName == (obj != null ? obj.name : "null") || customEditor.WindowName == displayName) && customEditor.SelfType == typeof(T));
            }
            
            if (window == null)
            {
                window = CreateAndInitWindow<T>(obj, displayName, typeof(T));
            }
            
            window.Focus();
            (window as T)?.PassInspectedObject(obj);
            return window as T;
        }

        private static T CreateAndInitWindow<T>(Object obj, string displayName, params Type[] desiredDockNextTo) where T : EditorWindowBase
        {
            T window = WindowBase.CreateAndInitWindow<T>(displayName, desiredDockNextTo);
            window.PassInspectedObject(obj);
            return window;
        }

        [DidReloadScripts]
        private static void OnScriptReloadSelf()
        {
            GLogger.Log("Reloading scripts");
            
            WindowSettings settings = GetWindowSettings();
            if (settings.OpenedCustomWindows.Count == 0)
            {
                settings.OpenedCustomWindows.AddRange(Resources.FindObjectsOfTypeAll<WindowBase>());

                foreach (WindowBase windowBase in settings.OpenedCustomWindows)
                {
                    windowBase.SetType(windowBase.GetType());
                }
            }
        }

        
    }
}