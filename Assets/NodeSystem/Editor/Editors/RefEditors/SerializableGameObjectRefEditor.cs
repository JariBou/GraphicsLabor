using NodeSystem.Editor.Editors.RefEditors.SearchProviders;
using NodeSystem.Runtime.Core.RefSystem;
using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Search;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.RefEditors
{
    [CustomPropertyDrawer(typeof(SerializableGameObjectRef))]
    public class SerializableGameObjectRefEditor : PropertyDrawer
    {
        private const SearchViewFlags SearchViewFlags = UnityEngine.Search.SearchViewFlags.Borderless |
                                                        UnityEngine.Search.SearchViewFlags.GridView |
                                                        UnityEngine.Search.SearchViewFlags.DisableSavedSearchQuery |
                                                        UnityEngine.Search.SearchViewFlags.OpenInspectorPreview;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            EditorGUI.BeginProperty(position, label, property);
            SerializableGameObjectRef src = (SerializableGameObjectRef)property.boxedValue;
            SerializedProperty objIdProp = property.FindPropertyRelative("_objectId");
            // EditorGUI.LabelField(position, objIdProp.stringValue);
            EditorGUI.BeginChangeCheck();
            GameObject gameObject = src.Get();

            GUIContent labelContent = new()
            {
                tooltip = objIdProp.stringValue,
                text = label.text
            };

            Object obj = EditorGUI.ObjectField(position, labelContent, gameObject, typeof(GameObject), true);

            if (EditorGUI.EndChangeCheck())
            {
                objIdProp.stringValue = obj switch
                {
                    GameObject go => ReferenceManager.GetGuidOf(go),
                    null => ReferenceManager.NoneReference,
                    _ => objIdProp.stringValue
                };

                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.EndProperty();
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            SerializedProperty objectIdProp = property.FindPropertyRelative("_objectId");

            GameObject ownerGo = objectIdProp.stringValue == ReferenceManager.NoneReference
                ? null
                : ReferenceManager.GetGameObject<GameObject>(objectIdProp.stringValue);


            NsGameObjectSearchProvider searchProvider = new("RefSearchProvider");
            SearchContext searchContext = SearchService.CreateContext(searchProvider);
            SearchViewState searchViewState = new(searchContext, SearchViewFlags)
            {
                hideTabs = true,
                title = "Select a referenced GameObject...",
                windowTitle = new GUIContent("Select a referenced GameObject...") // Great, doesn't work... thanks Unity
            };

            /*
            // Create a SearchContext for our object selector.
            var provider = CreateProvider();
            var searchContext = SearchService.CreateContext(provider);

            // Create the SearchViewFlags for our object selector. We want it to show as a borderless window, in grid view and without the ability to show the saved search queries.
            var searchViewFlags = SearchViewFlags.Borderless | SearchViewFlags.GridView | SearchViewFlags.DisableSavedSearchQuery;

            // Create the SearchViewState of our object selector.
            var searchViewState = new SearchViewState(searchContext, searchViewFlags);

            // Set the group we want to show
            searchViewState.group = "all"; // Group that shows all results plus the "None" item. This is the default.
            */

            ObjectField objectField = new()
            {
                objectType = typeof(GameObject),
                value = ownerGo,
                focusable = true,
                name = property.displayName,

                tooltip = property.tooltip,
                label = property.displayName,

                searchContext = searchContext,
                searchViewFlags = SearchViewFlags,
                searchViewState = searchViewState
                // style =
                // {
                //     flexGrow = 1
                // }
            };

            objectField.RegisterValueChangedCallback(evt =>
            {
                property.serializedObject.Update();
                objectIdProp.serializedObject.Update();

                Object obj = evt.newValue;
                switch (obj)
                {
                    case GameObject go:
                        string stringValue = ReferenceManager.GetGuidOf(go.gameObject);
                        objectIdProp.stringValue = stringValue;
                        break;
                    case null:
                        objectIdProp.stringValue = ReferenceManager.NoneReference;
                        break;
                    default:
                        objectIdProp.stringValue = objectIdProp.stringValue;
                        break;
                }

                objectIdProp.serializedObject.ApplyModifiedProperties();
                property.serializedObject.ApplyModifiedProperties();
            });

            return objectField;
        }
    }
}