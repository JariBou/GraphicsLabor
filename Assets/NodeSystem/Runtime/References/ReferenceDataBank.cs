using System;
using System.Collections;
using System.Collections.Generic;
using NodeSystem.Runtime.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReferenceDataBank : MonoBehaviour
{
    [SerializeField] private List<GameObjectReference> m_references;

    private void OnEnable()
    {
        // I don't liek this one bit
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
        List<GameObject> objectsInScene = new List<GameObject>(FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        List<GameObjectReference> prevRefList = new List<GameObjectReference>(m_references);
        // m_references = new List<GameObjectReference>(objectsInScene.Count);
        foreach (GameObject go in objectsInScene)
        {
            if (m_references.Find(goRef => goRef.Object == go) == null)
            {
                m_references.Add(new GameObjectReference(go));
            }
            else
            {
                prevRefList.Remove(prevRefList.Find(goRef => goRef.Object == go));
            }
        }

        foreach (GameObjectReference t in prevRefList)
        {
            m_references.Remove(t);
        }
        Debug.Log("Load References");
    }

    public T GetGameObject<T>(string guid) where T : UnityEngine.Object
    {
        return m_references.Find(goRef => goRef.Guid == guid).Object as T;
    }
    
    public string GetGuidOf<T>(T obj) where T : UnityEngine.Object
    {
        return m_references.Find(goRef => goRef.Object == obj).Guid ?? "";
    }

    [Serializable]
    public class GameObjectReference
    {
        [SerializeField] private GameObject m_go;
        [SerializeField] private string m_guid;

        public GameObject Object => m_go;
        public string Guid => m_guid;

        public GameObjectReference(GameObject go)
        {
            m_go = go;
            m_guid = GuidSystem.NewGuid();
        }
    }
    
}
