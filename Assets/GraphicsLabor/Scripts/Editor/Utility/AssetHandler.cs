﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEditor;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class AssetHandler
    {
        [OnOpenAsset]
        private static bool OpenEditor(int instanceID, int line)
        {
            // Also works with user Custom ScriptableObjects that do not inherit from one of the SO parents provided
            Object obj = EditorUtility.InstanceIDToObject(instanceID);

            // Not handled by GraphicsLabor
            // if (!obj.InheritsFrom(typeof(ScriptableObject))) return false;
            
            // Editable takes priority over Manager


            IEnumerable<CustomAttributeData> objectCustomAttributes = ReflectionUtility.GetAllAttributesOfObject(obj,
                data => data.AttributeType.IsSubclassOf(typeof(ScriptableObjectAttribute)), true).ToList();
            

            List<Type> attributeTypes = objectCustomAttributes.Select(data => data.AttributeType).ToList();

            if (attributeTypes.Count == 0)
            {
                return false;
            }
            
            if (attributeTypes.Contains(typeof(EditableAttribute)))
            {
                ScriptableObjectEditorWindow.ShowWindow(obj);
                return true;
            }

            // Not handled by GraphicsLabor
            return false;
        }

        /// <summary>
        /// Path must start with "Assets/"
        /// </summary>
        public static void CreateFolder(string parentFolder, string newFolderName)
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(parentFolder, newFolderName)))
            {
                AssetDatabase.CreateFolder(parentFolder, newFolderName);
                AssetDatabase.Refresh();
            }
        }
        
        /// <summary>
        /// Creates necessary folders to create the give path
        /// Path must start with "Assets/"
        /// </summary>
        public static void CreateFolder(string fullPath)
        {
            String[] pathParts = fullPath.Split("/");
            String currParentPath = pathParts[0];
            for (int i = 1; i < pathParts.Length; i++)
            {
                CreateFolder(currParentPath, pathParts[i]);
                currParentPath += $"/{pathParts[i]}";
            }
        }
    }
}