using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Tags
{
    public abstract class LaborBehaviour : MonoBehaviour, ITagHolder
    {
        [SerializeField] private LaborTags _tags;

        public LaborTags GetLaborTags()
        {
            return _tags;
        }

        /// <summary>
        /// Override this method if you want to Use the OnValidateMethod
        /// </summary>
        protected void OnSelfValidate(){}

        #if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponents<ITagHolder>().Length > 1)
            {
                Debug.LogWarning($"Gameobject {nameof(gameObject)} has more than 1 ITagHolder Component, this may cause unwanted behaviour");
            }
            OnSelfValidate();
        }
        #endif
    }
}