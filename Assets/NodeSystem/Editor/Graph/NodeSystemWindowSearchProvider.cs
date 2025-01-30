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
    public struct SearchContextElement
    {
        public object target { get; }
        public string title { get;  }

        public SearchContextElement(object target, string title)
        {
            this.target = target;
            this.title = title;
        }
    }
    
    
    public class NodeSystemWindowSearchProvider : ScriptableObject, ISearchWindowProvider
    {
        public NodeSystemView graph;
        public VisualElement target;
        
        private static List<SearchContextElement> elements;
        
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> tree = new List<SearchTreeEntry>();
            tree.Add(new SearchTreeGroupEntry(new GUIContent("Nodes"), 0));
            
            elements = new List<SearchContextElement>();
            
            IEnumerable<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies()/*.Where(assembly => !assembly.GetName().Name.StartsWith("Unity"))*/;

            foreach (Assembly assembly in assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.CustomAttributes.ToList() != null)
                    {
                        NodeInfoAttribute attribute = type.GetCustomAttribute<NodeInfoAttribute>();
                        if (attribute != null)
                        {
                            object node = Activator.CreateInstance(type);
                            if (string.IsNullOrEmpty(attribute.menuItem)) continue;
                            elements.Add(new SearchContextElement(node, attribute.menuItem));
                        }
                    }
                }
            }
            
            //Sort by name
            elements.Sort((entry1, entry2) =>
            {
                string[] splits1 = entry1.title.Split('/');
                string[] splits2 = entry2.title.Split('/');

                for (int i = 0; i < splits1.Length; i++)
                {
                    if (i >= splits2.Length)
                    {
                        return 1;
                    }
                    
                    int value = splits1[i].CompareTo(splits2[i]);
                    if (value != 0)
                    {
                        // Leaves go before nodes
                        if (splits1.Length != splits2.Length && (i == splits1.Length - 1 || i == splits2.Length - 1))
                        {
                            return splits1.Length < splits2.Length ? 1 : -1;
                        }
                        return value;
                    }
                    
                }
                return 0;
            });
            
            List<string> groups = new List<string>();

            foreach (SearchContextElement element in elements)
            {
                string[] entryTitle = element.title.Split('/');

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
                
                SearchTreeEntry entry = new SearchTreeEntry(new GUIContent(entryTitle.Last()))
                {
                    level = entryTitle.Length,
                    userData = new SearchContextElement(element.target, element.title)
                };
                tree.Add(entry);
            } 
            
            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 windowMousePosition = graph.ChangeCoordinatesTo(graph, context.screenMousePosition - graph.window.position.position);
            Vector2 graphMousePosition = graph.contentViewContainer.WorldToLocal(windowMousePosition);
            Debug.Log(graphMousePosition);
            
            SearchContextElement element = (SearchContextElement)searchTreeEntry.userData;
            
            NodeSystemNode node = (NodeSystemNode)element.target;
            node.SetPosition(new Rect(graphMousePosition, new Vector2()));
            graph.Add(node);
            
            return true;
        }
    }
}