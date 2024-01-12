using System;
using System.Collections.Generic;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.InspectedAttributes;
using GraphicsLabor.Scripts.Attributes.LaborerAttributes.ScriptableObjectAttributes;
using UnityEngine;

namespace GraphicsLabor.Tests
{
    [CreateAssetMenu(menuName = "Tests/TestScriptableObject"), Manageable, Editable]
    public class TestEditableScriptableObject : ScriptableObject
    {
        public string testString = "test";
        public int testInt = 20;
        public SerializableTestClass SerializableTestClass;
        [TabProperty("FirstOne", "AnotherOne")]public int testInt2 = 25;
        [TabProperty("FirstOne")]public float testFloat = 56.2f;
        [ShowProperty] public float testProp2 => testFloat;
        public List<string> testList;
        [Expandable, TabProperty("AnotherOne")] public AnotherEditableScriptableObject TestSO;
        [TabProperty("Terasse", "FirstOne"), ShowProperty] public float testProp => testFloat;
        [TabProperty("AnotherOne")]public GameObject testGameObject;
        [TabProperty("AnotherOne")]public SerializableTestClass SerializableTestClass2;
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
        [Expandable]public AnotherEditableScriptableObject testezrr;

    }
}
