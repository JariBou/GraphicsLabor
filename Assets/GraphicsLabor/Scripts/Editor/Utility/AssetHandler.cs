using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Editor.ScriptableObjectParents;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class AssetHandler
    {
        [OnOpenAsset]
        private static bool OpenEditor(int instanceID, int line)
        {
            // Also works with user Custom ScriptableObjects that do not inherit from one of the SO parents provided
            Object obj = EditorUtility.InstanceIDToObject(instanceID);

            // Not handled by GraphicsLabor
            // if (!obj.InheritsFrom(typeof(ScriptableObject))) return false;
            
            // Editable takes priority over Manager

            Debug.Log(obj.InheritsFrom(typeof(ScriptableObject)) ? "It should be fine" : "Problem");

            IEnumerable<CustomAttributeData> objectCustomAttributes = ReflectionUtility.GetAllAttributesOfObject(obj,
                data => data.AttributeType.IsSubclassOf(typeof(ScriptableObjectAttribute)), true).ToList();
            

            List<Type> attributeTypes = objectCustomAttributes.Select(data => data.AttributeType).ToList();

            if (attributeTypes.Count == 0)
            {
                return false;
            }
            
            if (attributeTypes.Contains(typeof(EditableAttribute)))
            {
                ScriptableObjectEditorWindow.ShowWindow(obj);
                return true;
            } else if (attributeTypes.Contains(typeof(ScriptableObjectManager)))
            {
                // Coming soon...        
            }

            // Not handled by GraphicsLabor
            return false;
        }
    }
}