using System;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils;
using UnityEditor;
using UnityEngine;

namespace NodeSystem.Editor.Windows
{
    public class NodeSystemSetup : EditorWindow
    {
        private NodeSystemStatus _nodeSystemStatus;
        private float VerticalOffset => EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        private void OnGUI()
        {
            Rect currentRect = EditorGUILayout.GetControlRect();

            NodeSystemBank systemBank = UpdateNodeSystemStatus();

            {
                // Status display
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

                Rect objectRect = new()
                {
                    x = colorRect.x + colorRect.width + EditorGUIUtility.standardVerticalSpacing,
                    y = currentRect.y,
                    width =
                        currentRect.width - colorRect.x - colorRect.width - EditorGUIUtility.standardVerticalSpacing,
                    height = EditorGUIUtility.singleLineHeight
                };

                EditorGUI.BeginDisabledGroup(true);

                EditorGUI.ObjectField(objectRect, systemBank, typeof(NodeSystemBank), false);

                EditorGUI.EndDisabledGroup();
            }

            currentRect.y += VerticalOffset;

            {
                // Log Display
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

            {
                // Create Bank Button
                Rect createNodeBankButtonRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y,
                    width = position.width,
                    height = EditorGUIUtility.singleLineHeight
                };

                EditorGUI.BeginDisabledGroup(_nodeSystemStatus != NodeSystemStatus.NotFound);
                if (GUI.Button(createNodeBankButtonRect, "Create singleton Bank"))
                {
                    GameObject gameObject = new()
                    {
                        name = "[Node System] - S, DDL - Bank"
                    };
                    gameObject.AddComponent<NodeSystemBank>();

                    UpdateNodeSystemStatus(); // Just to check if for whatever reason it failed to create it wont say that it's ok
                }

                EditorGUI.EndDisabledGroup();
            }

            currentRect.y += VerticalOffset;

            {
                // Create ReferenceManagers Button
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
                if (GUI.Button(createReferenceManagersButtonRect, "Create singleton Reference Manager"))
                {
                    GameObject refManager = new()
                    {
                        name = "[Node System] - S, DDL - Reference Manager"
                    };
                    refManager.AddComponent<ReferenceManager>();

                    UpdateNodeSystemStatus(); // Just to check if for whatever reason it failed to create it wont say that it's ok
                }

                EditorGUI.EndDisabledGroup();
            }

            currentRect.y += VerticalOffset;

            {
                // Create Create reference Data Bank Button
                // TODO
                Rect createReferenceManagersButtonRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y,
                    width = position.width,
                    height = EditorGUIUtility.singleLineHeight
                };

                ReferenceDataBank refDataBank = FindAnyObjectByType<ReferenceDataBank>();

                EditorGUI.BeginDisabledGroup(refDataBank != null);
                GUIContent guiContent = new("Create Reference Data Bank in Scene")
                {
                    tooltip = refDataBank != null
                        ? "Reference Data Bank already in Scene"
                        : "Create a new Reference Data Bank in the current Scene"
                };
                if (GUI.Button(createReferenceManagersButtonRect, guiContent))
                {
                    GameObject newRefDataBank = new()
                    {
                        name = "[Node System] - Reference Data Bank"
                    };
                    ReferenceDataBank referenceDataBank = newRefDataBank.AddComponent<ReferenceDataBank>();
                    referenceDataBank.LoadReferences();

                    UpdateNodeSystemStatus(); // Just to check if for whatever reason it failed to create it wont say that it's ok
                }

                EditorGUI.EndDisabledGroup();
            }

            currentRect.y += VerticalOffset;

            {
                // Info Text
                GUIContent textGuiContent =
                    new("Game Object naming tags:\n- S: Singleton\n- DDL: Don't Destroy on Load");
                Vector2 textGuiContentSize = EditorStyles.label.CalcSize(textGuiContent);

                Rect textRect = new()
                {
                    x = currentRect.x,
                    y = currentRect.y,
                    width = position.width,
                    height = textGuiContentSize.y
                };

                EditorGUI.LabelField(textRect, textGuiContent);

                currentRect.y += textRect.height + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        private void OnBecameVisible()
        {
            UpdateNodeSystemStatus();
        }


        private void OnDidOpenScene()
        {
            UpdateNodeSystemStatus();
        }

        private void OnProjectChange()
        {
            throw new NotImplementedException();
        }


        [MenuItem("Window/" + NodeSystemConsts.AddComponentMenuCategoryName + "/Node System Setup")]
        private static void Init()
        {
            GetWindow<NodeSystemSetup>();
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

        private NodeSystemBank UpdateNodeSystemStatus()
        {
            NodeSystemBank nodeSystemBank = FindAnyObjectByType<NodeSystemBank>();
            if (nodeSystemBank == null)
                _nodeSystemStatus = NodeSystemStatus.NotFound;
            else
                _nodeSystemStatus = nodeSystemBank.HasBankAsset()
                    ? NodeSystemStatus.AllSet
                    : NodeSystemStatus.MissingGraphBankAsset;
            return nodeSystemBank;
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

        private enum NodeSystemStatus
        {
            NotFound,
            AllSet,
            MissingGraphBankAsset
        }
    }
}