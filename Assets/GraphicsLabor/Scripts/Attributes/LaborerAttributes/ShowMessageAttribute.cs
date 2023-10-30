using System;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    /// <summary>
    /// Allows to display a box above the field to give a warning or info
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ShowMessageAttribute : PropertyAttribute, ILaborerAttribute
    {

        public string Message { get; private set; }
        public MessageType MessageType { get; private set; }

        public ShowMessageAttribute(string message, MessageType messageType = MessageType.None)
        {
            this.Message = message;
            this.MessageType = messageType;
        }
    }
    
 

}