using System.Collections;
using System.Linq;
using System.Reflection;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Editor.Utility;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GraphicsLabor.Scripts.Editor.CustomInspector
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
                        if (stage != null)
                        {
                            // Prefab mode
                            EditorSceneManager.MarkSceneDirty(stage.scene);
                        }
                        else
                        {
                            // Normal scene
                            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                        }
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

        public static void PropertyField(SerializedProperty property)
        {
            if (PropertyUtility.IsVisible(property))
            {
                using (new EditorGUI.DisabledScope(disabled: false))
                {
                    EditorGUILayout.PropertyField(property, PropertyUtility.GetLabel(property), true);
                }
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