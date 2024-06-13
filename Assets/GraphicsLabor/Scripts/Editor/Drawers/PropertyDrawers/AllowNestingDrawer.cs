using System.Linq;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using GraphicsLabor.Scripts.Core.Utility;
using GraphicsLabor.Scripts.Editor.Utility.GUI;
using GraphicsLabor.Scripts.Editor.Utility.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Drawers.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(AllowNestingAttribute))]
    public class AllowNestingDrawer : PropertyDrawerBase
    {
        protected override void OnSelfGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            //EditorGUI.PropertyField(rect, property, label, true);
            LaborerEditorGUI.PropertyField(rect, property, true);

            EditorGUI.EndProperty();
        }
    }
}