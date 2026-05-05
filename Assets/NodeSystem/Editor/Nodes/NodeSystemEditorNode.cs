using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeSystem.Editor.Editors.NodeEditors;
using NodeSystem.Editor.Exceptions;
using NodeSystem.Editor.Graph.Elements;
using NodeSystem.Editor.Graph.Manipulators;
using NodeSystem.Editor.Nodes.Manipulators;
using NodeSystem.Editor.Ports;
using NodeSystem.Editor.Utils;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.Core;
using NodeSystem.Runtime.Core.PortConfigEnums;
using NodeSystem.Runtime.NodesLibrary.Events;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

namespace NodeSystem.Editor.Nodes
{
    public class NodeSystemEditorNode : Node
    {
        private const long DoubleClickDelayInMs = 200;

        private const BindingFlags GetFieldsBindingFlags =
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        private static List<Assembly> _assemblies = new();

        private readonly SerializedObject _serializedObject;

        private Port _outputPort;
        private SerializedProperty _serializedProperty;

        public sealed override string title
        {
            get => base.title;
            set => base.title = value;
        }

        public NodeSystemNode Node { get; }

        public List<Port> Ports { get; }


        public NodeSystemEditorNode(NodeSystemNode node, SerializedObject serializedObject)
        {
            AddToClassList("code-graph-node");

            Node = node;
            Ports = new List<Port>();

            _serializedObject = serializedObject;

            Type typeInfo = node.GetType();
            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();
            if (info == null) throw new MissingNodeInfoException(typeInfo, serializedObject);

            Node.IsPure = info.IsPure;

            title = info.Title;

            string[] depths = info.MenuItem.Split('/');
            foreach (string depth in depths)
            {
                AddToClassList(depth.ToLower().Replace(' ', '-'));
            }

            name = typeInfo.Name;

            NodeEditorBase nodeEditor = null;
            if (!_assemblies.Any())
            {
                _assemblies = AppDomain.CurrentDomain.GetAssemblies()
                                       .Where(assembly => !assembly.GetName().ToString().StartsWith("Unity")).ToList();
            }

            IEnumerable<Type> editors = _assemblies
                .SelectMany(a => a.GetTypes().Where(t =>
                                                        t.IsDefined(typeof(CustomNodeEditorAttribute)) &&
                                                        !t.IsAbstract &&
                                                        t.GetCustomAttribute<CustomNodeEditorAttribute>().TargetType ==
                                                        node.GetType()));

            Type[] customEditors = editors as Type[] ?? editors.ToArray();

            if (node is IEventNode)
            {
                SerializedProperty property = GetSerializedPropertyOf("_eventName");

                TextField textField = new()
                {
                    // value = eventNode.EventName,
                    tooltip = "Event Name", bindingPath = property.propertyPath, style =
                    {
                        width = Length.Pixels(125), height = Length.Pixels(20), alignSelf = new StyleEnum<Align>(Align.Center),
                    },
                };

                titleContainer.Insert(1, textField);
            }

            if (customEditors.Any()) nodeEditor = Activator.CreateInstance(customEditors.First()) as NodeEditorBase;

            // Output first so always index 0
            if (info.HasFlowOutput)
            {
                if (nodeEditor == null || !nodeEditor.AddOutputPorts(this))
                {
                    for (int i = 0; i < info.OutputPortCount; i++)
                    {
                        CreateFlowOutputPort();
                    }
                }
            }

            if (info.HasFlowInput)
                if (nodeEditor == null || !nodeEditor.AddInputPorts(this))
                    CreateFlowInputPort();

            CreateExposedVariables(typeInfo);

            RefreshExpandedState();

            this.AddManipulator(new NsNodeClickSelector());
            this.AddManipulator(new DoubleClickable(OnDoubleClicked, DoubleClickDelayInMs));
        }

        private void CreateExposedVariables(Type typeInfo)
        {
            Node.PortInfos.Clear();
            foreach (FieldInfo fieldInfo in typeInfo.GetFields(GetFieldsBindingFlags))
            {
                if (fieldInfo.GetCustomAttribute<ExposedPropertyAttribute>() is { } propertyAttribute)
                    HandleSourcePropertyAttribute(propertyAttribute, fieldInfo);
            }

            RefreshPorts();
        }

        private void HandleSourcePropertyAttribute(ExposedPropertyAttribute propertyAttribute, FieldInfo fieldInfo)
        {
            if (!propertyAttribute.HasOutPort && !propertyAttribute.HasInPort) return;

            SerializedProperty prop = GetSerializedPropertyOf(fieldInfo.Name);

            if (prop == null) return;

            Direction portDirection = propertyAttribute.HasOutPort ? Direction.Output : Direction.Input;
            Type propertyType;
            try
            {
                // Can sometimes crash, like when using string instead of String idk why
                propertyType = propertyAttribute.AutoTyping
                    ? fieldInfo.FieldType /*TypeUtils.GetPropertyType(prop)*/
                    : propertyAttribute.PortType;
            }
            catch (Exception e)
            {
                propertyType = propertyAttribute.PortType;
                Console.WriteLine(e);
            }

            propertyType ??= fieldInfo.FieldType;

            EditorNodePort port = EditorNodePort.Create(Orientation.Horizontal, portDirection,
                                                        propertyAttribute.PortCapacity == PropPortCapacity.Single
                                                            ? Port.Capacity.Single
                                                            : Port.Capacity.Multi,
                                                        propertyType);
            port.HideWhenConnected = propertyAttribute.DisableInputWhenConnected;
            port.LinkedPropertyName = fieldInfo.Name;
            port.portName = "";
            port.tooltip = propertyType.ToString();
            port.style.height = new StyleLength(StyleKeyword.Auto);
            Ports.Add(port);


            switch (propertyAttribute.PreferredLocation)
            {
                case PropContainerLocation.InputContainer:
                    inputContainer.Add(port);
                    break;
                case PropContainerLocation.OutputContainer:
                    outputContainer.Add(port);
                    break;
                case PropContainerLocation.ExtensionContainer:
                    extensionContainer.Add(port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Node.AddPortInfo(new PortInfo(
                                 fieldInfo.Name,
                                 Node.ID,
                                 Ports.IndexOf(port),
                                 propertyAttribute.PortDirection
                             ));

            if (propertyAttribute.LabelOnly)
            {
                Label label = new()
                {
                    text = propertyAttribute.OverrideDisplayName != ""
                        ? propertyAttribute.OverrideDisplayName
                        : ObjectNames.NicifyVariableName(
                            fieldInfo.Name),
                    focusable = true,
                };
                port.AddField(label);
            }
            else
            {
                PropertyField tempField = new(prop)
                {
                    name = propertyAttribute.OverrideDisplayName != ""
                        ? propertyAttribute.OverrideDisplayName
                        : ObjectNames.NicifyVariableName(
                            fieldInfo.Name),
                    bindingPath = prop.propertyPath, style =
                    {
                        height = Length.Percent(100), width = Length.Percent(90),
                    },
                    focusable = true,
                };

                if (propertyAttribute.PreferredLocation != PropContainerLocation.ExtensionContainer)
                {
                    tempField.style.minWidth = Length.Pixels(0);
                    tempField.style.maxWidth = Length.Pixels(360);
                }

                port.AddField(tempField);
            }
        }

        /// <summary>
        ///     Returns the m_nodes index of this node
        /// </summary>
        /// <returns></returns>
        private void FetchSerializedProperty()
        {
            _serializedObject.Update();
            SerializedProperty nodes = _serializedObject.FindProperty("_nodes");

            if (!nodes.isArray) return;

            int size = nodes.arraySize;
            for (int i = 0; i < size; i++)
            {
                SerializedProperty element = nodes.GetArrayElementAtIndex(i);
                SerializedProperty elementId = element.FindPropertyRelative("guid");

                if (elementId.stringValue != Node.ID) continue;

                _serializedProperty = element;
                return;
            }
        }

        private void CreateFlowInputPort()
        {
            // Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
            Port inputPort = EditorNodePort.Create<NsEdge>(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi,
                                                           typeof(PortTypes.FlowPort));
            inputPort.portName = "In";
            inputPort.tooltip = "The flow input";
            inputPort.portColor = NodeSystemEditorConsts.PortColor_In;
            RegisterPort(inputPort, PropContainerLocation.InputContainer);
            // m_ports.Add(inputPort);
            // inputContainer.Add(inputPort);
        }

        private void CreateFlowOutputPort()
        {
            // m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            _outputPort = EditorNodePort.Create<NsEdge>(Orientation.Horizontal, Direction.Output, Port.Capacity.Single,
                                                        typeof(PortTypes.FlowPort));
            _outputPort.portName = "Out";
            _outputPort.tooltip = "The flow output";
            _outputPort.portColor = NodeSystemEditorConsts.PortColor_Out;
            RegisterPort(_outputPort, PropContainerLocation.OutputContainer);
            // m_ports.Add(m_outputPort);
            // outputContainer.Add(m_outputPort);
        }

        public void RegisterPort(Port port, PropContainerLocation propContainerLocation)
        {
            Ports.Add(port);
            switch (propContainerLocation)
            {
                case PropContainerLocation.InputContainer:
                    inputContainer.Add(port);
                    break;
                case PropContainerLocation.OutputContainer:
                    outputContainer.Add(port);
                    break;
                case PropContainerLocation.ExtensionContainer:
                    extensionContainer.Add(port);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(propContainerLocation), propContainerLocation, null);
            }
        }

        public void UpdatePosition()
        {
            Node.SetPosition(GetPosition());
        }

        // SHOULD NOT BE USED!!!
        // public virtual Port GetInputPort()
        // {
        //     return m_ports[1];
        // } 
        //
        // public virtual List<Port> GetOutputPorts()
        // {
        //     return new List<Port> { m_ports[0] };
        // }

        public SerializedProperty GetSerializedPropertyOf(string linkedPropertyName)
        {
            if (_serializedProperty == null) FetchSerializedProperty();
            if (_serializedProperty != null) return _serializedProperty.FindPropertyRelative(linkedPropertyName);

            Debug.LogError("Problem with exposed property creation (m_serializedProperty is null)");
            return null;
        }

        public int GetIndexOfPort(Port port)
        {
            return Ports.IndexOf(port);
        }


        #region Editor QOL

        public void OnDoubleClicked(EventBase evt)
        {
            Type nodeType = Node.GetType();
            Debug.Log(nodeType);
            string nodeClass = nodeType.ToString().Split('.').Last();
            string asset = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.EndsWith(nodeClass + ".cs"));
            if (asset == null) return;

            _ = OpenDelayed(asset);
            return;

            async Awaitable OpenDelayed(string className)
            {
                await Awaitable.WaitForSecondsAsync(0.1f);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(className);
                Debug.Log(className);
                if (script == null)
                    Debug.LogWarning($"Couldn't open node of type '{className}'");
                else
                    AssetDatabase.OpenAsset(script);
            }
        }

        #endregion

        // Thanks Unity :clown:
        public void ToggleCollapsed()
        {
            ToggleCollapse();
            RefreshExpandedState();
        }
    }
}