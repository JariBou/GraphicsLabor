using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Editor.Editors
{
    [CustomEditor(typeof(ReferenceDataBank))]
    public class ReferenceManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            ReferenceDataBank referenceDataBank = (ReferenceDataBank)target;
            base.OnInspectorGUI();
        
            if (GUILayout.Button("Add Reference"))
            {
                referenceDataBank.LoadReferences();
            }
        }
    }
}
