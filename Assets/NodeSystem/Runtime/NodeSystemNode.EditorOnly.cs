using System.Linq;
using NodeSystem.Runtime.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace NodeSystem.Runtime
{
    public abstract partial class NodeSystemNode
    {
        public virtual void CopyDataFrom(NodeSystemNode node)
        {
            ports = node.ports.Select(portInfo =>
                                          new PortInfo(portInfo.ExposedPropertyName, guid, portInfo.PortIndex,
                                                       portInfo.PortDirection))
                        .ToList();
            IsPure = node.IsPure;
        #if UNITY_EDITOR
            position = node.position;
        #endif
        }
    #if UNITY_EDITOR
        /// <summary>
        ///     Only available in editor
        /// </summary>
        [FormerlySerializedAs("_position"), FormerlySerializedAs("m_position"), SerializeField]
        protected internal Rect position;


        /// <summary>
        ///     Only available in editor
        /// </summary>
        public Rect Position => position;

        /// <summary>
        ///     Only available in editor
        /// </summary>
        /// <param name="newPosition"> The new position of the node </param>
        public void SetPosition(Rect newPosition)
        {
            position = newPosition;
        }

        /// <summary>
        ///     Only available in editor
        /// </summary>
        /// <param name="displacement"> The displacement of the node </param>
        public void Displace(Vector2 displacement)
        {
            position.x += displacement.x;
            position.y += displacement.y;
        }

    #endif
    }
}