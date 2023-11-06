using UnityEngine;

namespace GraphicsLabor.Scripts.Core.Utility
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
    }
    
    public static class GLogger
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