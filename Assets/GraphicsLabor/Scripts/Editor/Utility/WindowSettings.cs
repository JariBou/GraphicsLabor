using System.Collections.Generic;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    internal class WindowSettings : ScriptableObject
    {
        [SerializeField] public List<EditorWindowBase> OpenedCustomEditors = new();

    }
}