#if !NodeSystemImplemented
using UnityEditor;
using UnityEditor.Build;

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
        string scriptingDefineSymbols =
 PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
        if (!scriptingDefineSymbols.Contains("NodeSystemImplemented"))
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), scriptingDefineSymbols + ";NodeSystemImplemented");
        }
    }
}
#endif