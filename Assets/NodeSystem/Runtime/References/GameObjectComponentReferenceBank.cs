using System;
using System.Collections.Generic;
using NodeSystem.Runtime.Utils;
using UnityEngine;

namespace NodeSystem.Runtime.References
{
    [AddComponentMenu(NodeSystemConsts.AddComponentMenuCategoryName +
                      "/References/GameObject Component Reference Bank")]
    public class GameObjectComponentReferenceBank : MonoBehaviour
    {
        [SerializeField] private List<GameObjectComponentReference> _references = new();

        public void LoadReferences()
        {
            Component[] currentComponents = GetComponents<Component>();
            List<GameObjectComponentReference> refsToRemove = new(_references);
            foreach (Component comp in currentComponents)
            {
                if (_references.Find(x => x.Comp == comp) == null)
                    _references.Add(new GameObjectComponentReference(comp));
                else
                    refsToRemove.Remove(refsToRemove.Find(x => x.Comp == comp));
            }

            foreach (GameObjectComponentReference t in refsToRemove)
            {
                _references.Remove(t);
            }
        }

        public T GetComp<T>(string guid) where T : Component
        {
            try
            {
                return _references.Find(goRef => goRef.Guid == guid)?.Comp as T;
            }
            catch (Exception)
            {
                Debug.LogWarning("Object  with guid '" + guid + "' could not be found. ");
                return null;
            }
        }

        public string GetGuidOf<T>(T obj) where T : Component
        {
            try
            {
                return _references.Find(goRef => goRef.Comp == obj).Guid ?? "";
            }
            catch (Exception)
            {
                Debug.LogWarning("Object  '" + obj + "' could not be found. ");
                return null;
            }
        }

        [Serializable]
        private class GameObjectComponentReference
        {
            [SerializeField] private Component _comp;
            [SerializeField] private string _guid;

            public Component Comp => _comp;
            public string Guid => _guid;

            public GameObjectComponentReference(Component comp)
            {
                _comp = comp;
                _guid = GuidSystem.NewGuid();
            }
        }
    }
}