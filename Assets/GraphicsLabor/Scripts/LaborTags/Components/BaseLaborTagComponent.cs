using GraphicsLabor.Scripts.Core.Utility;
using UnityEngine;

namespace GraphicsLabor.Scripts.LaborTags.Components
{
    public abstract class BaseLaborTagComponent : MonoBehaviour, ITagHolder
    {
        [SerializeField] private LaborTags _tags;
        public LaborTags Tags => _tags;
        public LaborTags GetLaborTags()
        {
            return _tags;
        }

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponents<ITagHolder>().Length > 1)
            {
                GLogger.LogWarning($"Gameobject {nameof(gameObject)} has more than 1 ITagHolder Component, this may cause unwanted behaviour");
            }
        }
        #endif
    }
}