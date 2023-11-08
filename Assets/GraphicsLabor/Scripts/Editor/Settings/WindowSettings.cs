using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Settings
{
    public sealed class WindowSettings : ScriptableObject
    {
        [SerializeField, ReadOnly] public List<WindowBase> _openedCustomWindows = new();
        public List<WindowBase> OpenedCustomWindows => _openedCustomWindows;


        public WindowBase FindWindowWhere(Func<WindowBase, bool> predicate)
        {
            WindowBase window = _openedCustomWindows.Find(predicate.Invoke);
            return window != default ? window : null;
        }
    }
}