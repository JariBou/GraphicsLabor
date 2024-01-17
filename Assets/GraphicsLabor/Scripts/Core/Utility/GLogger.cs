using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

[assembly: InternalsVisibleTo("GraphicsLabor.Editor"), InternalsVisibleTo("GraphicsLabor.Tests")]
namespace GraphicsLabor.Scripts.Core.Utility
{
    internal enum LogLevel
    {
        Info,
        Warning,
        Error,
    }
    
    internal static class GLogger
    {
        private const string Prefix = "GraphicsLabor";
        private const string FormatString = "[{0} - {1}] ";

        public static void Log(string message, LogLevel logLevel = LogLevel.Info)
        {
            Debug.Log(string.Format(FormatString, Prefix, logLevel.ToString()) + message);
        }
        
        public static void LogWarning(string message, LogLevel logLevel = LogLevel.Warning)
        {
            Debug.LogWarning(string.Format(FormatString, Prefix, logLevel.ToString()) + message);
        }
        
        public static void LogError(string message, LogLevel logLevel = LogLevel.Error)
        {
            Debug.LogError(string.Format(FormatString, Prefix, logLevel.ToString()) + message);
        }

    }
}