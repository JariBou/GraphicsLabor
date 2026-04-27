using System;
using NodeSystem.Editor.Editors.Fields;
using NodeSystem.Editor.Editors.RefEditors.SearchProviders;
using NodeSystem.Runtime.Core.RefSystem;
using NodeSystem.Runtime.Extensions;
using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Search;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Editors.RefEditors
{
    [CustomPropertyDrawer(typeof(SerializableCompRef<>))]
    public class SerializableCompRefEditor : PropertyDrawer
    {
        private const SearchViewFlags SearchViewFlags = UnityEngine.Search.SearchViewFlags.Borderless |
                                                        UnityEngine.Search.SearchViewFlags.GridView |
                                                        UnityEngine.Search.SearchViewFlags.DisableSavedSearchQuery |
                                                        UnityEngine.Search.SearchViewFlags.OpenInspectorPreview;

        private float _cellHeight;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color colorSave = GUI.color;
            GUI.color = Color.lightCyan;
            GUI.Box(position, GUIContent.none);
            GUI.color = colorSave;

            property.serializedObject.Update();
            ISerializableTypedRef typedRef = (ISerializableTypedRef)property.boxedValue;

            EditorGUI.BeginProperty(position, label, property);
            float cellWidth = position.width / 3;

            {
                Rect nameRect = new()
                {
                    x = position.x, y = position.y, width = cellWidth, height = EditorGUIUtility.singleLineHeight,
                };

                EditorGUI.LabelField(nameRect, label);

                Rect typeRect = new()
                {
                    x = position.x + cellWidth, y = position.y, width = cellWidth * 2, height = EditorGUIUtility.singleLineHeight,
                };
                GUIContent typeGuiContent = new($"Type: SerializableCompRef<{typedRef.GetRefType().Name}>");
                Color color = GUI.color;
                GUI.color = Color.orange;
                EditorGUI.LabelField(typeRect, typeGuiContent);
                GUI.color = color;
            }

            SerializedProperty ownerIdProp = property.FindPropertyRelative("_ownerId");
            SerializedProperty compIdProp = property.FindPropertyRelative("_compId");
            Rect ownerLabelRect = new()
            {
                x = position.x, y = position.y + EditorGUIUtility.singleLineHeight, width = cellWidth, height = EditorGUIUtility.singleLineHeight,
            };
            Rect ownerObjFieldRect = new()
            {
                x = position.x + cellWidth, y = position.y + EditorGUIUtility.singleLineHeight, width = cellWidth * 2,
                height = EditorGUIUtility.singleLineHeight,
            };

            GUIContent ownerGuiContent = new("Owner: ", $"Id: {ownerIdProp.stringValue}");
            EditorGUI.LabelField(ownerLabelRect, ownerGuiContent);
            EditorGUI.BeginDisabledGroup(true);
            GameObject ownerGo = ReferenceManager.GetGameObject(ownerIdProp.stringValue);
            EditorGUI.ObjectField(ownerObjFieldRect, ownerGo, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();


            Rect compLabelRect = new()
            {
                x = position.x, y = position.y + EditorGUIUtility.singleLineHeight * 2, width = cellWidth, height = EditorGUIUtility.singleLineHeight,
            };
            Rect compObjFieldRect = new()
            {
                x = position.x + cellWidth, y = position.y + EditorGUIUtility.singleLineHeight * 2, width = cellWidth * 2,
                height = EditorGUIUtility.singleLineHeight,
            };

            GUIContent compGuiContent = new("Component: ", $"Id: {compIdProp.stringValue}");
            EditorGUI.LabelField(compLabelRect, compGuiContent);
            EditorGUI.BeginDisabledGroup(true);

            EditorGUI.ObjectField(compObjFieldRect, ownerGo.GetReferencedComponent(compIdProp.stringValue), typedRef.GetRefType(), false);
            EditorGUI.EndDisabledGroup();

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 3;
        }


        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ISerializableTypedRef typedRef = (ISerializableTypedRef)property.boxedValue;

            VisualElement container = new()
            {
                // style =
                // {
                //     flexGrow = 1
                // }
            };

            SerializedProperty ownerIdProp = property.FindPropertyRelative("_ownerId");
            SerializedProperty compIdProp = property.FindPropertyRelative("_compId");
            GameObject ownerGo = ownerIdProp.stringValue == ReferenceManager.NoneReference
                ? null
                : ReferenceManager.GetGameObject<GameObject>(ownerIdProp.stringValue);


            // Game Object
            ObjectField objectField = new()
            {
                objectType = typeof(GameObject), value = ownerGo, focusable = true, tooltip =
                    "The Owner Object of the  selected comp if Any. Double click to highlight in inspector if scene is open.",
                label = "Owner",
            };
            objectField.SetEnabled(false);

            container.Add(objectField);

            Component displayedComp = ownerGo.GetReferencedComponent(compIdProp.stringValue);
            Type refType = typedRef.GetRefType();

            NsGameObjectCompSearchProvider searchProvider = new("RefSearchCompProvider", refType);

            NsObjectField compField = new(property.displayName)
            {
                ObjectType = refType, Value = displayedComp, tooltip = property.tooltip, HideTabs = true,
                WindowTitle = new GUIContent("Select a referenced GameObject..."), SearchProvider = searchProvider, SearchViewFlags = SearchViewFlags,
            };

            compField.RegisterSelectionCallback((obj, cancelled) =>
            {
                if (cancelled) return;

                property.serializedObject.Update();

                switch (obj)
                {
                    case Component comp:
                    {
                        ownerIdProp.stringValue = ReferenceManager.GetGuidOf(comp.gameObject);
                        GameObjectComponentReferenceBank gameObjectComponentReferenceBank =
                            comp.gameObject.GetComponent<GameObjectComponentReferenceBank>();
                        if (gameObjectComponentReferenceBank == null)
                        {
                            gameObjectComponentReferenceBank =
                                comp.gameObject.AddComponent<GameObjectComponentReferenceBank>();
                        }

                        gameObjectComponentReferenceBank.LoadReferences();
                        compIdProp.stringValue = gameObjectComponentReferenceBank.GetGuidOf(comp);
                        objectField.SetValueWithoutNotify(comp.gameObject); // This is soooo weird, I can't stress it enough but hey... it works
                        break;
                    }
                    case null:
                    {
                        ownerIdProp.stringValue = ReferenceManager.NoneReference;
                        compIdProp.stringValue = ReferenceManager.NoneReference;
                        objectField.SetValueWithoutNotify(null);
                        break;
                    }
                }

                compIdProp.serializedObject.ApplyModifiedProperties();
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(compField);

            return container;
        }
    }
}