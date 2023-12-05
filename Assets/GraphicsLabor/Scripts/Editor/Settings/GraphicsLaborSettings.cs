using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Serialization;

namespace GraphicsLabor.Scripts.Editor.Settings
{
    [Editable]
    public class GraphicsLaborSettings : ScriptableObject
    {
        public string testString;
        [Tooltip("Can contain up to 32 custom tags")] public List<string> Tags;
        public List<AssemblyDefinitionAsset> UserAssemblies;

        private List<Assembly> _assemblies;

        
        private void OnValidate()
        {
            if (Tags.Count > 32)
            {
                Tags.RemoveAt(Tags.Count-1);
            }
        }
    }
}