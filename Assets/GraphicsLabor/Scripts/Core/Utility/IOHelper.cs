﻿using System;
using System.IO;
using UnityEditor;

namespace GraphicsLabor.Scripts.Core.Utility
{
    public static class IOHelper
    {
        
        /// <summary>
        /// Path must start with "Assets/"
        /// </summary>
        public static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (!AssetDatabase.IsValidFolder(Path.Combine(parentFolderPath, newFolderName)))
            {
                AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
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