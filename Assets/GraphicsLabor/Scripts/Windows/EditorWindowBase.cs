using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphicsLabor.Scripts.Windows
{
    public enum ElementType
    {
        Label = 1,
        Toggle = 5,
        
    }
    public abstract class EditorWindowBase : EditorWindow
    {
        public abstract void OnGUI();
    }
}