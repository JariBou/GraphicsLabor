using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
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
        
        private void OnValidate()
        {
            if (_tags.Count > 31)
            {
                _tags.RemoveAt(_tags.Count-1);
            }
        }
    }
    
    [Serializable]
    public class EnumSettings
    {
        [SerializeField] private string _enumClassName;
        [SerializeField, Tooltip("Leave blank for no namespace")] private string _enumSpace;
        [SerializeField] private bool _isFlag;
        [SerializeField] private List<string> _enumNames;
            
        public string EnumClassName => _enumClassName;
        public string EnumSpace => _enumSpace;
        public bool IsFlag => _isFlag;
        public List<string> EnumNames => _enumNames;
            
        public EnumSettings(List<string> enumNames, string enumClassName, bool isFlag = false, string enumSpace = null)
        {
            _enumNames = enumNames;
            _enumClassName = enumClassName;
            _isFlag = isFlag;
            _enumSpace = enumSpace;
        }
    }
}