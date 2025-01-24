using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReferenceDataBank))]
public class ReferenceManagerEditor : Editor
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
