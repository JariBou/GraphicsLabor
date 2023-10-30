using System;
using UnityEngine;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ExpandableAttribute : PropertyAttribute, ILaborerAttribute
    {
    }
}