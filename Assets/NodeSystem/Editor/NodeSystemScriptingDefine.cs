#if !NodeSystemImplemented 
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class NodeSystemScriptingDefine
{
    [InitializeOnLoad]
    public class Autorun
    {
        static Autorun()
        {
            AddNodeSystemScriptingDefine();
        }
    }
    
    static void AddNodeSystemScriptingDefine()
    {
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        if (!scriptingDefineSymbols.Contains("NodeSystemImplemented"))
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, scriptingDefineSymbols + ";NodeSystemImplemented");
        }
    }
}
#endif
#endif


