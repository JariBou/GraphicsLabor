using System;
using UnityEditor;

namespace GraphicsLabor.Scripts.Attributes.LaborerAttributes.DrawerAttributes
{

    public enum MessageLevel
    {
        None,
        Info,
        Warning,
        Error
    }
    
    /// <summary>
    /// Allows to display a box above the field to give a warning or info
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ShowMessageAttribute : DrawerAttribute
    {

        public string Message { get; private set; }
        public MessageLevel MessageType { get; private set; }

        public ShowMessageAttribute(string message, MessageLevel messageType = MessageLevel.None)
        {
            Message = message;
            MessageType = messageType;
        }
    }
    
 

}