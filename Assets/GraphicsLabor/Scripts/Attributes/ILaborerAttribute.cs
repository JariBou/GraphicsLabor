using System;
using UnityEngine;

namespace GraphicsLabor.Scripts.Attributes
{
    public interface ILaborerAttribute
    {
    }

    public class DrawerAttribute : PropertyAttribute, ILaborerAttribute
    {
    }

    public class ScriptableObjectAttribute : Attribute, ILaborerAttribute
    {
    }

    public class InspectedAttribute : Attribute, ILaborerAttribute
    {
    }

    
    
}
