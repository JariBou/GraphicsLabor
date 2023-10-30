using System;
using UnityEditor;
using UnityEngine;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes
{
    /// <summary>
    /// Allows to display a box above the field to give a warning or info
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ShowMessage : PropertyAttribute, ILaborerAttribute
    {

        public string message { get; private set; }
        public MessageType messageType { get; private set; }

        public ShowMessage(string message, MessageType messageType = MessageType.None)
        {
            this.message = message;
            this.messageType = messageType;
        }
    }
    
 

}