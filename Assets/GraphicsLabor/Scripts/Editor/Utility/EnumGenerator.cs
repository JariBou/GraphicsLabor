using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class EnumGenerator
    {
        private const string Path = "Assets/GraphicsLabor/Scripts/Core/LaborerTags/LaborTags.cs";

        public static void CreateTagEnumFile()
        {
            StringBuilder content = new();

            List<string> enumNames = new List<string>
            {
                "First", "Second", "Third"
            };
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
            
            
            
            File.WriteAllText(Path, content.ToString());
            
            AssetDatabase.Refresh();
        }
    }
}