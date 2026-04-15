using System;
using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Editor.Windows
{
    public class NodeSystemSetup : EditorWindow
    {
        private float VerticalOffset => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        private enum NodeSystemStatus
        {
            NotFound, AllSet, MissingGraphBankAsset 
        }
        
        private NodeSystemStatus _nodeSystemStatus;
        
        
        [MenuItem("Window/NodeSystem/Node System Setup")]
        private static void Init()
        {
            GetWindow<NodeSystemSetup>();
        }
        

        private void OnDidOpenScene()
        {
            UpdateNodeSystemStatus();
        }

        private void OnProjectChange()
        {
            throw new NotImplementedException();
        }

        private void OnBecameVisible()
        {
            UpdateNodeSystemStatus();
        }

        private Color GetStatusColor()
        {
            return _nodeSystemStatus switch
            {
                NodeSystemStatus.AllSet => Color.green,
                NodeSystemStatus.MissingGraphBankAsset => Color.yellow,
                NodeSystemStatus.NotFound => Color.red,
                _ => Color.black
            };
        }

        private void OnGUI()
        {
            Rect currentRect = EditorGUILayout.GetControlRect();
            
            UpdateNodeSystemStatus();

            { // Status display
                GUIContent bankStatusGuiContent = new("Bank Status: ");
                Vector2 bankStatusGuiContentSize = EditorStyles.label.CalcSize(bankStatusGuiContent);
                Rect textRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y,
                    width = bankStatusGuiContentSize.x,
                    height = EditorGUIUtility.singleLineHeight
                };

                EditorGUI.LabelField(textRect, bankStatusGuiContent);
                
                Rect colorRect = new()
                {
                    x = currentRect.x + bankStatusGuiContentSize.x + EditorGUIUtility.standardVerticalSpacing,
                    y = currentRect.y,
                    width = EditorGUIUtility.singleLineHeight,
                    height = EditorGUIUtility.singleLineHeight
                };

                Color color = GUI.color;
                GUI.color = GetStatusColor();
                Texture2D whiteTexture = new((int)colorRect.width, (int)colorRect.height);
                GUIContent statusGuiContent = new()
                {
                    tooltip = _nodeSystemStatus == NodeSystemStatus.NotFound ? "Missing" : "Found",
                    image = whiteTexture
                };
                GUI.Box(colorRect, statusGuiContent);
                GUI.color = color;
            }
            
            currentRect.y += VerticalOffset;

            { // Log Display
                Rect logRect = new()
                {
                    x = currentRect.x + EditorGUIUtility.singleLineHeight,
                    y = currentRect.y,
                    width = position.width - EditorGUIUtility.singleLineHeight,
                    height = EditorGUIUtility.singleLineHeight
                };
                Color color = GUI.color;
                GUI.color = Color.lightBlue;
                EditorGUI.LabelField(logRect, GetSystemStatusLog());
                GUI.color = color;
            }

            currentRect.y += VerticalOffset * 2;

            { // Create Bank Button
                Rect createNodeBankButtonRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y,
                    width = position.width,
                    height = EditorGUIUtility.singleLineHeight
                };

                EditorGUI.BeginDisabledGroup(_nodeSystemStatus != NodeSystemStatus.NotFound);
                if (GUI.Button(createNodeBankButtonRect, "Create Bank"))
                {
                    GameObject gameObject = new()
                    {
                        name = "[Node System] - Bank",
                    };
                    gameObject.AddComponent<NodeSystemBank>();
                    
                    UpdateNodeSystemStatus(); // Just to check if for whatever reason it failed to create it wont say that it's ok
                }
                EditorGUI.EndDisabledGroup();
            }

            currentRect.y += VerticalOffset;
            
            { // Create ReferenceManagers Button
                // TODO
                Rect createReferenceManagersButtonRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y,
                    width = position.width,
                    height = EditorGUIUtility.singleLineHeight
                };

                ReferenceManager referenceManager = FindAnyObjectByType<ReferenceManager>();

                EditorGUI.BeginDisabledGroup(referenceManager != null);
                if (GUI.Button(createReferenceManagersButtonRect, "Create Reference Manager"))
                {
                    GameObject gameObject = new()
                    {
                        name = "[Node System] - Reference Manager",
                    };
                    gameObject.AddComponent<ReferenceManager>();
                    ReferenceDataBank referenceDataBank = gameObject.AddComponent<ReferenceDataBank>();
                    referenceDataBank.LoadReferences();

                    UpdateNodeSystemStatus(); // Just to check if for whatever reason it failed to create it wont say that it's ok
                }
                EditorGUI.EndDisabledGroup();
            }
            
        }

        private void UpdateNodeSystemStatus()
        {
            NodeSystemBank nodeSystemBank = FindAnyObjectByType<NodeSystemBank>();
            if (nodeSystemBank == null)
            {
                _nodeSystemStatus =  NodeSystemStatus.NotFound;
            }
            else
            {
                _nodeSystemStatus = nodeSystemBank.HasBankAsset()
                    ? NodeSystemStatus.AllSet
                    : NodeSystemStatus.MissingGraphBankAsset;
            }
        }

        private string GetSystemStatusLog()
        {
            return _nodeSystemStatus switch
            {
                NodeSystemStatus.AllSet => "All set!",
                NodeSystemStatus.MissingGraphBankAsset => "Missing graph bank asset on NodeSystemBank component!",
                NodeSystemStatus.NotFound => "NodeSystemBank not found!",
                _ => ""
            };
        }
    }
}