using System;
using System.Runtime.Serialization;
using NodeSystem.Runtime.References;
using UnityEditor;
using UnityEngine;
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
    public class SerializableTypedRef<T> : ISerializableTypedRef where T : MonoBehaviour
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

        public T Get()
        {
            return ReferenceManager.GetGameObject<T>(_objectId);
        }
    }
    
    [Serializable]
    public class SerializableSourceType
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
    
    [CustomPropertyDrawer(typeof(SerializableTypedRef<>))]
    public class SerializableTypedRefEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            ISerializableTypedRef typedRef = (ISerializableTypedRef)property.boxedValue;

            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty objIdProp = property.FindPropertyRelative("_objectId");
            EditorGUI.BeginChangeCheck();
            GameObject gameObject =  objIdProp.stringValue == ReferenceManager.NoneReference ? null : ReferenceManager.GetGameObject<Object>(objIdProp.stringValue);
            Object obj = EditorGUI.ObjectField(position, gameObject, typedRef.GetRefType(), true);

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
    
    #endif
}