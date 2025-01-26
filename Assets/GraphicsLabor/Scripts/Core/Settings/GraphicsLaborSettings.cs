using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Core.Utility;
using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Settings
{
    public class GraphicsLaborSettings : ScriptableObject
    {
        [Label("Buffer SO Path"), ReadOnly] public string _tempScriptableObjectsPath = "Assets/GraphicsLabor/Generated/ScriptableObjects"; 
        [Label("Tags Path"), ReadOnly] public string _tagsPath = "Assets/GraphicsLabor/Scripts/Core/Tags"; // For now let it be default, will see if there is any use to modifying its location
        [Label("Default Enum Path")] public string _defaultEnumsPath = "Assets/GraphicsLabor/Generated/Enums"; 
        [Tooltip("Can contain up to 31 custom tags, an object can hold multiple LaborerTags")] public List<string> _tags;
        public List<EnumSettings> _enums;
        public SerializedDictionary<string, Test> _test2;
        [SerializedDictionarySettings(keyName: "Zuckerberg", valueName: "This a value")]public SerializedDictionary<string, List<string>> _test21;
        public List<SerializedDictionary<string, Test>> _test3;
        public TestWrapper _test4;
        
        private void OnValidate()
        {
            if (_tags.Count > 31)
            {
                _tags.RemoveAt(_tags.Count-1);
            }
        }
    }

    [Serializable]
    public class TestWrapper
    {
        public SerializedDictionary<string, Test> _test;
    }

    [Serializable]
    public class Test
    {
        public string _test;
        public int _test2;
        public bool _test3;
    }
    
    [Serializable]
    public class EnumSettings
    {
        [SerializeField] private string _enumClassName;
        [SerializeField, Tooltip("Leave blank for no namespace")] private string _enumSpace;
        [SerializeField] private bool _isFlag;
        [SerializeField] private SerializedDictionary<string, int> _enumNamesAndValues;
            
        public string EnumClassName => _enumClassName;
        public string EnumSpace => _enumSpace;
        public bool IsFlag => _isFlag;
        public SerializedDictionary<string, int> EnumNames => _enumNamesAndValues;
            
        public EnumSettings(SerializedDictionary<string, int> enumNames, string enumClassName, bool isFlag = false, string enumSpace = null)
        {
            _enumNamesAndValues = enumNames;
            _enumClassName = enumClassName;
            _isFlag = isFlag;
            _enumSpace = enumSpace;
        }
        
        public EnumSettings(Dictionary<string, int> enumNames, string enumClassName, bool isFlag = false, string enumSpace = null)
        {
            _enumNamesAndValues = new SerializedDictionary<string, int>(enumNames);
            _enumClassName = enumClassName;
            _isFlag = isFlag;
            _enumSpace = enumSpace;
        }
    }
}