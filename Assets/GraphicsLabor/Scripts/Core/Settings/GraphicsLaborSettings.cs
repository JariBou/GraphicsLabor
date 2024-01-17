using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Settings
{
    [Editable]
    public class GraphicsLaborSettings : ScriptableObject
    {
        // [ShowMessage("Path where \"Buffer\" Manageable Scriptable objects will be created for the Scriptable Objects Creator", MessageLevel.Info)]
        // No more use to allow modifying this since we won't support installing via git, too much work for little use
        [Label("Buffer SO Path"), ReadOnly] public string _tempScriptableObjectsPath = "Assets/GraphicsLabor/Generated/ScriptableObjects"; 
        [Label("Tags Path"), ReadOnly] public string _tagsPath = "Assets/GraphicsLabor/Scripts/Core/Tags"; // For now let it be default, will see if there is any use to modifying its location
        [Tooltip("Can contain up to 31 custom tags, an object can hold multiple LaborerTags")] public List<string> _tags;
        
        private void OnValidate()
        {
            if (_tags.Count > 31)
            {
                _tags.RemoveAt(_tags.Count-1);
            }
        }
    }
}