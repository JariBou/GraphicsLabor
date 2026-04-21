using System;
using System.Collections.Generic;
using NodeSystem.Editor.Editors.RefEditors.SearchProviders;
using NodeSystem.Runtime.Core.RefSystem;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
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

        
            // TODO: find a way  to activate it by default
            NsGameObjectSearchProvider searchProvider = new NsGameObjectSearchProvider("RefSearchProvider");
            SearchContext searchContext = SearchService.CreateContext(searchProvider);
            var searchViewFlags = SearchViewFlags.Borderless | SearchViewFlags.GridView | SearchViewFlags.DisableSavedSearchQuery;
            var searchViewState = new SearchViewState(searchContext, searchViewFlags);
            searchViewState.group = "all"; // Group that shows all results plus the "None" item. This is the default.
            
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
                searchViewFlags = searchViewFlags,
                searchViewState = searchViewState,
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
        
        
    static QueryEngine<GameObject> CreateQueryEngine()
    {
        var qe = new QueryEngine<GameObject>(new QueryValidationOptions() { validateFilters = true });
        qe.AddFilter<string>("t", FilterObjectType, new[] { "=", ":" });
        return qe;
    }

    SearchProvider CreateProvider()
    {
        return new SearchProvider("MyProviderId", "My Provider") { fetchItems = FetchItems, toObject = ToObject, active = true };
    }

    static Object ToObject(SearchItem item, Type type)
    {
        return item.data as UnityEngine.Object;
    }

    IEnumerable<SearchItem> FetchItems(SearchContext context, List<SearchItem> items, SearchProvider provider)
    {
        var parsedQuery = CreateQueryEngine().ParseQuery(context.searchQuery);
        if (!parsedQuery.valid)
        {
            foreach (var parsedQueryError in parsedQuery.errors)
            {
                context.AddSearchQueryError(new SearchQueryError(parsedQueryError, context, provider));
            }
            yield break;
        }

        parsedQuery.returnPayloadIfEmpty = true;
        var results = parsedQuery.Apply(SearchUtils.FetchGameObjects());
        foreach (var gameObject in results)
        {
            yield return provider.CreateItem(context, gameObject.name, gameObject.name, null, null, gameObject);
        }
    }

    static bool FilterObjectType(GameObject obj, string op, string value)
    {
        var valueLowerCase = value.ToLowerInvariant();
        var components = obj.GetComponents<Component>();
        foreach (var component in components)
        {
            var componentType = component.GetType();
            var componentName = componentType.Name.ToLowerInvariant();
            var componentFullName = componentType.FullName.ToLowerInvariant();
            if (op == "=")
            {
                if (componentName == valueLowerCase || componentFullName == valueLowerCase)
                {
                    return true;
                }
            }
            else if (op == ":")
            {
                if (componentName.Contains(valueLowerCase) || componentFullName.Contains(valueLowerCase))
                {
                    return true;
                }
            }
        }
        return false;
    }
    }
}