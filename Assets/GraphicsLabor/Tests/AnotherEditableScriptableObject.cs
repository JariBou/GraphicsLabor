using GraphicsLabor.Scripts.Editor.ScriptableObjectParents;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEngine;

namespace GraphicsLabor.Tests
{
    [CreateAssetMenu(menuName = "Tests/AnotherScriptableObject")]
    public class AnotherEditableScriptableObject : EditableScriptableObject
    {
        public string test;
        public int test2;
    }
}
