using System;
using System.Collections.Generic;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace NodeSystem.Runtime.References
{
    [AddComponentMenu(NodeSystemConsts.AddComponentMenuCategoryName + "/References/GameObject Reference Bank")]
    public class ReferenceDataBank : MonoBehaviour
    {
        [FormerlySerializedAs("m_references"), SerializeField]
        private List<GameObjectReference> _references = new();

        [SerializeField] private bool _autoRecord = true;

        public void OnEnable()
        {
            if (_autoRecord) ReferenceManager.Instance.RecordRefDataBank(this);
        }

        private void OnDisable()
        {
            ReferenceManager.Instance.UnrecordRefDataBank(this);
        }

        public event Action ReferencesChanged;

        public void LoadReferences()
        {
            GameObject[] objectsInScene = FindObjectsByType<GameObject>(FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            List<GameObjectReference> refsToRemove = new(_references);
            foreach (GameObject go in objectsInScene)
                if (_references.Find(goRef => goRef.Object == go) == null)
                    _references.Add(new GameObjectReference(go));
                else
                    refsToRemove.Remove(refsToRemove.Find(goRef => goRef.Object == go));

            foreach (GameObjectReference t in refsToRemove) _references.Remove(t);
            TriggerReferencesChanged();
            Debug.Log("Load References");
        }

        public T GetGameObject<T>(string guid) where T : Object
        {
            try
            {
                return _references.Find(goRef => goRef.Guid == guid)?.Object as T;
            }
            catch (Exception)
            {
                Debug.LogWarning("Object  with guid '" + guid + "' could not be found. ");
                return null;
            }
        }

        public string GetGuidOf<T>(T obj) where T : Object
        {
            try
            {
                return _references.Find(goRef => goRef.Object == obj).Guid ?? "";
            }
            catch (Exception)
            {
                Debug.LogWarning("Object  '" + obj + "' could not be found. ");
                return null;
            }
        }

        public GameObject[] GetReferencedGameObjects()
        {
            GameObject[] list = new GameObject[_references.Count];
            for (int i = 0; i < _references.Count; i++)
            {
                GameObjectReference reference = _references[i];
                list[i] = reference.Object;
            }

            return list;
        }

        protected virtual void TriggerReferencesChanged()
        {
            ReferencesChanged?.Invoke();
        }

        [Serializable]
        private class GameObjectReference
        {
            [FormerlySerializedAs("m_go"), SerializeField]
            private GameObject _go;

            [FormerlySerializedAs("m_guid"), SerializeField]
            private string _guid;

            public GameObjectReference(GameObject go)
            {
                _go = go;
                _guid = GuidSystem.NewGuid();
            }

            public GameObject Object => _go;
            public string Guid => _guid;
        }
    }
}