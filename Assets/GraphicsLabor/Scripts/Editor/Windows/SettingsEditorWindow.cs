﻿using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Settings;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility;
using GraphicsLabor.Scripts.Editor.Utility.GUI;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public sealed class SettingsEditorWindow : ScriptableObjectEditorWindow
    {
        
        [MenuItem("Window/GraphicLabor/GL Settings")]
        public static void ShowSettings()
        {
            CreateNewEditorWindow<SettingsEditorWindow>(GetSettings(), "GL Settings");
        }

        protected override void OnSelfGui(Rect currentRect)
        {
            _totalDrawnHeight = DrawWithRect(currentRect);
            _totalDrawnHeight += LaborerGUIUtility.PropertyHeightSpacing;

            // TODO: goes out of the screen when pushed and doesn't enter the scrollable thing
            currentRect.y = _totalDrawnHeight;
            if (GUI.Button(currentRect, "Save Tags"))
            {
                TagGenerator.CreateTagEnumFile();
            }
            currentRect.y += LaborerGUIUtility.SingleLineHeight;
            if (GUI.Button(currentRect, "Save Tags2"))
            {
                GraphicsLaborSettings settings = AssetDatabase.LoadAssetAtPath<GraphicsLaborSettings>("Assets/GraphicsLabor/Scripts/Core/Settings/GraphicsLaborSettings.asset");

                List<string> enumNames = settings._tags;
            
                Dictionary<string, int> dict = new()
                {
                    ["Null"] = 0
                };
                foreach (string enumName in enumNames)
                {
                    dict[enumName] = 1;
                }
                // Raises an error: access to file is denied (maybe not in main thread?)
                EnumGenerator.GenerateEnum(dict, "LaborTags", true, "GraphicsLabor.Scripts.Core.Tags", settings._tagsPath);
            }
        }
    }
}