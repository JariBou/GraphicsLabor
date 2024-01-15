using GraphicsLabor.Scripts.Core.Utility;
using UnityEngine;

namespace GraphicsLabor.Scripts.LaborTags
{
    public abstract class LaborBehaviour : MonoBehaviour, ITagHolder
    {
        [SerializeField] private LaborTags _tags;
        
        public LaborTags GetLaborTags()
        {
            return _tags;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Override this method if you want to Use the OnValidateMethod
        /// </summary>
        protected void OnSelfValidate(){}
        
        private void OnValidate()
        {
            if (GetComponents<ITagHolder>().Length > 1)
            {
                GLogger.LogWarning($"Gameobject {nameof(gameObject)} has more than 1 ITagHolder Component, this may cause unwanted behaviour");
            }
            OnSelfValidate();
        }
        #endif
    }
}