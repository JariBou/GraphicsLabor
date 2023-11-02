using System;
using UnityEditor;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes
{
    /// <summary>
    /// Allows to display a box above the field to give a warning or info
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ShowMessageAttribute : DrawerAttribute
    {

        public string Message { get; private set; }
        public MessageType MessageType { get; private set; }

        public ShowMessageAttribute(string message, MessageType messageType = MessageType.None)
        {
            Message = message;
            MessageType = messageType;
        }
    }
    
 

}