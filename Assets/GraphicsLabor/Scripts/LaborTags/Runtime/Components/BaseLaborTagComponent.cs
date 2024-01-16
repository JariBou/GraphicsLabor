using UnityEngine;

namespace GraphicsLabor.Scripts.LaborTags.Runtime.Components
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
                Debug.LogWarning($"Gameobject {nameof(gameObject)} has more than 1 ITagHolder Component, this may cause unwanted behaviour");
            }
        }
        #endif
    }
}