using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NodeSystem.Runtime.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace NodeSystem.Runtime.References
{
    [AddComponentMenu(NodeSystemConsts.AddComponentMenuCategoryName+"/References/Reference Manager")]
    public class ReferenceManager : MonoBehaviour
    {
        public const string NoneReference = "";

        private static ReferenceManager _instance;

        [FormerlySerializedAs("m_referenceDataBanks")] [SerializeField]
        private List<ReferenceDataBank> _referenceDataBanks = new();

        [NotNull]
        public static ReferenceManager Instance
        {
            get
            {
                if (_instance is not null) return _instance;

                // IRC I did it this way to allow for future implementation of in-editor graphs running
                
                _instance = FindAnyObjectByType<ReferenceManager>() ?? new GameObject("ReferenceManager").AddComponent<ReferenceManager>();

                // #if !UNITY_EDITOR
                // DontDestroyOnLoad(_instance.gameObject);
                // #endif          
                return _instance;
            }
        }

        private void Awake()
        {
            if (Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                _instance = null;
            }
        }

        public static List<ReferenceDataBank> GetAvailableDataBanks()
        {
            return new List<ReferenceDataBank>(
                FindObjectsByType<ReferenceDataBank>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        }

        public void RecordRefDataBank(ReferenceDataBank referenceDataBank)
        {
            Debug.Log("Recording " + referenceDataBank.name);
            _referenceDataBanks.Add(referenceDataBank);
        }

        public void UnrecordHolder(ReferenceDataBank referenceDataBank)
        {
            _referenceDataBanks.Remove(referenceDataBank);
        }

        public static T GetGameObject<T>(string guid) where T : Object
        {
            if (guid == "") return null;
            // return GetAvailableDataBanks().Select(holder => holder.GetGameObject<T>(guid)).FirstOrDefault(obj => obj);
            // return Instance._referenceDataBanks.Select(holder => holder.GetGameObject<T>(guid)).FirstOrDefault(obj => obj);
            T[] objects = Instance._referenceDataBanks.Select(holder => holder.GetGameObject<T>(guid)).ToArray();
            return objects.Any() ? objects.First() : null;
        }

        public static string GetGuidOf<T>(T obj) where T : Object
        {
            foreach (string guidOf in Instance._referenceDataBanks.Select(mHolder => mHolder.GetGuidOf(obj))
                         .Where(guidOf => guidOf != "")) return guidOf;

            return "";
        }
    }
}