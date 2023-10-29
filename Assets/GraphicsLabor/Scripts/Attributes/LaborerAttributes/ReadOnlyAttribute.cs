using System;
using UnityEngine;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    /// <summary>
    /// Allows to display an attribute as ReadOnly
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ReadOnly : PropertyAttribute, ILaborerAttribute
    {

        public readonly string _overrideName;

        public ReadOnly(string overrideName = null)
        {
            _overrideName = overrideName;
        }
    }
    
 

}