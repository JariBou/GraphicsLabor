using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace GraphicsLabor.Scripts.Editor.Utility
{
    public static class LaborerEditorGUI
    {
        private static readonly GUIStyle ButtonStyle = new GUIStyle(GUI.skin.button) { richText = true };

        public static void Button(Object target, MethodInfo methodInfo)
        {
            if (methodInfo.GetParameters().All(p => p.IsOptional))
            {
                ButtonAttribute buttonAttribute =
                    (ButtonAttribute)methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
                
                // TODO: see if show if Attribute can be used here
                
                string buttonText = string.IsNullOrEmpty(buttonAttribute.Text)
                    ? ObjectNames.NicifyVariableName(methodInfo.Name)
                    : buttonAttribute.Text;

                EButtonEnableMode mode = buttonAttribute.SelectedEnableMode;
                bool buttonEnabled = true;
                buttonEnabled &=
                    mode == EButtonEnableMode.Always ||
                    mode == EButtonEnableMode.Editor && !Application.isPlaying ||
                    mode == EButtonEnableMode.Playmode && Application.isPlaying;

                bool methodIsCoroutine = methodInfo.ReturnType == typeof(IEnumerator);
                if (methodIsCoroutine)
                {
                    buttonEnabled &= Application.isPlaying;
                }
                EditorGUI.BeginDisabledGroup(!buttonEnabled);

                if (GUILayout.Button(buttonText, ButtonStyle))
                {
                    object[] defaultParams = methodInfo.GetParameters().Select(p => p.DefaultValue).ToArray();
                    IEnumerator methodResult = (IEnumerator)methodInfo.Invoke(target, defaultParams);

                    if (!Application.isPlaying)
                    {
                        // Set target object and scene dirty to serialize changes to disk
                        // If not it just goes into the void basically
                        EditorUtility.SetDirty(target);

                        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                        // Normal scene
                        // Prefab mode
                        EditorSceneManager.MarkSceneDirty(stage ? stage.scene : SceneManager.GetActiveScene());
                    }
                    else if (methodResult != null && target is MonoBehaviour behaviour)
                    {
                        behaviour.StartCoroutine(methodResult);
                    }
                }

                EditorGUI.EndDisabledGroup();
            }
            else
            {
                const string warning = nameof(ButtonAttribute) + " works only on methods with no parameters";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }
        }

        #region Scriptable Objects

        public static void EditableSoButton(Object target, string buttonText)
        {

            EditorGUI.BeginDisabledGroup(false);

            if (GUILayout.Button(buttonText, ButtonStyle))
            {
                ScriptableObjectEditorWindow.ShowWindow(target);
            }

            EditorGUI.EndDisabledGroup();
        }
        
        public static void ManageableSoButton(Object target, string buttonText)
        {

            EditorGUI.BeginDisabledGroup(false);

            if (GUILayout.Button(buttonText, ButtonStyle))
            {
                ScriptableObjectCreatorWindow.ShowWindow(target);
            }

            EditorGUI.EndDisabledGroup();
        }

        #endregion

        #region Fields

        public static void PropertyField(Rect rect, SerializedProperty property, bool includeChildren)
        {
            // Check if visible
            if (!PropertyUtility.IsVisible(property)) return;
            
            bool enabled = PropertyUtility.IsEnabled(property);

            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                EditorGUI.PropertyField(rect, property, PropertyUtility.GetLabel(property), includeChildren);
            }
        }

        public static void LayoutPropertyField(SerializedProperty property)
        {
            if (!PropertyUtility.IsVisible(property)) return;
            
            bool isEnabled = PropertyUtility.IsEnabled(property);
                
            using (new EditorGUI.DisabledScope(disabled: !isEnabled))
            {
                EditorGUILayout.PropertyField(property, PropertyUtility.GetLabel(property), true);
            }
        }

        #endregion

        #region Properties
        
        public static void LayoutProperty(SerializedObject serializedObject, PropertyInfo property)
        {
            
            bool isVisible = PropertyUtility.IsVisible(property, serializedObject);
            
            if (!isVisible) return;

            bool isEnabled = PropertyUtility.IsEnabled(property, serializedObject);


            if (property.CanWrite)
            {
                if (DrawWritableField(serializedObject.targetObject, property, isEnabled)) return;
                
                string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            } else if (!DrawNonWritableField(serializedObject.targetObject, property, isEnabled))
            {
                string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
            } 
            
        }
        public static bool DrawWritableField(Object targetObject, PropertyInfo property, bool enabled = false)
        {
            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                string label = ObjectNames.NicifyVariableName(property.Name);
                bool isDrawn = true;
                object value = property.GetValue(targetObject, null);
                
                if (value == null)
                {
                    string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    return false;
                }
                
                Type valueType = value.GetType();

                if (valueType == typeof(bool))
                {
                    property.SetValue(targetObject,EditorGUILayout.Toggle(label, (bool)value));
                }
                else if (valueType == typeof(short))
                {
                    property.SetValue(targetObject,EditorGUILayout.IntField(label, (short)value));
                }
                else if (valueType == typeof(ushort))
                {
                    property.SetValue(targetObject,EditorGUILayout.IntField(label, (ushort)value));
                }
                else if (valueType == typeof(int))
                {
                    property.SetValue(targetObject,EditorGUILayout.IntField(label, (int)value));
                }
                else if (valueType == typeof(uint))
                {
                    property.SetValue(targetObject,EditorGUILayout.LongField(label, (uint)value));
                }
                else if (valueType == typeof(long))
                {
                    property.SetValue(targetObject,EditorGUILayout.LongField(label, (long)value));
                }
                else if (valueType == typeof(ulong))
                {
                    property.SetValue(targetObject,EditorGUILayout.TextField(label, ((ulong)value).ToString()));
                }
                else if (valueType == typeof(float))
                {
                    property.SetValue(targetObject,EditorGUILayout.FloatField(label, (float)value));
                }
                else if (valueType == typeof(double))
                {
                    property.SetValue(targetObject,EditorGUILayout.DoubleField(label, (double)value));
                }
                else if (valueType == typeof(string))
                {
                    property.SetValue(targetObject,EditorGUILayout.TextField(label, (string)value));
                }
                else if (valueType == typeof(Vector2))
                {
                    property.SetValue(targetObject,EditorGUILayout.Vector2Field(label, (Vector2)value));
                }
                else if (valueType == typeof(Vector3))
                {
                    property.SetValue(targetObject,EditorGUILayout.Vector3Field(label, (Vector3)value));
                }
                else if (valueType == typeof(Vector4))
                {
                    property.SetValue(targetObject,EditorGUILayout.Vector4Field(label, (Vector4)value));
                }
                else if (valueType == typeof(Vector2Int))
                {
                    property.SetValue(targetObject,EditorGUILayout.Vector2IntField(label, (Vector2Int)value));
                }
                else if (valueType == typeof(Vector3Int))
                {
                    property.SetValue(targetObject,EditorGUILayout.Vector3IntField(label, (Vector3Int)value));
                }
                else if (valueType == typeof(Color))
                {
                    property.SetValue(targetObject,EditorGUILayout.ColorField(label, (Color)value));
                }
                else if (valueType == typeof(Bounds))
                {
                    property.SetValue(targetObject,EditorGUILayout.BoundsField(label, (Bounds)value));
                }
                else if (valueType == typeof(Rect))
                {
                    property.SetValue(targetObject,EditorGUILayout.RectField(label, (Rect)value));
                }
                else if (valueType == typeof(RectInt))
                {
                    property.SetValue(targetObject,EditorGUILayout.RectIntField(label, (RectInt)value));
                }
                else if (typeof(Object).IsAssignableFrom(valueType))
                {
                    property.SetValue(targetObject,EditorGUILayout.ObjectField(label, (Object)value, valueType, true));
                }
                else if (valueType.BaseType == typeof(Enum))
                {
                    property.SetValue(targetObject,EditorGUILayout.EnumPopup(label, (Enum)value));
                }
                else if (valueType.BaseType == typeof(TypeInfo))
                {
                    property.SetValue(targetObject,EditorGUILayout.TextField(label, value.ToString()));
                }
                else
                {
                    isDrawn = false;
                }

                return isDrawn;
            }
        }
        public static bool DrawNonWritableField(Object targetObject, PropertyInfo property, bool enabled = false)
        {
            using (new EditorGUI.DisabledScope(disabled: !enabled))
            {
                bool isDrawn = true;
                string label = ObjectNames.NicifyVariableName(property.Name);
                object value = property.GetValue(targetObject, null);

                if (value == null)
                {
                    string warning = $"{nameof(ShowPropertyAttribute)} doesn't support {property.PropertyType.Name} types";
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
                    return false;
                }
                
                Type valueType = value.GetType();

                if (valueType == typeof(bool))
                {
                    EditorGUILayout.Toggle(label, (bool)value);
                }
                else if (valueType == typeof(short))
                {
                    EditorGUILayout.IntField(label, (short)value);
                }
                else if (valueType == typeof(ushort))
                {
                    EditorGUILayout.IntField(label, (ushort)value);
                }
                else if (valueType == typeof(int))
                {
                    EditorGUILayout.IntField(label, (int)value);
                }
                else if (valueType == typeof(uint))
                {
                    EditorGUILayout.LongField(label, (uint)value);
                }
                else if (valueType == typeof(long))
                {
                    EditorGUILayout.LongField(label, (long)value);
                }
                else if (valueType == typeof(ulong))
                {
                    EditorGUILayout.TextField(label, ((ulong)value).ToString());
                }
                else if (valueType == typeof(float))
                {
                    EditorGUILayout.FloatField(label, (float)value);
                }
                else if (valueType == typeof(double))
                {
                    EditorGUILayout.DoubleField(label, (double)value);
                }
                else if (valueType == typeof(string))
                {
                    EditorGUILayout.TextField(label, (string)value);
                }
                else if (valueType == typeof(Vector2))
                {
                    EditorGUILayout.Vector2Field(label, (Vector2)value);
                }
                else if (valueType == typeof(Vector3))
                {
                    EditorGUILayout.Vector3Field(label, (Vector3)value);
                }
                else if (valueType == typeof(Vector4))
                {
                    EditorGUILayout.Vector4Field(label, (Vector4)value);
                }
                else if (valueType == typeof(Vector2Int))
                {
                    EditorGUILayout.Vector2IntField(label, (Vector2Int)value);
                }
                else if (valueType == typeof(Vector3Int))
                {
                    EditorGUILayout.Vector3IntField(label, (Vector3Int)value);
                }
                else if (valueType == typeof(Color))
                {
                    EditorGUILayout.ColorField(label, (Color)value);
                }
                else if (valueType == typeof(Bounds))
                {
                    EditorGUILayout.BoundsField(label, (Bounds)value);
                }
                else if (valueType == typeof(Rect))
                {
                    EditorGUILayout.RectField(label, (Rect)value);
                }
                else if (valueType == typeof(RectInt))
                {
                    EditorGUILayout.RectIntField(label, (RectInt)value);
                }
                else if (typeof(Object).IsAssignableFrom(valueType))
                {
                    EditorGUILayout.ObjectField(label, (Object)value, valueType, true);
                }
                else if (valueType.BaseType == typeof(Enum))
                {
                    EditorGUILayout.EnumPopup(label, (Enum)value);
                }
                else if (valueType.BaseType == typeof(TypeInfo))
                {
                    EditorGUILayout.TextField(label, value.ToString());
                }
                else
                {
                    isDrawn = false;
                }

                return isDrawn;
            }
        }

        #endregion
        
        public static void HorizontalLine(Rect rect, float height, Color color)
        {
            rect.height = height;
            EditorGUI.DrawRect(rect, color);
        }
        
        public static void HelpBox(Rect rect, string message, MessageType type, Object context = null, bool logToConsole = false)
        {
            EditorGUI.HelpBox(rect, message, type);

            if (!logToConsole) return;
            
            switch (type)
            {
                case MessageType.None:
                case MessageType.Info:
                    Debug.Log(message, context);
                    break;
                case MessageType.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case MessageType.Error:
                    Debug.LogError(message, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static float GetIndentLength(Rect sourceRect)
        {
            Rect indentRect = EditorGUI.IndentedRect(sourceRect);
            float indentLength = indentRect.x - sourceRect.x;

            return indentLength;
        }
    }
}