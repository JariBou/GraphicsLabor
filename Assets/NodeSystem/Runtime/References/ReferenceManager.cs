using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;

public class ReferenceManager : MonoBehaviour
{
    [SerializeField] private List<ReferenceDataBank> m_referenceDataBanks = new();

    private static ReferenceManager m_instance;
    public static ReferenceManager Instance
    {
        get {
            if (m_instance != null) return m_instance;
            
            m_instance = FindAnyObjectByType<ReferenceManager>();
            if (m_instance == null)
            {
                m_instance = new GameObject("ReferenceManager").AddComponent<ReferenceManager>();
                #if !UNITY_EDITOR
                DontDestroyOnLoad(m_instance.gameObject);
                #endif          
            }
            m_instance.Initialize();
            return m_instance;
        }
    }

    public static List<ReferenceDataBank> GetAvailableDataBanks()
    {
        return new List<ReferenceDataBank>(FindObjectsByType<ReferenceDataBank>(FindObjectsInactive.Include, FindObjectsSortMode.None));
    }

    public void Initialize()
    {
        m_referenceDataBanks.Clear();
        
        foreach (ReferenceDataBank referenceDataBank in GetAvailableDataBanks())
        {
            m_instance.RecordHolder(referenceDataBank);
        }
    }
    
    public void RecordHolder(ReferenceDataBank referenceDataBank)
    {
        Debug.Log("Recording " + referenceDataBank.name);
        m_referenceDataBanks.Add(referenceDataBank);
    }

    public void UnrecordHolder(ReferenceDataBank referenceDataBank)
    {
        GetAvailableDataBanks().Remove(referenceDataBank);
    }
    public static T GetGameObject<T>(string guid) where T : UnityEngine.Object
    {
        if (guid == "") return null;
        // return GetAvailableDataBanks().Select(holder => holder.GetGameObject<T>(guid)).FirstOrDefault(obj => obj);
        return Instance.m_referenceDataBanks.Select(holder => holder.GetGameObject<T>(guid)).FirstOrDefault(obj => obj);
    }
    
    public static string GetGuidOf<T>(T obj) where T : UnityEngine.Object
    {
        foreach (var guidOf in Instance.m_referenceDataBanks.Select(mHolder => mHolder.GetGuidOf(obj)).Where(guidOf => guidOf != ""))
        {
            return guidOf;
        }

        return "";
    }

    public void TestPrint()
    {
        Debug.Log(m_referenceDataBanks.Count);
    }
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
