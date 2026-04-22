using System;
using System.Collections.Generic;
using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.RefEditors.SearchProviders
{
    public class NsGameObjectSearchProvider : SearchProvider
    {
        protected const string FilterId = "refgo:";
        protected ReferenceDataBank[] _availableDataBanks;
        protected List<GameObject> _gameObjects = new();

        public NsGameObjectSearchProvider(string id, string displayName = "NsGameObjectSearchProvider",
            Func<SearchContext, List<SearchItem>, SearchProvider, object> fetchItemsHandler = null) :
            base(id, displayName, fetchItemsHandler)
        {
            filterId = FilterId;
            fetchItems = FetchItems;
            fetchDescription = FetchDescription;
            toObject = ToObject; // Yeah so this is hella important
            fetchThumbnail = FetchThumbnail;
            // fetchLabel = FetchLabel;
            onEnable = OnEnable;
            onDisable = OnDisable;
        }

        protected virtual Texture2D FetchThumbnail(SearchItem item, SearchContext ctx)
        {
            return PrefabUtility.GetIconForGameObject((GameObject)item.data);
        }

        protected virtual string FetchDescription(SearchItem item, SearchContext ctx)
        {
            GameObject go = (GameObject)item.data;

            bool goActive = go.activeSelf;
            Transform parent = go.transform.parent;
            List<Transform> parentTransforms = new();
            while (parent != null)
            {
                parentTransforms.Add(parent);
                parent = parent.parent;
            }

            string path = "Scene/";
            for (int i = parentTransforms.Count - 1; i >= 0; i--)
            {
                parent = parentTransforms[i];
                path += parent.name + "/";
            }

            path += go.name;

            string description = $"IsActive: {goActive}\nPath: {path}";

            return description;
        }

        protected virtual Object ToObject(SearchItem item, Type t)
        {
            return (GameObject)item.data;
        }

        protected virtual void OnDisable()
        {
            ReferenceManager.RefDataBanksChanged -= ReferenceManagerOnRefDataBanksChanged;
        }

        protected virtual void OnEnable()
        {
            ReferenceManager.RefDataBanksChanged += ReferenceManagerOnRefDataBanksChanged;
            UpdateCachedData();
        }

        protected virtual void ReferenceManagerOnRefDataBanksChanged()
        {
            UpdateCachedData();
        }

        protected virtual IEnumerable<SearchItem> FetchItems(SearchContext ctx, List<SearchItem> items,
            SearchProvider provider)
        {
            for (int index = 0; index < _gameObjects.Count; index++)
            {
                GameObject obj = _gameObjects[index];
                if (!obj.name.Contains(ctx.searchText, StringComparison.InvariantCultureIgnoreCase)) continue;
                yield return provider.CreateItem(ctx, $"{index}_{obj.name}", obj.name, null, null, obj);
            }
        }

        protected virtual void UpdateCachedData()
        {
            _gameObjects = new List<GameObject>();
            _availableDataBanks = ReferenceManager.GetAvailableDataBanks();
            foreach (ReferenceDataBank dataBank in _availableDataBanks)
                _gameObjects.AddRange(dataBank.GetReferencedGameObjects());
        }
    }
}