using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Editor.Utility.Reflection;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEditor;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class AssetHandler
    {
        [OnOpenAsset]
        private static bool OpenEditor(int instanceID, int line)
        {
            Object obj = EditorUtility.InstanceIDToObject(instanceID);
            
            IEnumerable<CustomAttributeData> objectCustomAttributes = ReflectionUtility.GetAllAttributesOfObject(obj,
                data => data.AttributeType.IsSubclassOf(typeof(ScriptableObjectAttribute)), true).ToList();
            
            List<Type> attributeTypes = objectCustomAttributes.Select(data => data.AttributeType).ToList();

            if (attributeTypes.Count == 0)
            {
                // Not handled by GraphicsLabor
                return false;
            }
            
            if (attributeTypes.Contains(typeof(EditableAttribute)))
            {
                ScriptableObjectEditorWindow.ShowWindow(obj);
                return true;
            }

            // Not handled by GraphicsLabor
            return false;
        }

    }
}