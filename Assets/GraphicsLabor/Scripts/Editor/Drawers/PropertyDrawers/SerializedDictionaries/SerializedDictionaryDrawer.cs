using System.Collections.Generic;
using GraphicsLabor.Scripts.Core.Utility;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Editor.Drawers.PropertyDrawers.SerializedDictionaries
{
    [CustomPropertyDrawer(typeof(SerializedDictionary<,>))]
    public class SerializedDictionaryDrawer : PropertyDrawerBase
    {
        private readonly Dictionary<string, SerializedDictionaryDrawerInstance> _drawers = new ();
        
        protected override void OnSelfGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            GetDrawerInstanceFor(property).DoGUI(rect);
        }

        protected override float GetSelfPropertyHeight(SerializedProperty property, GUIContent label)
        {
           
            return GetDrawerInstanceFor(property).GetHeight();
        }

        private SerializedDictionaryDrawerInstance GetDrawerInstanceFor(SerializedProperty property)
        {
            if (!_drawers.ContainsKey(property.propertyPath))
            {
                _drawers[property.propertyPath] = new SerializedDictionaryDrawerInstance(property);
            }
            return _drawers[property.propertyPath];
        }
    }
}