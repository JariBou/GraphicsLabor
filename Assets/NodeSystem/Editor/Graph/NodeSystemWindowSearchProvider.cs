using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeSystem.Runtime;
using NodeSystem.Runtime.Attributes;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph
{
    public class NodeSystemWindowSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        private struct SearchContextElement
        {
            public object Target { get; }
            public string Title { get;  }

            public SearchContextElement(object target, string title)
            {
                Target = target;
                Title = title;
            }
        }
        
        public NodeSystemView graph;
        public VisualElement target;
        
        private static List<SearchContextElement> _elements;
        private static List<SearchTreeEntry> _cachedTree = new();
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            if (_cachedTree.Count != 0)
            {
                return _cachedTree;
            }
            
            List<SearchTreeEntry> tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Nodes")) };

            _elements = new List<SearchContextElement>();
            
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => !assembly.GetName().Name.StartsWith("Unity"));

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    NodeInfoAttribute attribute = type.GetCustomAttribute<NodeInfoAttribute>();
                    if (attribute != null)
                    {
                        object node = Activator.CreateInstance(type); // Suspect n°1
                        if (string.IsNullOrEmpty(attribute.MenuItem)) continue;
                        _elements.Add(new SearchContextElement(node, attribute.MenuItem));
                    }
                }
            }
            
            //Sort by name
            _elements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.Title.Split('/');
                string[] splits2 = entry2.Title.Split('/');

                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length)
                    {
                        return 1;
                    }
                    
                    int value = string.Compare(splits1[i], splits2[i], StringComparison.Ordinal);
                    if (value == 0) continue;
                    
                    // Leaves go before nodes
                    if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                    {
                        return splits1.Length < splits2.Length ? 1 : -1;
                    }
                    return value;

                }
                return 0;
            });
            
            List<string> groups = new List<string>();

            foreach (SearchContextElement element in _elements)
            {
                string[] entryTitle = element.Title.Split('/');

                string groupName = "";

                for (int i = 0; i < entryTitle.Length - 1; i++)
                {
                    groupName += entryTitle[i];

                    if (!groups.Contains(groupName))
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(entryTitle[i]), i+1));
                        groups.Add(groupName);
                    }
                    groupName += "/";
                }
                
                SearchTreeEntry entry = new(new GUIContent(entryTitle.Last()))
                {
                    level = entryTitle.Length,
                    userData = new SearchContextElement(element.Target, element.Title)
                };
                tree.Add(entry);
            }

            _cachedTree = tree;
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 windowMousePosition = graph.ChangeCoordinatesTo(graph, context.screenMousePosition - graph.window.position.position);
            Vector2 graphMousePosition = graph.contentViewContainer.WorldToLocal(windowMousePosition);
            Debug.Log(graphMousePosition);
            
            SearchContextElement element = (SearchContextElement)searchTreeEntry.userData;
            
            NodeSystemNode node = (NodeSystemNode)element.Target;
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
            graph.Add(node);
            
            return true;
        }
    }
}