using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using Object = UnityEngine.Object;

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
        
        public static void CreateAssetIfNeeded(Object obj, string path, bool saveAssets = true)
        {
            var assetAtPath = AssetDatabase.LoadAssetAtPath<Object>(path);

            if (assetAtPath == null)
            {
                AssetDatabase.CreateAsset(obj, path);
            }

            if (saveAssets)
            {
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Deletes all non-whitelisted assets from the folders
        /// </summary>
        /// <param name="folders">An Array of strings with the paths to the folders that should be checked</param>
        /// <param name="whiteList">A IEnumerable of strings containing paths to Assets that should not be deleted</param>
        public static void DeleteAssets(string[] folders, IEnumerable<string> whiteList)
        {
            foreach (string asset in AssetDatabase.FindAssets("", folders))
            {
                string path = AssetDatabase.GUIDToAssetPath(asset);
                // ReSharper disable once PossibleMultipleEnumeration
                if (!whiteList.Contains(path))
                {
                    AssetDatabase.DeleteAsset(path);
                }
            }
        }
    }
}