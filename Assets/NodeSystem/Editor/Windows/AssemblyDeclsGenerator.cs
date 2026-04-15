using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphicsLabor.Scripts.Editor.Utility;
using NodeSystem.Runtime;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Editor.Windows
{
    public class AssemblyDeclsGenerator : EditorWindow
    {
        public const string DefaultGeneratedPath = "Assets/NodeSystem/Generated";
        [SerializeField] private string _generatedPath = "Assets/NodeSystem/Generated";
        
        [MenuItem("Window/NodeSystem/AssemblyDeclsGenerator")]
        static void Init()
        {
            GetWindow<AssemblyDeclsGenerator>();
        }

        private void OnGUI()
        {
            Rect currentRect = EditorGUILayout.GetControlRect();
            int lineNumber = 0;
            {
                Rect rect = new Rect(currentRect.x, currentRect.y + lineNumber * EditorGUIUtility.singleLineHeight, currentRect.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, "Generated AssemblyDecls Path (Change only if you know what you are doing)");
                lineNumber++;
            }
            {
                Rect rect = new Rect(currentRect.x, currentRect.y + lineNumber * EditorGUIUtility.singleLineHeight, currentRect.width, EditorGUIUtility.singleLineHeight);
                _generatedPath = EditorGUI.TextField(rect, _generatedPath);
                lineNumber++;
            }
            {
                Rect rect = new Rect(currentRect.x, currentRect.y + lineNumber * EditorGUIUtility.singleLineHeight, currentRect.width, EditorGUIUtility.singleLineHeight);
                if (GUI.Button(rect, "Generate"))
                {
                    Generate();        
                }
            }
        }

        private void Generate()
        {
            StringBuilder content = new();
            
            Assembly assembly = typeof(NodeSystemNode).Assembly;
            Type[] types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(NodeSystemNode))).ToArray();

            content.Append("//\n//\n" + 
                           "// AUTO-GENERATED CODE - DO NOT MODIFY BY HAND!\n" +
                           "//\n" +
                           "// To regenerate this file, look at the NodeSystem's AssemblyDeclsGenerator in Editor\n" +
                           "//\n//\n");
            content.Append("using UnityEngine;\n\n");

            foreach (Type type in types)
            {
                if (type.IsGenericType)
                {
                    continue;
                }
                content.Append($"[assembly: MakeSerializable(typeof({type.FullName}))]\n");
            }
            
            
            IOHelper.CreateFolder(_generatedPath); // Just in case
            File.WriteAllText(_generatedPath + "/AssemblyDecls.cs", content.ToString());
            
            AssetDatabase.Refresh();
        }
    }
}