using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Editor.ScriptableObjectParents;
using GraphicsLabor.Scripts.Editor.Windows;
using UnityEngine;

namespace GraphicsLabor.Tests
{
    [CreateAssetMenu(menuName = "Tests/TestScriptableObject")]
    public class TestEditableScriptableObject : EditableScriptableObject
    {
        public string testString = "test";
        public int testInt = 20;
        public SerializableTestClass SerializableTestClass;
        [TabProperty("FirstOne", "AnotherOne")]public int testInt2 = 25;
        [TabProperty("FirstOne")]public float testFloat = 56.2f;
        [TabProperty("AnotherOne")]public GameObject testGameObject;
        [TabProperty("AnotherOne")]public SerializableTestClass SerializableTestClass2;
        [Expandable, TabProperty("AnotherOne")] public AnotherEditableScriptableObject testSo;
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
