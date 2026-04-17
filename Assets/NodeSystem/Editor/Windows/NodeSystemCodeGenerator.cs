using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using GraphicsLabor.Scripts.Editor.Utility;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Attributes.EditorTarget;
using NodeSystem.Runtime.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Windows
{
    public class NodeSystemCodeGenerator : EditorWindow
    {
        public const string DefaultDeclsGeneratedPath = "Assets/NodeSystem/Generated";
        public const string DefaultEventNodesGeneratedPath = "Assets/NodeSystem/Generated/EventNodes";

        private const string AutoGenDisclaimer = "//\n//\n" +
                                                 "// AUTO-GENERATED CODE - DO NOT MODIFY BY HAND!\n" +
                                                 "//\n" +
                                                 "// To regenerate this file, look at the NodeSystem's Node System Code Generator in Editor\n" +
                                                 "//\n//\n";

        [FormerlySerializedAs("_generatedPath")] [SerializeField]
        private string _declsGeneratedPath = "Assets/NodeSystem/Generated";

        [SerializeField] private string _eventNodesGeneratedPath = "Assets/NodeSystem/Generated/EventNodes";
        private float _verticalSpacing;

        private void OnGUI()
        {
            _verticalSpacing = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            Rect currentRect = EditorGUILayout.GetControlRect();
            int lineNumber = 0;
            {
                Rect rect = new(currentRect.x, currentRect.y + lineNumber * _verticalSpacing, currentRect.width,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect,
                    "Internal Generated AssemblyDecls Path (Change only if you know what you are doing)");
                lineNumber++;
            }
            {
                Rect rect = new(currentRect.x, currentRect.y + lineNumber * _verticalSpacing, currentRect.width,
                    EditorGUIUtility.singleLineHeight);
                _declsGeneratedPath = EditorGUI.TextField(rect, _declsGeneratedPath);
                lineNumber++;
            }
            {
                Rect rect = new(currentRect.x, currentRect.y + lineNumber * _verticalSpacing, currentRect.width,
                    EditorGUIUtility.singleLineHeight);
                if (GUI.Button(rect, "Generate")) GenerateDecls();

                lineNumber++;
            }

            lineNumber++;

            {
                Rect rect = new(currentRect.x, currentRect.y + lineNumber * _verticalSpacing, currentRect.width,
                    EditorGUIUtility.singleLineHeight);
                EditorGUI.LabelField(rect, "Generated EventNodes Path (Change only if you know what you are doing)");
                lineNumber++;
            }
            {
                Rect rect = new(currentRect.x, currentRect.y + lineNumber * _verticalSpacing, currentRect.width,
                    EditorGUIUtility.singleLineHeight);
                _eventNodesGeneratedPath = EditorGUI.TextField(rect, _eventNodesGeneratedPath);
                lineNumber++;
            }

            {
                // Generate event nodes butoon
                Rect generateEventNodesButtonRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y + lineNumber * _verticalSpacing,
                    width = position.width,
                    height = EditorGUIUtility.singleLineHeight
                };

                if (GUI.Button(generateEventNodesButtonRect, "Generate Event Nodes")) GenerateEventNodes();
            }
        }

        [MenuItem("Window/" + NodeSystemConsts.AddComponentMenuCategoryName + "/Node System Code Generator")]
        private static void Init()
        {
            GetWindow<NodeSystemCodeGenerator>();
        }

        private void GenerateDecls()
        {
            StringBuilder content = new();

            Assembly assembly = typeof(NodeSystemNode).Assembly;
            var types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(NodeSystemNode))).ToArray();

            content.Append(AutoGenDisclaimer);
            content.Append("using UnityEngine;\n\n");

            foreach (Type type in types)
            {
                if (type.IsGenericType) continue;
                content.Append($"[assembly: MakeSerializable(typeof({type.FullName}))]\n");
            }


            IOHelper.CreateFolder(_declsGeneratedPath); // Just in case
            File.WriteAllText(_declsGeneratedPath + "/AssemblyDecls.cs", content.ToString());

            AssetDatabase.Refresh();
        }

        private void GenerateEventNodes()
        {
            IOHelper.CreateFolder(_eventNodesGeneratedPath); // Just in case

            var selectMany = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GetName().Name.StartsWith("Unity"))
                .SelectMany(assembly =>
                    assembly.GetTypes().Where(type => type.IsDefined(typeof(GenerateEventNodeAttribute)))).ToArray();

            foreach (Type type in selectMany) TryGenerateNodeForType(type);

            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(_eventNodesGeneratedPath);
            EditorUtility.FocusProjectWindow();
        }

        private void TryGenerateNodeForType(Type type)
        {
            GenerateEventNodeAttribute generateAttribute = type.GetCustomAttribute<GenerateEventNodeAttribute>();

            StringBuilder content = new();
            if (type.FullName == null)
            {
                Debug.LogError(
                    $"No FullName for type {type.AssemblyQualifiedName} when trying to generate Event Node. Skipping...");
                return;
            }

            string typeUsing = "using " + type.FullName.Split('.')[..^1].Aggregate((a, b) => a + '.' + b) + ";\n";
            string className = (type.Name.EndsWith("Data") ? type.Name[..^4] : type.Name) + "Node";

            content.Append(AutoGenDisclaimer);
            content.Append("using System;\n");
            content.Append("using NodeSystem.Runtime.Attributes.EditorTarget;\n");
            content.Append("using NodeSystem.Runtime.Core;\n");
            content.Append("using NodeSystem.Runtime.NodesLibrary.Events;\n");
            content.Append("using UnityEngine;\n\n");
            content.Append(typeUsing);
            content.Append("\n");

            content.Append("namespace NodeSystem.Generated.EventNodes\n");
            content.Append("{\n");
            {
                content.Append($"\t[EventNodeInfo(\"{generateAttribute.NodeTitle}\"," +
                               $" \"{generateAttribute.MenuItem}\"," +
                               $" isPure: {generateAttribute.IsPure.ToString().ToLower()}), Serializable]\n");
                content.Append($"\tpublic class {className} : EventNodeBase<{type.Name}>\n");
                content.Append("\t{\n");

                {
                    content.Append("\t\t[EventExposedProperty]\n");
                    content.Append($"\t\tpublic {type.Name} eventData;\n");
                }
                content.Append("\n\n");
                {
                    content.Append($"\t\tpublic override Awaitable Invoke(ExecContext ctx, {type.Name} data)\n");
                    content.Append("\t\t{\n");
                    {
                        content.Append("\t\t\tthis.eventData = data;\n");
                        content.Append("\t\t\treturn DefaultInvoke(ctx);\n");
                    }
                    content.Append("\t\t}\n");
                }

                content.Append("\t}\n");
            }

            content.Append("}\n");

            File.WriteAllText(_eventNodesGeneratedPath + $"/{className}.cs", content.ToString());
        }
    }
}