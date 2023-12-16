using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Settings
{
    public class GraphicsLaborSettings : ScriptableObject
    {
        [ShowMessage("Path where \"Buffer\" Manageable Scriptable objects will be created for the Scriptable Objects Creator", MessageType.Info)]
        [Label("Buffer SO Path")]public string _tempScriptableObjectsPath = "Assets/Config/GraphicsLabor/ScriptableObjects";
        [Label("Tags Path")]public string _tagsPath = "Assets/Config/GraphicsLabor/LaborerTags";
        [Tooltip("Can contain up to 32 custom tags")] public List<string> _tags;
        
        private void OnValidate()
        {
            if (_tags.Count > 32)
            {
                _tags.RemoveAt(_tags.Count-1);
            }
        }
    }
}