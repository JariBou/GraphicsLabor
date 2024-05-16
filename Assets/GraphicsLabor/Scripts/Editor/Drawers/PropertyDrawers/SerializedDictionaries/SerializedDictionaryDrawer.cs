using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers.PropertyDrawers.SerializedDictionaries
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public class SerializedDictionaryDrawer : PropertyDrawerBase
    {
        private Dictionary<string, SerializedDictionaryDrawerInstance> _drawers = new ();
        
        protected override void OnSelfGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            GetInstanceFor(property).DoGUI(rect, label);
        }

        protected override float GetSelfPropertyHeight(SerializedProperty property, GUIContent label)
        {
           
            return GetInstanceFor(property).GetHeight();
        }

        private SerializedDictionaryDrawerInstance GetInstanceFor(SerializedProperty property)
        {
            if (!_drawers.ContainsKey(property.propertyPath))
            {
                _drawers[property.propertyPath] = new SerializedDictionaryDrawerInstance(property);
            }
            return _drawers[property.propertyPath];
        }
    }
}