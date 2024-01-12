using System.Collections.Generic;
using System.IO;
using System.Text;
using GraphicsLabor.Scripts.Core.Settings;
using NUnit.Framework;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class EnumGenerator
    {
        private const string Path = "Assets/GraphicsLabor/Scripts/Core/LaborerTags/LaborTags.cs";

        public static void CreateTagEnumFile()
        {
            
            GraphicsLaborSettings settings = AssetDatabase.LoadAssetAtPath<GraphicsLaborSettings>("Assets/GraphicsLabor/Scripts/Editor/Settings/GraphicsLaborSettings.asset");
            
            StringBuilder content = new();

            // List<string> enumNames = new List<string>
            // {
            //     "First", "Second", "Third"
            // };

            List<string> enumNames = settings._tags;
            content.Append("//\n//\n//\n" + 
                           "// AUTO-GENERATED CODE - DO NOT MODIFY BY HAND!\n" +
                           "//\n" +
                           "// To regenerate this file, look at GraphicLaborer's TagManager\n" +
                           "//\n//\n//\n");
            content.Append("namespace GraphicsLabor.Scripts.Core.LaborerTags\n{\n");
            content.Append("\t[System.Flags] public enum LaborTags\n\t{\n");

            for (int i = 0; i < enumNames.Count; i++)
            {

                content.Append($"\t\t{enumNames[i]} = 1 << {i+1},\n");
            }

            content.Append("\t}\n");
            content.Append("}");
            
            
            AssetHandler.CreateFolder(settings._tagsPath); // Just in case
            File.WriteAllText(settings._tagsPath + "/LaborTags.cs", content.ToString());
            
            AssetDatabase.Refresh();
        }
    }
}