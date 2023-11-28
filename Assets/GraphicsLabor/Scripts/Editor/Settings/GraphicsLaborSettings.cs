using System.Collections.Generic;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using UnityEditorInternal;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Settings
{
    [Editable]
    public class GraphicsLaborSettings : ScriptableObject
    {
        public string testString;
        [Tooltip("Can contain up to 32 custom tags")] public List<string> Tags;
        public List<AssemblyDefinitionAsset> Assemblies;

        // private void OnValidate()
        // {
        //     if (Tags.Count > 64)
        //     {
        //         Tags.RemoveAt(Tags.Count-1);
        //     }
        // }
    }
}