using System;
using System.Collections.Generic;
using UnityEngine;

namespace GraphicsLabor.Tests
{
    [CreateAssetMenu(menuName = "Tests/TestScriptableObject")]
    public class TestScriptableObject : ScriptableObject
    {
        public string testString = "test";
        public int testInt = 20;
        public SerializableTestClass SerializableTestClass;
        public int testInt2 = 25;
        public float testFloat = 56.2f;
        public GameObject testGameObject;
        public SerializableTestClass SerializableTestClass2;
    }

    [Serializable]
    public class SerializableTestClass
    {
        public int testInt;
        public SerializableTestClassButDifferent SerializableTestClassButDifferent;
    }
    
    [Serializable]
    public class SerializableTestClassButDifferent
    {
        public int testInt;
        public string testString;
        public float testFloat;
        public int testInt2;
        public List<int> testListInt = new();
    }
}
