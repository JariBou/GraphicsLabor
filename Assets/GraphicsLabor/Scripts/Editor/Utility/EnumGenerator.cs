using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GraphicsLabor.Scripts.Core.Settings;
using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class EnumGenerator
    {
        public static void GenerateEnum(Dictionary<string, int> enumAndValuesDictionary, string enumClassName,
            bool isFlag = false, string enumNamespace = null)
        {
            StringBuilder content = new();
            
            content.Append("//\n//\n" + 
                           "// AUTO-GENERATED CODE - Any modifications made will be overwritten on next generation!\n" +
                           "//\n" +
                           "// To regenerate this file, look at GraphicLaborer's Enum Manager in Editor\n" +
                           "//\n//\n");
            if (enumNamespace != null)
            {
                content.Append($"namespace {enumNamespace}\n{{\n");
            }

            string flagText = isFlag ? "[System.Flags] " : "";
            
            if (enumNamespace != null) content.Append("\t");
            
            content.Append($"{flagText}public enum {enumClassName}\n\t{{\n");

            string[] names = enumAndValuesDictionary.Keys.ToArray();
            
            foreach (string name in names)
            {
                if (enumNamespace != null) content.Append("\t");

                int enumValue = enumAndValuesDictionary[name];
                content.Append(isFlag ? $"\t{name} = 1 << {enumValue},\n" : $"\t{name} = {enumValue},\n");
            }
            
            if (enumNamespace != null) content.Append("\t");
            content.Append("}\n");
            if (enumNamespace != null) content.Append("}");
            
            GraphicsLaborSettings settings = AssetDatabase.LoadAssetAtPath<GraphicsLaborSettings>("Assets/GraphicsLabor/Scripts/Core/Settings/GraphicsLaborSettings.asset");

            IOHelper.CreateFolder(settings._defaultEnumsPath); // Just in case
            File.WriteAllText(settings._defaultEnumsPath + $"/{enumClassName}.cs", content.ToString());
            
            AssetDatabase.Refresh();
        }
        
        public static void GenerateEnum(IEnumerable<string> enumNames, string enumClassName, bool isFlag = false, string enumNamespace = null)
        {
            StringBuilder content = new();
            
            content.Append("//\n//\n" + 
                           "// AUTO-GENERATED CODE - Any modifications made will be overwritten on next generation!\n" +
                           "//\n" +
                           "// To regenerate this file, look at GraphicLaborer's Enum Manager in Editor\n" +
                           "//\n//\n");
            if (enumNamespace != null)
            {
                content.Append($"namespace {enumNamespace}\n{{\n");
            }

            string flagText = isFlag ? "[System.Flags] " : "";
            
            if (enumNamespace != null) content.Append("\t");
            
            content.Append($"{flagText}public enum {enumClassName}\n");
            
            if (enumNamespace != null) content.Append("\t");
            
            content.Append("{\n");

            string[] names = enumNames as string[] ?? enumNames.ToArray();
            for (int i = 0; i < names.Length; i++)
            {
                if (enumNamespace != null) content.Append("\t");

                content.Append(isFlag ? $"\t{names[i]} = 1 << {i},\n" : $"\t{names[i]} = {i},\n");
            }
            
            if (enumNamespace != null) content.Append("\t");
            content.Append("}\n");
            if (enumNamespace != null) content.Append("}");
            
            GraphicsLaborSettings settings = AssetDatabase.LoadAssetAtPath<GraphicsLaborSettings>("Assets/GraphicsLabor/Scripts/Core/Settings/GraphicsLaborSettings.asset");

            IOHelper.CreateFolder(settings._defaultEnumsPath); // Just in case
            File.WriteAllText(settings._defaultEnumsPath + $"/{enumClassName}.cs", content.ToString());
            
            AssetDatabase.Refresh();
        }

        public static void GenerateEnum(EnumSettings settings)
        {
            GenerateEnum(settings.EnumNames, settings.EnumClassName, settings.IsFlag, settings.EnumSpace);
        }
        
        
        
    }
}