#if !GraphicsLaborImplemented
using UnityEditor;
using UnityEditor.Build;

public class GraphicsLaborScriptingDefine
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
        string scriptingDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup));
        if (!scriptingDefineSymbols.Contains("GraphicsLaborImplemented"))
        {
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup), scriptingDefineSymbols + ";GraphicsLaborImplemented");
        }
    }
}
#endif