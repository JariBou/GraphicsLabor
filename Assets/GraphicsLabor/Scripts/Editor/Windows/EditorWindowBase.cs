using UnityEditor;

namespace GraphicsLabor.Scripts.Editor.Windows
{
    public abstract class EditorWindowBase : EditorWindow
    {
        public abstract void OnGUI();
    }
}