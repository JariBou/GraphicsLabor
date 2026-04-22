using System;
using System.Collections.Generic;
using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.RefEditors.SearchProviders
{
    public class NsGameObjectCompSearchProvider : NsGameObjectSearchProvider
    {
        private new const string FilterId = "refgocomp:";
        private readonly Type _compType;
        private readonly Dictionary<GameObject, Component> _goCompDic = new();

        public NsGameObjectCompSearchProvider(string id, Type compType,
            string displayName = "NsGameObjectCompSearchProvider",
            Func<SearchContext, List<SearchItem>, SearchProvider, object> fetchItemsHandler = null) :
            base(id, displayName, fetchItemsHandler)
        {
            filterId =  FilterId;
            _compType = compType;
        }

        protected override Texture2D FetchThumbnail(SearchItem item, SearchContext ctx)
        {
            if (_goCompDic.TryGetValue((GameObject)item.data, out Component comp))
                return (Texture2D)EditorGUIUtility.ObjectContent(comp, _compType).image;

            return base.FetchThumbnail(item, ctx);
        }

        protected override Object ToObject(SearchItem item, Type t)
        {
            GameObject go = (GameObject)item.data;
            return _goCompDic.TryGetValue((GameObject)item.data, out Component comp)
                ? comp
                : go.GetComponent(_compType);
        }

        protected override void UpdateCachedData()
        {
            _gameObjects = new List<GameObject>();
            _availableDataBanks = ReferenceManager.GetAvailableDataBanks();
            foreach (ReferenceDataBank dataBank in _availableDataBanks)
            {
                GameObject[] referencedGameObjects = dataBank.GetReferencedGameObjects();
                foreach (GameObject referencedGameObject in referencedGameObjects)
                {
                    Component component = referencedGameObject.GetComponent(_compType);
                    if (component == null) continue;

                    _gameObjects.Add(referencedGameObject);
                    _goCompDic.TryAdd(referencedGameObject, component);
                }
            }
        }
    }
}