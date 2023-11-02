using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Editor.ScriptableObjectParents;
using UnityEngine;

namespace GraphicsLabor.Tests
{
    [CreateAssetMenu(menuName = "Tests/AnotherScriptableObject")]
    public class AnotherEditableScriptableObject : EditableScriptableObject
    {
        public string test;
        public int test2 = 1;
        [ShowProperty] public int TestProp => test2;
    }
}
