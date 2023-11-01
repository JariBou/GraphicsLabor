﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(SceneAttribute))]
    public class ScenePropertyDrawer : PropertyDrawerBase
    {
        private const string SceneListItem = "{0} ({1})"; // pattern for displaying in the inspector
        private const string ScenePattern = @".+\/(.+)\.unity"; // Regex pattern for all unity scenes names (end in .unity)
        private const string TypeWarningMessage = "{0} must be an int or a string";
        private const string BuildSettingsWarningMessage = "No scenes found in the build settings";

        protected override float GetSelfPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool validPropertyType = property.propertyType is SerializedPropertyType.String or SerializedPropertyType.Integer;
            bool anySceneInBuildSettings = GetScenes().Length > 0;

            return (validPropertyType && anySceneInBuildSettings)
                ? GetPropertyHeight(property)
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnSelfGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            string[] scenes = GetScenes();
            bool anySceneInBuildSettings = scenes.Length > 0;
            if (!anySceneInBuildSettings)
            {
                DrawDefaultPropertyAndHelpBox(rect, property, BuildSettingsWarningMessage, MessageType.Warning);
                return;
            }

            string[] sceneOptions = GetSceneOptions(scenes);
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    DrawPropertyForString(rect, property, label, scenes, sceneOptions);
                    break;
                case SerializedPropertyType.Integer:
                    DrawPropertyForInt(rect, property, label, sceneOptions);
                    break;
                default:
                    string message = string.Format(TypeWarningMessage, property.name);
                    DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
                    break;
            }

            EditorGUI.EndProperty();
        }

        private string[] GetScenes()
        {
            return EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => Regex.Match(scene.path, ScenePattern).Groups[1].Value)
                .ToArray();
        }

        private string[] GetSceneOptions(string[] scenes)
        {
            return scenes.Select((s, i) => string.Format(SceneListItem, s, i)).ToArray();
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, string[] scenes, string[] sceneOptions)
        {
            int index = IndexOf(scenes, property.stringValue);
            int newIndex = EditorGUI.Popup(rect, label.text, index, sceneOptions);
            string newScene = scenes[newIndex];

            if (!property.stringValue.Equals(newScene, StringComparison.Ordinal))
            {
                property.stringValue = scenes[newIndex];
            }
        }

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, string[] sceneOptions)
        {
            int index = property.intValue;
            int newIndex = EditorGUI.Popup(rect, label.text, index, sceneOptions);

            if (property.intValue != newIndex)
            {
                property.intValue = newIndex;
            }
        }

        private static int IndexOf(string[] scenes, string scene)
        {
            var index = Array.IndexOf(scenes, scene);
            return Mathf.Clamp(index, 0, scenes.Length - 1);
        }
    }
}