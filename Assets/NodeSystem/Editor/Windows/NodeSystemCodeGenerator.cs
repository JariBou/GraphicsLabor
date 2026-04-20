using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
#if GraphicsLaborImplemented
using GraphicsLabor.Scripts.Editor.Utility;
#endif
using NodeSystem.Runtime;
using NodeSystem.Runtime.Attributes;
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

        [FormerlySerializedAs("_generatedPath"), SerializeField] 
        private string _declsGeneratedPath = "Assets/NodeSystem/Generated";

        [SerializeField] private string _eventNodesGeneratedPath = "Assets/NodeSystem/Generated/EventNodes";
        private float _verticalSpacing;

        private List<Type> _cachedGenEventDataTypes;
        private Vector2 _scrollPos = Vector2.zero;
        private bool _foldoutState;

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
                // Generate event nodes button
                Rect generateEventNodesButtonRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y + lineNumber * _verticalSpacing,
                    width = position.width,
                    height = EditorGUIUtility.singleLineHeight
                };
                
                if (GUI.Button(generateEventNodesButtonRect, "Generate All Event Nodes")) GenerateEventNodes();

                lineNumber += 1;
                
                Rect foldoutRect = new()
                {
                    x = currentRect.x + 5 * 3,
                    y = currentRect.y + lineNumber * _verticalSpacing,
                    width = position.width,
                    height = _verticalSpacing,
                };
                
                lineNumber++;

                _foldoutState = EditorGUI.Foldout(foldoutRect, _foldoutState, "Individual Event Creation");
                if (_foldoutState)
                {
                    Rect scrollViewRect = new()
                    {
                        x = currentRect.x,
                        y = currentRect.y + lineNumber * _verticalSpacing,
                        width = position.width - 5,
                        height = _verticalSpacing * 6,
                    };

                    Rect scrollViewContentRect = new()
                    {
                        x = scrollViewRect.x,
                        y = scrollViewRect.y,
                        width = scrollViewRect.width - 15,
                        height = _verticalSpacing * GetGenEventTypes().Count,
                    };

                    lineNumber += GetGenEventTypes().Count;
                    
                    GUI.Box(scrollViewRect, GUIContent.none);

                    _scrollPos = GUI.BeginScrollView(scrollViewRect, _scrollPos, scrollViewContentRect, false, true);
                    List<Type> list = GetGenEventTypes();
                
                    for (int index = 0; index < list.Count; index++)
                    {
                        Rect content = new()
                        {
                            x = scrollViewRect.x,
                            y = scrollViewRect.y + _verticalSpacing * index,
                            width = scrollViewContentRect.width,
                            height = _verticalSpacing,
                        };
                        Type type = list[index];
                        if (GUI.Button(content, $"Generate Nodes for '{type.Name}'"))
                        {
                            TryGenerateNodeForType(type);
                            TryGenerateBreakNodeForType(type);
                            AssetDatabase.Refresh();
                            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(_eventNodesGeneratedPath);
                            EditorUtility.FocusProjectWindow();
                        }
                    }
                
                    GUI.EndScrollView();
                }
            }
        }

        [MenuItem("Window/" + NodeSystemConsts.AddComponentMenuCategoryName + "/Node System Code Generator")]
        private static void Init()
        {
            GetWindow<NodeSystemCodeGenerator>();
        }

        private List<Type> GetGenEventTypes()
        {
            _cachedGenEventDataTypes ??= AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GetName().Name.StartsWith("Unity"))
                .SelectMany(assembly =>
                    assembly.GetTypes().Where(type => type.IsDefined(typeof(GenerateEventNodeAttribute)))).ToList();
            return _cachedGenEventDataTypes;
        }

        private void GenerateDecls()
        {
            StringBuilder content = new();

            Assembly assembly = typeof(NodeSystemNode).Assembly;
            Type[] types = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(NodeSystemNode))).ToArray();

            content.Append(AutoGenDisclaimer);
            content.Append("using UnityEngine;\n\n");

            foreach (Type type in types)
            {
                if (type.IsGenericType) continue;
                content.Append($"[assembly: MakeSerializable(typeof({type.FullName}))]\n");
            }

            string fullPath = _declsGeneratedPath;
#if GraphicsLaborImplemented
            IOHelper.CreateFolder(fullPath); // Just in case
#else
            String[] pathParts = fullPath.Split("/");
            String currParentPath = pathParts[0];
            for (int i = 1; i < pathParts.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(Path.Combine(currParentPath, pathParts[i])))
                {
                    AssetDatabase.CreateFolder(currParentPath, pathParts[i]);
                    AssetDatabase.Refresh();
                }
                currParentPath += $"/{pathParts[i]}";
            }
#endif
            File.WriteAllText(_declsGeneratedPath + "/AssemblyDecls.cs", content.ToString());

            AssetDatabase.Refresh();
        }

        private void GenerateEventNodes()
        {
            string fullPath = _eventNodesGeneratedPath;
#if GraphicsLaborImplemented
            IOHelper.CreateFolder(fullPath); // Just in case
#else
            String[] pathParts = fullPath.Split("/");
            String currParentPath = pathParts[0];
            for (int i = 1; i < pathParts.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(Path.Combine(currParentPath, pathParts[i])))
                {
                    AssetDatabase.CreateFolder(currParentPath, pathParts[i]);
                    AssetDatabase.Refresh();
                }
                currParentPath += $"/{pathParts[i]}";
            }
#endif

            Type[] selectMany = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.GetName().Name.StartsWith("Unity"))
                .SelectMany(assembly =>
                    assembly.GetTypes().Where(type => type.IsDefined(typeof(GenerateEventNodeAttribute)))).ToArray();

            foreach (Type type in selectMany)
            {
                TryGenerateNodeForType(type);
                TryGenerateBreakNodeForType(type);
            }

            AssetDatabase.Refresh();
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Object>(_eventNodesGeneratedPath);
            EditorUtility.FocusProjectWindow();
        }

        private void TryGenerateBreakNodeForType(Type type)
        {
            string className = "Break" + type.Name + "Node";
            string classTypeUsing = "using " + type.Namespace + ";\n";
            
            StringBuilder content = new();
            content.Append(AutoGenDisclaimer);
            content.Append("using System;\n");
            content.Append("using NodeSystem.Runtime;\n");
            content.Append("using NodeSystem.Runtime.Core;\n");
            content.Append("using NodeSystem.Runtime.Core.PortConfigEnums;\n");
            content.Append("using NodeSystem.Runtime.Attributes;\n");
            content.Append("using UnityEngine;\n\n");
            content.Append($"{classTypeUsing}\n");
            
            StringBuilder fields = new();
            
            fields.Append("\t\t// Input\n");
            fields.Append("\t\t[ExposedProperty(PropPortDirection.Input, preferredLocation: PropContainerLocation.InputContainer)]\n");
            fields.Append($"\t\tpublic {type.Name} eventData;\n\n");
            fields.Append("\t\t// Outputs\n");
            
            StringBuilder methodBody = new();
            methodBody.Append($"\t\t\t{type.Name} eventDataValue = await GetValueOfProp<{type.Name}>(context, nameof(eventData));\n\n");
            
            foreach (FieldInfo fieldInfo in type.GetFields())
            {
                string typeUsing = "using " + fieldInfo.FieldType.Namespace + ";\n";
                
                fields.Append("\t\t[EventExposedProperty]\n");

                if (fieldInfo.FieldType == typeof(string))
                {
                    fields.Append($"\t\tpublic string {fieldInfo.Name};\n");
                } else if (fieldInfo.FieldType == typeof(int))
                {
                    fields.Append($"\t\tpublic int {fieldInfo.Name};\n");
                } else if (fieldInfo.FieldType == typeof(bool))
                {
                    fields.Append($"\t\tpublic bool {fieldInfo.Name};\n");
                } else if (fieldInfo.FieldType == typeof(float))
                {
                    fields.Append($"\t\tpublic float {fieldInfo.Name};\n");
                } else
                {
                    content.Append(typeUsing);
                    fields.Append($"\t\tpublic {fieldInfo.FieldType.Name} {fieldInfo.Name};\n");
                }

                methodBody.Append($"\t\t\t{fieldInfo.Name} = eventDataValue.{fieldInfo.Name};\n");

            }

            methodBody.Append("\t\t\treturn await base.OnProcessAsync(context);\n");
            
            content.Append("namespace NodeSystem.Generated.EventNodes.BreakNodes\n");
            content.Append("{\n");
            {
                content.Append($"\t[NodeInfo(\"Break {type.Name} Node\"," +
                               $" \"BreakNodes/{className}\"," +
                               " isPure: true), Serializable]\n");
                content.Append($"\tpublic class {className} : NodeSystemNode\n");
                content.Append("\t{\n");

                {
                    content.Append(fields);
                }
                content.Append("\n\n");
                {
                    content.Append($"\t\tpublic override async Awaitable<ProcessInfo> OnProcessAsync(ExecContext context)\n");
                    content.Append("\t\t{\n");
                    {
                        content.Append(methodBody);
                    }
                    content.Append("\t\t}\n");
                }

                content.Append("\t}\n");
            }
            content.Append("}\n");

            string fullPath = _eventNodesGeneratedPath + "/BreakNodes";
#if GraphicsLaborImplemented
            IOHelper.CreateFolder(fullPath); // Just in case
#else
            String[] pathParts = fullPath.Split("/");
            String currParentPath = pathParts[0];
            for (int i = 1; i < pathParts.Length; i++)
            {
                if (!AssetDatabase.IsValidFolder(Path.Combine(currParentPath, pathParts[i])))
                {
                    AssetDatabase.CreateFolder(currParentPath, pathParts[i]);
                    AssetDatabase.Refresh();
                }
                currParentPath += $"/{pathParts[i]}";
            }
#endif
            
            File.WriteAllText(_eventNodesGeneratedPath + $"/BreakNodes/{className}.cs", content.ToString());
            
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

            // string typeUsing = "using " + type.FullName.Split('.')[..^1].Aggregate((a, b) => a + '.' + b) + ";\n";
            string typeUsing = "using " + type.Namespace + ";\n";
            string className = (type.Name.EndsWith("Data") ? type.Name[..^4] : type.Name) + "Node";

            content.Append(AutoGenDisclaimer);
            content.Append("using System;\n");
            content.Append("using NodeSystem.Runtime.Attributes;\n");
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
                        content.Append("\t\t\teventData = data;\n");
                        content.Append("\t\t\treturn DefaultInvokeAwaitAsync(ctx);\n");
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