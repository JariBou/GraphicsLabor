using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NodeSystem.Editor.Editors.NodeEditors;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Attributes;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
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
        private static List<Assembly> _assemblies = new ();
        private NodeSystemNode m_graphNode;
        
        private Port m_outputPort;
        
        private List<Port> m_ports;
        
        private SerializedObject m_serializedObject;
        private SerializedProperty m_serializedProperty;
        private int m_indexInNodes = -1;

        public NodeSystemNode Node => m_graphNode;
        public List<Port> Ports => m_ports;
        
        public NodeSystemEditorNode(NodeSystemNode node, SerializedObject serializedObject)
        {
            AddToClassList("code-graph-node");
            
            m_graphNode = node;
            m_ports = new List<Port>();
            
            m_serializedObject = serializedObject;
            
            Type typeInfo = node.GetType();
            NodeInfoAttribute info = typeInfo.GetCustomAttribute<NodeInfoAttribute>();
            // m_graphNode.PureExecutionDone = !info.IsPure;
            m_graphNode.IsPure = info.IsPure;

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
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(CustomNodeEditorAttribute)) && !t.IsAbstract && t.GetCustomAttribute<CustomNodeEditorAttribute>().TargetType == node.GetType()));
                /*
                Assembly.GetAssembly(node.GetType()).GetTypes()
                .Where(t => t.IsDefined(typeof(CustomNodeEditorAttribute)) && !t.IsAbstract && t.GetCustomAttribute<CustomNodeEditorAttribute>().TargetType == node.GetType()); */
            IEnumerable<Type> customEditors = editors as Type[] ?? editors.ToArray();
            
            if (customEditors.Any())
            {
                nodeEditor = Activator.CreateInstance(customEditors.First()) as NodeEditorBase;
            }

            // Output first so always index 0
            if (info.HasFlowOutput)
            {
                if (nodeEditor != null && nodeEditor.AddOutputPorts(this))
                {
                    Debug.Log("Node of type '"+node.GetType()+"' Drawn with custom Editor");
                    // nodeEditor.AddOutputPorts(this);
                }
                else
                {
                    for (int i = 0; i < info.OutputPortCount; i++)
                    {
                        CreateFlowOutputPort();
                    }
                }
            }
            
            if (info.HasFlowInput)
            {
                if (nodeEditor != null && nodeEditor.AddInputPorts(this))
                {
                    // nodeEditor.AddInputPorts(this);
                }
                else
                {
                    CreateFlowInputPort();
                }
            }

            CreateExposedVariables(typeInfo);
            
            RefreshExpandedState();
            
            // this.AddManipulator(new Clickable(OnClicked));
        }

        private void CreateExposedVariables(Type typeInfo) 
        {
            m_graphNode.PortInfos.Clear();
            foreach (FieldInfo fieldInfo in typeInfo.GetFields())
            {
                if (fieldInfo.GetCustomAttribute<ExposedPropertyAttribute>() is { } propertyAttribute)
                {
                    if (!propertyAttribute.HasOutPort && !propertyAttribute.HasInPort) continue;
                    
                    SerializedProperty prop = GetSerializedPropertyOf(fieldInfo.Name);
                    
                    if (prop == null) continue;
                        
                    Direction portDirection = propertyAttribute.HasOutPort ? Direction.Output : Direction.Input;
                    Type propertyType;
                    try
                    {
                        // Can sometimes crash, like when using string isntead of String idk why
                        propertyType = propertyAttribute.AutoTyping ? fieldInfo.FieldType /*TypeUtils.GetPropertyType(prop)*/ : propertyAttribute.PortType;
                        // Debug.LogWarning(propertyType);
                    }
                    catch (Exception e)
                    {
                        propertyType = propertyAttribute.PortType;
                        Console.WriteLine(e);
                    }

                    propertyType ??= fieldInfo.FieldType;
                        
                    EditorNodePort port = EditorNodePort.Create(Orientation.Horizontal, portDirection, propertyAttribute.PortCapacity == PropPortCapacity.Single ? Port.Capacity.Single : Port.Capacity.Multi, propertyType);
                    // port.contentContainer.Add(tempField);
                    port.HideWhenConnected = propertyAttribute.DisableInputWhenConnected;
                    port.LinkedPropertyName = fieldInfo.Name;
                    port.portName = "";
                    port.tooltip = propertyType.ToString();
                    m_ports.Add(port);

                    // port.style.height = EditorGUI.GetPropertyHeight(prop);
                    port.style.height = new StyleLength(StyleKeyword.Auto);
                    port.style.width = Length.Percent(100);
                    
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
                    m_graphNode.AddPortInfo(new PortInfo(
                        fieldInfo.Name,
                        m_graphNode.id,
                        m_ports.IndexOf(port),
                        propertyAttribute.PortDirection
                    ));

                    if (propertyAttribute.LabelOnly)
                    {
                        Label label = new()
                        {
                            text = propertyAttribute.OverrideDisplayName != "" ? propertyAttribute.OverrideDisplayName : ObjectNames.NicifyVariableName(
                                fieldInfo.Name),
                            focusable = true,
                            
                        };
                        port.AddField(label);
                    }
                    else if (propertyType == typeof(SerializableRef))
                    {
                        SerializableRef sr = (SerializableRef)fieldInfo.GetValue(m_graphNode);
                        sr.RefTypename = propertyType.AssemblyQualifiedName;
                        ObjectField objectField = new ObjectField
                        {
                            objectType = propertyType,
                            // Ref<Object> objectFieldValue =  (Ref<Object>)prop.objectReferenceValue;
                            // objectFieldValue.ObjectId:
                            value = sr.Get<GameObject>(),
                            focusable = true,
                        };

                        objectField.RegisterValueChangedCallback(evt =>
                        {
                            //Ref<Object> nodeRefVal = (Ref<Object>)fieldInfo.GetValue(m_graphNode);
                            sr.ObjectId = ReferenceManager.GetGuidOf(evt.newValue);
                            fieldInfo.SetValue(m_graphNode, sr);
                        });
                        port.AddField(objectField);
                        // Type genericTypeDefinition = propertyType.GetGenericTypeDefinition();
                        // if (genericTypeDefinition == typeof(Ref<>))
                        // {
                        //     Type refArgument = propertyType.GetGenericArguments()[0];
                        //     
                        //     ObjectField objectField = new ObjectField()
                        //     {
                        //         objectType = refArgument.GetType(),
                        //     };
                        //     Debug.Log("Is Subclass of Ref<> and ref arg='" + refArgument.GetType() + "'");
                        //     Debug.Log(prop.name);
                        //     Debug.Log(prop.objectReferenceValue);
                        //
                        //     Ref<Object> objectFieldValue =  (Ref<Object>)prop.objectReferenceValue;
                        //     // objectFieldValue.ObjectId:
                        //     objectField.value = objectFieldValue == null ? null : objectFieldValue.Get();
                        //     objectField.RegisterValueChangedCallback(evt =>
                        //     {
                        //         // SerializedProperty propFunc = GetSerializedPropertyOf(property.Name);
                        //         // Debug.Log(evt.newValue);
                        //         // Debug.Log(prop.serializedObject.targetObject);
                        //         // GetSerializedPropertyOf(property.Name).stringValue = ReferenceManager.Instance.GetGuidOf(evt.newValue);
                        //         // property.SetValue(m_serializedObject.FindProperty("m_nodes").GetArrayElementAtIndex(m_indexInNodes).serializedObject.targetObject, ReferenceManager.Instance.GetGuidOf(evt.newValue));
                        //         Ref<Object> nodeRefVal = (Ref<Object>)fieldInfo.GetValue(m_graphNode);
                        //         nodeRefVal.ObjectId = ReferenceManager.GetGuidOf(evt.newValue);
                        //         fieldInfo.SetValue(m_graphNode, nodeRefVal);
                        //     });
                        //     port.AddField(objectField);
                        //     port.portType = refArgument.GetType();
                        // }
                    }
                    else
                    {
                        PropertyField tempField = new(prop)
                        {
                            name = propertyAttribute.OverrideDisplayName != "" ? propertyAttribute.OverrideDisplayName : ObjectNames.NicifyVariableName(
                                fieldInfo.Name),
                            bindingPath = prop.propertyPath,
                            style =
                            {
                                height = Length.Percent(100),
                                width = Length.Percent(100), // TODO: IMPORTANT
                            },
                            focusable = true,
                        };
                        port.AddField(tempField);
                    }
                        
                    continue;


                    // PropertyField field = DrawProperty(propertyAttribute.OverrideDisplayName != "" ? propertyAttribute.OverrideDisplayName : property.Name);
                    //field.RegisterValueChangeCallback(OnFieldChangedCallback);
                }
                else if (fieldInfo.GetCustomAttribute<SourcePropertyAttribute>() is { } sourceAttribute)
                {
                    
                    SerializedProperty prop = GetSerializedPropertyOf(fieldInfo.Name);
                    
                    if (prop == null) continue;

                    // ReferenceManager referenceManager = GameObject.FindGameObjectWithTag("ReferenceManager").GetComponent<ReferenceManager>();

                    ObjectField objectField = new ObjectField
                    {
                        objectType = sourceAttribute.SourceType,
                        // ReferenceManager.Instance.TestPrint();
                        value = ReferenceManager.GetGameObject<GameObject>(prop.stringValue),
                        focusable = true,
                    };

                    objectField.RegisterValueChangedCallback(evt =>
                    {
                        // SerializedProperty propFunc = GetSerializedPropertyOf(property.Name);
                        // Debug.Log(evt.newValue);
                        // Debug.Log(prop.serializedObject.targetObject);
                        // GetSerializedPropertyOf(property.Name).stringValue = ReferenceManager.Instance.GetGuidOf(evt.newValue);
                        // property.SetValue(m_serializedObject.FindProperty("m_nodes").GetArrayElementAtIndex(m_indexInNodes).serializedObject.targetObject, ReferenceManager.Instance.GetGuidOf(evt.newValue));
                        fieldInfo.SetValue(m_graphNode, ReferenceManager.GetGuidOf(evt.newValue));
                    });
                    
                    extensionContainer.Add(objectField);
                }
            }
            RefreshPorts();
        }

        private void OnFieldChangedCallback(SerializedPropertyChangeEvent evt)
        {
            
        }

        private void Test()
        {
            int index = FetchSerializedProperty();
            
        }

        /// <summary>
        /// Returns the m_nodes index of this node 
        /// </summary>
        /// <returns></returns>
        private int FetchSerializedProperty()
        {
            m_serializedObject.Update();
            SerializedProperty nodes = m_serializedObject.FindProperty("m_nodes");
            if (nodes.isArray)
            {
                int size = nodes.arraySize;
                for (int i = 0; i < size; i++)
                {
                    SerializedProperty element = nodes.GetArrayElementAtIndex(i);
                    SerializedProperty elementId = element.FindPropertyRelative("m_guid");
                    if (elementId.stringValue == m_graphNode.id)
                    {
                        m_serializedProperty = element;
                        m_indexInNodes = i;
                        return i;
                    }
                }
            }
            return -1;
           //throw new NullReferenceException();
        }

        private PropertyField DrawProperty(string propertyName)
        {
            if (m_serializedProperty == null)
            {
                FetchSerializedProperty();
            }
            if (m_serializedProperty == null)
            {
                Debug.LogError("Problem with exposed property creation (m_serializedProperty is null)");
                return null;
            }
            SerializedProperty prop = m_serializedProperty.FindPropertyRelative(propertyName);
            PropertyField field = new PropertyField(prop)
            {
                bindingPath = prop.propertyPath
            };
            extensionContainer.Add(field);
            return field;
        }

        private void CreateFlowInputPort()
        {
            Port inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(PortTypes.FlowPort));
            inputPort.portName = "In";
            inputPort.tooltip = "The flow input";
            RegisterPort(inputPort, PropContainerLocation.InputContainer);
            // m_ports.Add(inputPort);
            // inputContainer.Add(inputPort);
        }

        private void CreateFlowOutputPort()
        {
            m_outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(PortTypes.FlowPort));
            m_outputPort.portName = "Out";
            m_outputPort.tooltip = "The flow output";
            RegisterPort(m_outputPort, PropContainerLocation.OutputContainer);
            // m_ports.Add(m_outputPort);
            // outputContainer.Add(m_outputPort);
        }

        public void RegisterPort(Port port, PropContainerLocation propContainerLocation)
        {
            m_ports.Add(port);
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
            m_graphNode.SetPosition(GetPosition());
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
            if (m_serializedProperty == null)
            {
                FetchSerializedProperty();
            }
            if (m_serializedProperty == null)
            {
                Debug.LogError("Problem with exposed property creation (m_serializedProperty is null)");
                return null;
            }
                        
            return m_serializedProperty.FindPropertyRelative(linkedPropertyName);
        }


        #region Editor QOL

        private float _lastClickedTime;
        public void OnClicked()
        {

            // F ME
            float now = Time.time;
            
            float diff = now - _lastClickedTime;
            Debug.Log(diff);
            if (diff < 0.1f)
            {
                Type nodeType = m_graphNode.GetType();
                Debug.Log(nodeType);
                string nodeClass = nodeType.ToString().Split('.').Last();
                string asset = AssetDatabase.GetAllAssetPaths().FirstOrDefault(p => p.EndsWith(nodeClass + ".cs"));
                if (asset != null)
                {
                    async Awaitable OpenDelayed(string className)
                    {
                        await Awaitable.WaitForSecondsAsync(0.1f);
                        MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(className);
                        Debug.Log(className);
                        if (script == null)
                        {
                            Debug.LogWarning($"Couldn't open node of type '{className}'");
                        }
                        else
                        {
                            AssetDatabase.OpenAsset(script);
                        }
                    }
                    
                    _ = OpenDelayed(asset);
                }
            }
            
            _lastClickedTime = now;
        }

        #endregion

    }
}