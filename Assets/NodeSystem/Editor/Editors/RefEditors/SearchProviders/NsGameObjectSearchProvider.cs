using System;
using System.Collections.Generic;
using NodeSystem.Runtime.References;
using UnityEditor.Search;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.RefEditors.SearchProviders
{
    public class NsGameObjectSearchProvider : SearchProvider
    {
        private const string FilterId = "refgo:";
        private ReferenceDataBank[] _availableDataBanks;
        private List<GameObject> _gameObjects = new();

        public NsGameObjectSearchProvider(string id, string displayName = "NsGameObjectSearchProvider",
            Func<SearchContext, List<SearchItem>, SearchProvider, object> fetchItemsHandler = null) :
            base(id, displayName, fetchItemsHandler)
        {
            filterId = FilterId;
            fetchItems = FetchItems;
            toObject = ToObject; // Yeah so this is hella important
            // fetchLabel = FetchLabel;
            onEnable = OnEnable;
            onDisable = OnDisable;
            // 
            
        }

        private Object ToObject(SearchItem item, Type t)
        {
            return ((GameObject)item.data);
        }

        private string FetchLabel(SearchItem item, SearchContext context)
        {
            return ((GameObject)item.data).name;
        }

        private void OnDisable()
        {
            
        }

        private void OnEnable()
        {
            _availableDataBanks = ReferenceManager.GetAvailableDataBanks();
            foreach (ReferenceDataBank dataBank in _availableDataBanks)
            {
                _gameObjects.AddRange(dataBank.GetReferencedGameObjects());
            }
        }

        private IEnumerable<SearchItem> FetchItems(SearchContext ctx, List<SearchItem> items, SearchProvider provider)
        {
            // new GameObject("Test1")
            for (int index = 0; index < _gameObjects.Count; index++)
            {
                Debug.Log(_gameObjects[index].name);
                GameObject obj = _gameObjects[index];
                // SearchItem searchItem = new SearchItem($"item_{index}")
                // {
                //     data = obj,
                //     provider = this,
                //     label = obj.name,
                //     description = obj.name,
                // };
                // items.Add(searchItem);
                yield return provider.CreateItem(ctx, $"{index}_{obj.name}", obj.name, null, null, obj);
            }

            // return null;
        }
    }

    public class NsGameObjectSearchProvider2
    {
        private const string id = "NsGameObjectSearchProvider";
        private const string displayName = "Referenced Game Objects";
        private const string filterId = "refgo:";


        [SearchItemProvider]
        static SearchProvider CreateProvider()
        {
            return new SearchProvider(id, displayName)
            {
                filterId = filterId,
                priority = 10,
                fetchItems = FetchItems
            };
        }

        private static IEnumerable<SearchItem> FetchItems(SearchContext context, List<SearchItem> _, SearchProvider provider)
        {
            if (context.empty)
            {
                yield break;
            }
            
            List<GameObject> gameObjects = new();
            foreach (ReferenceDataBank dataBank in ReferenceManager.GetAvailableDataBanks())
            {
                gameObjects.AddRange(dataBank.GetReferencedGameObjects());
            }
            for (int index = 0; index < gameObjects.Count; index++)
            {
                Debug.Log(gameObjects[index].name);
                GameObject obj = gameObjects[index];

                yield return provider.CreateItem(context, $"{index}_{obj.name}", obj.name, null, null, obj);
            }
        }
    }
}