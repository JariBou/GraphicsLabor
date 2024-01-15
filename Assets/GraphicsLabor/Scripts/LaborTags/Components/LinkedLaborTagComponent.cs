using GraphicsLabor.Scripts.Core.Utility;
using UnityEngine;

namespace GraphicsLabor.Scripts.LaborTags.Components
{
    [AddComponentMenu("GraphicsLabor/Linked Labor Tag")]
    public class LinkedLaborTagComponent : BaseLaborTagComponent
    {
        [SerializeField] private GameObject _linkedGameObject;
        [SerializeField] private MonoBehaviour _linkedMonoBehaviour;

        public GameObject GetLinkedGameObject() => _linkedGameObject;

        public T GetGameObjectComponent<T>() where T : Component
        {
            return _linkedGameObject.GetComponent<T>();
        }
        
        public MonoBehaviour GetLinkedMonoBehaviour() => _linkedMonoBehaviour;

        public T GetLinkedMonoBehaviour<T>() where T : MonoBehaviour
        {
            return (T) _linkedMonoBehaviour;
        }
    }
}