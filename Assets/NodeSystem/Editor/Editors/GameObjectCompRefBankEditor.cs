using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Editor.Editors
{
    [CustomEditor(typeof(GameObjectComponentReferenceBank))]
    public class GameObjectCompRefBankEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            GameObjectComponentReferenceBank referenceDataBank = (GameObjectComponentReferenceBank)target;
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Reference")) referenceDataBank.LoadReferences();
        }
    }
}