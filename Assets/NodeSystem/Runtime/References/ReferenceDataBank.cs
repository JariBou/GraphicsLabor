using System;
using System.Collections.Generic;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace NodeSystem.Runtime.References
{
    [AddComponentMenu("Reference Data Banks/GameObject Reference Bank")]
    public class ReferenceDataBank : MonoBehaviour
    {
        [FormerlySerializedAs("m_references")] [SerializeField]
        private List<GameObjectReference> _references;

        private void OnEnable()
        {
            // I don't liek this one bit
            //TODO: Update this, what was I thinking??
            ReferenceManager refManager = ReferenceManager.Instance;
            if (refManager == null)
            {
                GameObject refManagerGo = Instantiate(new GameObject("ReferenceManager"));
                DontDestroyOnLoad(refManagerGo);
                refManager = refManagerGo.AddComponent<ReferenceManager>();
            }

            refManager.RecordHolder(this);
        }

        private void OnDisable()
        {
            ReferenceManager.Instance.UnrecordHolder(this);
        }

        public void LoadReferences()
        {
            // Scene activeScene = SceneManager.GetActiveScene();
            var objectsInScene =
                new List<GameObject>(FindObjectsByType<GameObject>(FindObjectsInactive.Include,
                    FindObjectsSortMode.None));
            var refsToRemove = new List<GameObjectReference>(_references);
            // m_references = new List<GameObjectReference>(objectsInScene.Count);
            foreach (GameObject go in objectsInScene)
                if (_references.Find(goRef => goRef.Object == go) == null)
                    _references.Add(new GameObjectReference(go));
                else
                    refsToRemove.Remove(refsToRemove.Find(goRef => goRef.Object == go));

            foreach (GameObjectReference t in refsToRemove) _references.Remove(t);
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

        [Serializable]
        public class GameObjectReference
        {
            [FormerlySerializedAs("m_go")] [SerializeField]
            private GameObject _go;

            [FormerlySerializedAs("m_guid")] [SerializeField]
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