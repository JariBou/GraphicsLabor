using System;
using GraphicsLabor.Scripts.Editor.Settings;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GraphicsLabor.Scripts.Editor.Windows
{
    public abstract class EditorWindowBase : WindowBase
    {
        /// <summary>
        /// Called to pass the Inspected Object
        /// </summary>
        /// <param name="obj">Inspected Object</param>
        protected abstract void PassInspectedObject(Object obj);
        
        /// <summary>
        /// Creates and returns a new EditorWindow 
        /// </summary>
        /// <param name="obj">The Object the editor will be inspecting</param>
        /// <param name="displayName">The display name of the window</param>
        /// <typeparam name="T">The type of the editor window</typeparam>
        /// <returns></returns>
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
            else
            {
                (window as T)?.PassInspectedObject(obj);
            }

            window.Focus();
            return window as T;
        }

        /// <summary>
        /// Creates and inits a new EditorWindow
        /// </summary>
        /// <param name="obj">The inspected object</param>
        /// <param name="displayName">The window's display name</param>
        /// <param name="desiredDockNextTo">The type of window to dock next to</param>
        /// <typeparam name="T">The type of the editor window</typeparam>
        /// <returns></returns>
        private static T CreateAndInitWindow<T>(Object obj, string displayName, params Type[] desiredDockNextTo) where T : EditorWindowBase
        {
            T window = WindowBase.CreateAndInitWindow<T>(displayName, desiredDockNextTo);
             window.PassInspectedObject(obj);
            return window;
        }

        [DidReloadScripts]
        private static void OnScriptReloadSelf()
        {
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