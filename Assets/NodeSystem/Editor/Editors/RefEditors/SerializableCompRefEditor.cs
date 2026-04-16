using System;
using NodeSystem.Runtime.References;
using NodeSystem.Runtime.Utils.RefSystem;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.RefEditors
{
    [CustomPropertyDrawer(typeof(SerializableCompRef<>))]
    public class SerializableCompRefEditor : PropertyDrawer
    {
        private float _cellHeight;

        // Idk why Here Unity uses the VisualElement one instead of this one but hey... Unity being Unity again I guess
        /*public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            ISerializableTypedRef typedRef = (ISerializableTypedRef)property.boxedValue;
        
            EditorGUI.BeginProperty(position, label, property);
            
            EditorGUI.LabelField(position, label);
            SerializedProperty ownerIdProp = property.FindPropertyRelative("_ownerId");
            SerializedProperty compIdProp = property.FindPropertyRelative("_compId");
            EditorGUI.BeginChangeCheck();
            
            EditorGUI.BeginDisabledGroup(true);
            
            GameObject ownerGo =  ownerIdProp.stringValue == ReferenceManager.NoneReference ? null : ReferenceManager.GetGameObject<GameObject>(ownerIdProp.stringValue);
            
            float cellWidth = position.width / 3;
            _cellHeight = 18f;
            {
                Rect drawRect = new Rect()
                {
                    x = position.x,
                    y = position.y,
                    width = position.width,
                    height = _cellHeight,
                };
        
                EditorGUI.ObjectField(drawRect, ownerGo, typeof(GameObject), true);
            }
            
            EditorGUI.EndDisabledGroup();
        
            GameObjectComponentReferenceBank refBank = ownerGo?.GetComponent<GameObjectComponentReferenceBank>();
            Component displayedComp = refBank?.GetComp<Component>(compIdProp.stringValue);
        
            Type refType = typedRef.GetRefType();
            Object obj;
            {
                Rect drawRect = new Rect()
                {
                    x = position.x,
                    y = position.y + _cellHeight,
                    width = position.width,
                    height = _cellHeight,
                };
                
                obj = EditorGUI.ObjectField(drawRect, displayedComp, refType, true);
            }
        
            if (EditorGUI.EndChangeCheck())
            {
                if (obj is Component comp)
                {
                    ownerIdProp.stringValue = ReferenceManager.GetGuidOf(comp.gameObject);
                    GameObjectComponentReferenceBank gameObjectComponentReferenceBank = comp.gameObject.GetComponent<GameObjectComponentReferenceBank>();
                    if (gameObjectComponentReferenceBank == null)
                    {
                        gameObjectComponentReferenceBank = comp.gameObject.AddComponent<GameObjectComponentReferenceBank>();
                    }
                    gameObjectComponentReferenceBank.LoadReferences();
                    compIdProp.stringValue = gameObjectComponentReferenceBank.GetGuidOf(comp);
                } else if (obj is null)
                {
                    ownerIdProp.stringValue = ReferenceManager.NoneReference;
                    compIdProp.stringValue = ReferenceManager.NoneReference;
                }
                
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
            // base.OnGUI(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) * 2;
        }*/
        

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        { 
            ISerializableTypedRef typedRef = (ISerializableTypedRef)property.boxedValue;

            VisualElement container = new()
            {
                style =
                {
                    flexGrow = 1,
                }
            };
            
            SerializedProperty ownerIdProp = property.FindPropertyRelative("_ownerId");
            SerializedProperty compIdProp = property.FindPropertyRelative("_compId");
            GameObject ownerGo = ownerIdProp.stringValue == ReferenceManager.NoneReference
                ? null
                : ReferenceManager.GetGameObject<GameObject>(ownerIdProp.stringValue);

            
            // Game Object
            ObjectField objectField = new()
            {
                objectType = typeof(GameObject),
                value = ownerGo,
                focusable = true,
                
                tooltip = "The Owner Object of the  selected comp if Any. Double click to highlight in inspector if scene is open.",
                label = "Owner"
            };
            objectField.SetEnabled(false);

            container.Add(objectField);
            
            GameObjectComponentReferenceBank refBank = ownerGo?.GetComponent<GameObjectComponentReferenceBank>();
            Component displayedComp = refBank?.GetComp<Component>(compIdProp.stringValue);
            Type refType = typedRef.GetRefType();
            
            ObjectField compField = new()
            {
                objectType = refType,
                value = displayedComp,
                label = property.displayName,
                focusable = true,
                tooltip = property.tooltip
            };

            compField.RegisterValueChangedCallback(evt =>
            {
                property.serializedObject.Update();
                ownerIdProp.serializedObject.Update();
                
                Object obj = evt.newValue;
                switch (obj)
                {
                    case Component comp:
                    {
                        ownerIdProp.stringValue = ReferenceManager.GetGuidOf(comp.gameObject);
                        GameObjectComponentReferenceBank gameObjectComponentReferenceBank =
                            comp.gameObject.GetComponent<GameObjectComponentReferenceBank>();
                        if (gameObjectComponentReferenceBank == null)
                        {
                            gameObjectComponentReferenceBank =
                                comp.gameObject.AddComponent<GameObjectComponentReferenceBank>();
                        }

                        gameObjectComponentReferenceBank.LoadReferences();
                        compIdProp.stringValue = gameObjectComponentReferenceBank.GetGuidOf(comp);
                        objectField.SetValueWithoutNotify(comp.gameObject); // This is soooo weird, I can't stress it enough but hey... it works
                        break;
                    }
                    case null:
                    {
                        ownerIdProp.stringValue = ReferenceManager.NoneReference;
                        compIdProp.stringValue = ReferenceManager.NoneReference;
                        objectField.SetValueWithoutNotify(null);
                        break;
                    }
                }
                
                compIdProp.serializedObject.ApplyModifiedProperties();
                property.serializedObject.ApplyModifiedProperties();
            });

            container.Add(compField);
            
            return container;
        }
    }
}