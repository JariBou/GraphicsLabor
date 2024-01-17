using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Tags.Components
{
    /// <summary>
    /// A Component giving access to LaborTags and allowing for a direct access to a linkedGameObject and/or a linkedMonoBehaviour
    /// </summary>
    [AddComponentMenu("GraphicsLabor/Linked Labor Tags")]
    public class LinkedLaborTagComponent : BaseLaborTagComponent
    {
        [SerializeField] private GameObject _linkedGameObject;
        [SerializeField] private MonoBehaviour _linkedMonoBehaviour;

        [SerializeField] private bool _linkGameObject;
        [SerializeField] private bool _linkMonoBehaviour;

        public GameObject GetLinkedGameObject() => _linkGameObject ? _linkedGameObject : null;

        public T GetGameObjectComponent<T>() where T : Component
        {
            if (!_linkGameObject || _linkedGameObject == null) return null;
            return _linkedGameObject.GetComponent<T>();
        }
        
        public MonoBehaviour GetLinkedMonoBehaviour() => _linkMonoBehaviour ? _linkedMonoBehaviour : null;

        public T GetLinkedMonoBehaviour<T>() where T : MonoBehaviour
        {
            return _linkMonoBehaviour ? (T) _linkedMonoBehaviour : null;
        }
    }
}