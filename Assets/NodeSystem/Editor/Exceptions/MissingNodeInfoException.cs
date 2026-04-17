using System;
using UnityEditor;

namespace NodeSystem.Editor.Exceptions
{
    public class MissingNodeInfoException : Exception
    {
        public MissingNodeInfoException(Type typeInfo, SerializedObject serializedObject) : base($"Missing NodeInfo attribute on node of type '{typeInfo.FullName}' used in graph '{serializedObject.targetObject.name}'.")
        {
        }
    }
}