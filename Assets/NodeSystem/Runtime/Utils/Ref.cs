using System;
using System.Runtime.Serialization;
using NodeSystem.Runtime.NodesLibrary.Utils;
using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NodeSystem.Runtime.Utils
{
    [Serializable]
    public class Ref<T> : Object, ISerializable where T : Object
    {
        [SerializeField] private string _objectId;
        public string ObjectId
        {
            get => _objectId; 
            #if UNITY_EDITOR
            set => _objectId = value;
            #else
            private set => _objectId = value;
            #endif
        }

        public T Get()
        {
            return ReferenceManager.GetGameObject<T>(ObjectId);
        }

        public Type GetRefType()
        {
            return typeof(T);
        }

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ObjectValue", _objectId);
        }
    }

    [Serializable]
    public class SerializableRef
    {
        [SerializeField] private string _objectId = "";

        public string ObjectId
        {
            get => _objectId; 
            #if UNITY_EDITOR
            set => _objectId = value;
            #else
            private set => _objectId = value;
            #endif
        }
        
        [SerializeField] private string _refTypename = "";

        public string RefTypename
        {
            get => _refTypename; 
            #if UNITY_EDITOR
            set => _refTypename = value; 
            #else
            private set => _refTypename = value;
            #endif
        }

        public Type GetRefType()
        {
            return Type.GetType(_refTypename);
        }

        public T Get<T>() where T : Object
        {
            return ReferenceManager.GetGameObject<T>(_objectId);
        }
    }
    
    public interface ISerializableTypedRef
    {
        public Type GetRefType();
    }
    
    [Serializable]
    public class SerializableCompRef<T> : ISerializableTypedRef where T : Component
    {
        [FormerlySerializedAs("_objectId")] [SerializeField] private string _compId = ReferenceManager.NoneReference;
        [SerializeField] private string _ownerId = ReferenceManager.NoneReference;

        public string CompId
        {
            get => _compId; 
#if UNITY_EDITOR
            set => _compId = value;
#else
            private set => _compId = value;
#endif
        }
        
        public string OwnerId
        {
            get => _ownerId; 
#if UNITY_EDITOR
            set => _ownerId = value;
#else
            private set => _ownerId = value;
#endif
        }
        
        [SerializeField] private string _refTypename = "";

        public string RefTypename
        {
            get => _refTypename; 
#if UNITY_EDITOR
            set => _refTypename = value; 
#else
            private set => _refTypename = value;
#endif
        }

        public Type GetRefType()
        {
            return typeof(T);
            return Type.GetType(_refTypename);
        }

        public T Get()
        {
            GameObject owner = ReferenceManager.GetGameObject<GameObject>(_ownerId);
            if (owner == null)
            {
                return null;
            }

            GameObjectComponentReferenceBank compRefBank = owner.GetComponent<GameObjectComponentReferenceBank>();
            if (compRefBank == null)
            {
                Debug.LogError($"Missing GameObjectComponentReferenceBank on GameObject '{owner.name}'");
                return null;
            }

            return compRefBank.GetComp<T>(_compId);
        }
    }
    
    [Serializable]
    public class SerializableSourceType
    {
        [SerializeField] private string _objectId = ReferenceManager.NoneReference;

        public string ObjectId
        {
            get => _objectId; 
#if UNITY_EDITOR
            set => _objectId = value;
#else
            private set => _objectId = value;
#endif
        }
        
        [SerializeField] private string _refTypename = "";

        public string RefTypename
        {
            get => _refTypename; 
#if UNITY_EDITOR
            set => _refTypename = value; 
#else
            private set => _refTypename = value;
#endif
        }

        public Type GetRefType()
        {
            return Type.GetType(_refTypename);
        }

        public GameObject Get()
        {
            return ReferenceManager.GetGameObject<GameObject>(_objectId);
        }
    }
    
    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(SerializableSourceType))]
    public class SerializableSourceTypeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();
            
            EditorGUI.BeginProperty(position, label, property);
            SerializableSourceType src = (SerializableSourceType)property.boxedValue;
            SerializedProperty objIdProp = property.FindPropertyRelative("_objectId");
            EditorGUI.BeginChangeCheck();
            GameObject gameObject = src.Get();
            Object obj = EditorGUI.ObjectField(position, gameObject, typeof(GameObject), true);

            if (EditorGUI.EndChangeCheck())
            {
                if (obj is GameObject go)
                {
                    objIdProp.stringValue = ReferenceManager.GetGuidOf(go);
                } else if (obj is null)
                {
                    objIdProp.stringValue = ReferenceManager.NoneReference;
                }
                
                property.serializedObject.ApplyModifiedProperties();
            }
            
            EditorGUI.EndProperty();
            // base.OnGUI(position, property, label);
        }
    }
    
    [CustomPropertyDrawer(typeof(SerializableCompRef<>))]
    public class SerializableCompRefEditor : PropertyDrawer
    {
        private float _cellHeight;

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
                bindingPath = ownerIdProp.propertyPath,
                focusable = true,
                
                tooltip = "The Owner Object of the  selected comp if Any. Double click to highlight in inspector if scene is open.",
                label = "Owner"
            };
            objectField.SetEnabled(false);

            container.Add(objectField);
            

            {
                GameObjectComponentReferenceBank refBank = ownerGo?.GetComponent<GameObjectComponentReferenceBank>();
                Component displayedComp = refBank?.GetComp<Component>(compIdProp.stringValue);
                Type refType = typedRef.GetRefType();
                
                ObjectField compField = new()
                {
                    objectType = refType,
                    value = displayedComp,
                    label = property.displayName,
                    focusable = true,
                };

                // Doesn't work yet, would like not to use GL but might bring over some code from there
                // TooltipAttribute tooltipAttribute = property.GetAttribute<TooltipAttribute>();
                // if (tooltipAttribute != null)
                // {
                //     compField.tooltip = tooltipAttribute.tooltip;
                // }

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
            }
            
            // property.serializedObject.ApplyModifiedProperties();

            return container;
        }
    }
    
    #endif
}