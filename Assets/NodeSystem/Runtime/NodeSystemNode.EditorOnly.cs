using System.Linq;
using NodeSystem.Runtime.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    // TODO: see if this is fine in build
    public abstract partial class NodeSystemNode
    {
#if UNITY_EDITOR
        /// <summary>
        /// Only available in editor
        /// </summary>
        [FormerlySerializedAs("m_position")] [SerializeField]
        protected internal Rect _position;
        
             
        /// <summary>
        /// Only available in editor
        /// </summary>
        public Rect Position => _position;
        
        /// <summary>
        /// Only available in editor
        /// </summary>
        /// <param name="newPosition"> The new position of the node </param>
        public void SetPosition(Rect newPosition)
        {
            _position = newPosition;
        }
        
        /// <summary>
        /// Only available in editor
        /// </summary>
        /// <param name="displacement"> The displacement of the node </param>
        public void Displace(Vector2 displacement)
        {
            _position.x += displacement.x;
            _position.y += displacement.y;
        }
        
#endif
        
        public virtual void CopyFrom(NodeSystemNode node)
        {
            // string newGuid = GuidSystem.NewGuid();
            // m_guid = newGuid;
            m_ports = node.m_ports.Select(portInfo =>
                    new PortInfo(portInfo.ExposedPropertyName, m_guid, portInfo.PortIndex, portInfo.PortDirection))
                .ToList();
            IsPure = node.IsPure;
            _position = node._position;
        }
    }
}