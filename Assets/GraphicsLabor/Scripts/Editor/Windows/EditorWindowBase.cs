using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace GraphicsLabor.Scripts.Editor.Windows
{
    public abstract class EditorWindowBase : EditorWindow
    {
        private static readonly List<EditorWindowBase> OpenedCustomEditors = new();
        protected string WindowName { get; set; }
        private Type SelfType { get; set; }
        
        public abstract void OnGUI();
        protected abstract void PassInspectedObject(Object obj);
        
        private void OnInspectorUpdate()
        {
            Repaint();
        }

        private void OnDestroy()
        {
            OpenedCustomEditors.Remove(OpenedCustomEditors.Find(editor => editor.WindowName == WindowName && editor.SelfType == SelfType ));
        }

        protected static T CreateNewEditorWindow<T>(Object obj, string displayName = "EditorWindowBase") where T : EditorWindowBase
        {
            EditorWindowBase window = null;

            bool found = false;
            if (OpenedCustomEditors.Count != 0)
            {
                foreach (EditorWindowBase customEditor in OpenedCustomEditors.Where(customEditor => customEditor.WindowName == obj.name && customEditor.SelfType == typeof(T)))
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
            EditorWindowBase window = CreateWindow<T>(desiredDockNextTo);
            window.titleContent = new GUIContent(displayName);
            window.WindowName = obj.name;
            window.SelfType = typeof(T);
            window.PassInspectedObject(obj);
            OpenedCustomEditors.Add(window);
            return (T)window;
        }
    }
}