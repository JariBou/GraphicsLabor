using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using Unity.Collections;
using UnityEngine;

namespace GraphicsLabor.Scripts.Core.LaborerTags
{
    [AddComponentMenu("GraphicsLabor/Labor Tags")]
    public class LaborTagComponent : MonoBehaviour
    {
        public LaborTags _tags;
        public LaborTags Tags => _tags;
    }
}