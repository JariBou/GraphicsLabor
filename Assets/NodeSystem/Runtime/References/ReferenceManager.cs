using System;
using System.Collections.Generic;
using System.Linq;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime.References
{
    [AddComponentMenu("Reference Data Banks/Reference Manager")]
    public class ReferenceManager : MonoBehaviour
    {
        [FormerlySerializedAs("m_referenceDataBanks")] [SerializeField] private List<ReferenceDataBank> _referenceDataBanks = new();
        
        public const string NoneReference = "";

        private static ReferenceManager _instance;
        public static ReferenceManager Instance
        {
            get {
                if (_instance is not null) return _instance;
            
                _instance = FindAnyObjectByType<ReferenceManager>();
                if (_instance is null)
                {
                    _instance = new GameObject("ReferenceManager").AddComponent<ReferenceManager>();
                    // #if !UNITY_EDITOR
                    DontDestroyOnLoad(_instance.gameObject);
                    // #endif          
                }
                _instance.Initialize();
                return _instance;
            }
        }

        public static List<ReferenceDataBank> GetAvailableDataBanks()
        {
            return new List<ReferenceDataBank>(FindObjectsByType<ReferenceDataBank>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        }

        public void Initialize()
        {
            _referenceDataBanks.Clear();
        
            foreach (ReferenceDataBank referenceDataBank in GetAvailableDataBanks())
            {
                _instance.RecordHolder(referenceDataBank);
            }

            if (_referenceDataBanks.Count == 0)
            {
                _instance.RecordHolder(_instance.gameObject.AddComponent<ReferenceDataBank>());
            }
        }
    
        public void RecordHolder(ReferenceDataBank referenceDataBank)
        {
            Debug.Log("Recording " + referenceDataBank.name);
            _referenceDataBanks.Add(referenceDataBank);
        }

        public void UnrecordHolder(ReferenceDataBank referenceDataBank)
        {
            GetAvailableDataBanks().Remove(referenceDataBank);
        }
        public static T GetGameObject<T>(string guid) where T : UnityEngine.Object
        {
            if (guid == "") return null;
            // return GetAvailableDataBanks().Select(holder => holder.GetGameObject<T>(guid)).FirstOrDefault(obj => obj);
            return Instance._referenceDataBanks.Select(holder => holder.GetGameObject<T>(guid)).FirstOrDefault(obj => obj);
        }
    
        public static string GetGuidOf<T>(T obj) where T : UnityEngine.Object
        {
            foreach (var guidOf in Instance._referenceDataBanks.Select(mHolder => mHolder.GetGuidOf(obj)).Where(guidOf => guidOf != ""))
            {
                return guidOf;
            }

            return "";
        }

        public void TestPrint()
        {
            Debug.Log(_referenceDataBanks.Count);
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
}