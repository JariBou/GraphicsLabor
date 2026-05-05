using System.IO;
using UnityEditor;

namespace NodeSystem.Editor.Utils
{
    public static class IOHelper
    {
        /// <summary>
        ///     Creates a folder at parent path
        /// </summary>
        /// <param name="parentFolderPath">Full path of the parent folder</param>
        /// <param name="newFolderName">Name of the folder to be created</param>
        public static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder(Path.Combine(parentFolderPath, newFolderName))) return;

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
            AssetDatabase.Refresh();
        }

        /// <summary>
        ///     Creates necessary folders to create the given path
        ///     Path must start with "Assets/"
        /// </summary>
        /// <param name="fullPath">Full path of folders</param>
        public static void CreateFolder(string fullPath)
        {
            string[] pathParts = fullPath.Split("/");
            string currParentPath = pathParts[0];
            for (int i = 1; i < pathParts.Length; i++)
            {
                CreateFolder(currParentPath, pathParts[i]);
                currParentPath += $"/{pathParts[i]}";
            }
        }
    }
}