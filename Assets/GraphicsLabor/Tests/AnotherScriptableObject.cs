using GraphicsLabor.Scripts.Editor.Windows;
using UnityEngine;

namespace GraphicsLabor.Tests
{
    [CreateAssetMenu(menuName = "Tests/AnotherScriptableObject")]
    public class AnotherScriptableObject : EditableScriptableObject
    {
        public string test;
        public int test2;
    }
}
