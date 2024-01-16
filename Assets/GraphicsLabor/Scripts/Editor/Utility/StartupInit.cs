﻿using System;
using GraphicsLabor.Scripts.Editor.Settings;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    [InitializeOnLoad]
    public class Startup {
        static Startup()
        {
            // should close all Windows in the settings and reset all
            WindowSettings settings = WindowBase.GetWindowSettings();
            foreach (WindowBase window in settings.OpenedCustomWindows)
            {
                try
                {
                    window.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            settings.OpenedCustomWindows.Clear();
        }
    }
}