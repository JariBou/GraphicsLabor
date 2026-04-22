using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.Fields
{
    public class NsObjectFieldDisplay : VisualElement
    {
        private static readonly string ussClassName = "unity-object-field-display";
        private static readonly string iconUssClassName = ussClassName + "__icon";
        private static readonly string labelUssClassName = ussClassName + "__label";
        private static readonly string acceptDropVariantUssClassName = ussClassName + "--accept-drop";
        private readonly NsObjectField m_ObjectField;
        private readonly Image m_ObjectIcon;
        private readonly Label m_ObjectLabel;

        public NsObjectFieldDisplay(NsObjectField objectField)
        {
            AddToClassList(ussClassName);
            Image image = new();
            image.scaleMode = ScaleMode.ScaleAndCrop;
            image.pickingMode = PickingMode.Ignore;
            m_ObjectIcon = image;
            m_ObjectIcon.AddToClassList(iconUssClassName);
            Label label = new();
            label.pickingMode = PickingMode.Ignore;
            m_ObjectLabel = label;
            m_ObjectLabel.AddToClassList(labelUssClassName);
            m_ObjectField = objectField;
            Update();
            Add(m_ObjectIcon);
            Add(m_ObjectLabel);
        }

        public void Update()
        {
            GUIContent guiContent = EditorGUIUtility.ObjectContent(m_ObjectField.Value, m_ObjectField.ObjectType);
            m_ObjectIcon.image = guiContent.image;
            m_ObjectLabel.text = guiContent.text;
        }

        [EventInterest(typeof(MouseDownEvent), typeof(KeyDownEvent), typeof(DragUpdatedEvent), typeof(DragPerformEvent),
            typeof(DragLeaveEvent))]
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);
            if (evt == null)
                return;
            // ISSUE: explicit non-virtual call
            if (evt is MouseDownEvent mouseDownEvent && mouseDownEvent.button == 0)
            {
                OnMouseDown(evt as MouseDownEvent);
            }
            else if (evt.eventTypeId == EventBase<KeyDownEvent>.TypeId())
            {
                KeyDownEvent keyDownEvent1 = evt as KeyDownEvent;
                // ISSUE: explicit non-virtual call
                // ISSUE: explicit non-virtual call
                // ISSUE: explicit non-virtual call
                if ((evt is KeyDownEvent keyDownEvent2 ? keyDownEvent2.keyCode == KeyCode.Space ? 1 : 0 : 0) != 0 ||
                    (evt is KeyDownEvent keyDownEvent3 ? keyDownEvent3.keyCode == KeyCode.KeypadEnter ? 1 : 0 : 0) !=
                    0 || (evt is KeyDownEvent keyDownEvent4 && keyDownEvent4.keyCode == KeyCode.Return))
                {
                    OnKeyboardEnter();
                }
                else
                {
                    if (keyDownEvent1.keyCode != KeyCode.Delete && keyDownEvent1.keyCode != KeyCode.Backspace)
                        return;
                    OnKeyboardDelete();
                }
            }
            else if (evt.eventTypeId == EventBase<DragUpdatedEvent>.TypeId())
            {
                OnDragUpdated(evt);
            }
            else if (evt.eventTypeId == EventBase<DragPerformEvent>.TypeId())
            {
                OnDragPerform(evt);
            }
            else
            {
                if (evt.eventTypeId != EventBase<DragLeaveEvent>.TypeId())
                    return;
                OnDragLeave();
            }
        }

        private void OnDragLeave()
        {
            RemoveFromClassList(acceptDropVariantUssClassName);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            Object gameObject = m_ObjectField.Value;
            Component component = gameObject as Component;
            if ((bool)(Object)component)
                gameObject = component.gameObject;
            if (gameObject == null)
                return;
            if (evt.clickCount == 1)
            {
                if (!evt.shiftKey && !evt.ctrlKey && (bool)gameObject)
                    EditorGUIUtility.PingObject(gameObject);
                evt.StopPropagation();
            }
            else
            {
                if (evt.clickCount != 2)
                    return;
                if ((bool)gameObject)
                {
                    AssetDatabase.OpenAsset(gameObject);
                    GUIUtility.ExitGUI();
                }

                evt.StopPropagation();
            }
        }

        private void OnKeyboardEnter()
        {
            this.m_ObjectField.ShowObjectSelector();
            // throw new NotImplementedException();
        }

        private void OnKeyboardDelete()
        {
            m_ObjectField.Value = null;
        }

        private Object DndValidateObject()
        {
            Type objType = m_ObjectField.ObjectType;
            Object[] references = DragAndDrop.objectReferences;

            if (objType == null) return references[0];
            if (references[0] is GameObject reference1 &&
                (objType.IsInterface || typeof(Component).IsAssignableFrom(objType)))
                // ReSharper disable once CoVariantArrayConversion  Justification: Wtf?
                references = reference1.GetComponents(typeof(Component));
            foreach (Object reference2 in references)
                if (reference2 != null && objType.IsAssignableFrom(reference2.GetType()))
                    return reference2;
            return null;
        }

        private void OnDragUpdated(EventBase evt)
        {
            if (!(DndValidateObject() != null))
                return;
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            AddToClassList(acceptDropVariantUssClassName);
            evt.StopPropagation();
        }

        private void OnDragPerform(EventBase evt)
        {
            Object @object = DndValidateObject();
            if (!(@object != null))
                return;
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            m_ObjectField.Value = @object;
            DragAndDrop.AcceptDrag();
            RemoveFromClassList(acceptDropVariantUssClassName);
            evt.StopPropagation();
        }
    }
    
    public class ObjectFieldSelector : VisualElement
    {
        private readonly NsObjectField m_ObjectField;

        public ObjectFieldSelector(NsObjectField objectField) => this.m_ObjectField = objectField;

        [EventInterest(new System.Type[] {typeof (MouseDownEvent)})]
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);
            // ISSUE: explicit non-virtual call
            if (!(evt is MouseDownEvent mouseDownEvent) || (mouseDownEvent.button) != 0)
                return;
            // TODO
            this.m_ObjectField.ShowObjectSelector();
        }
    }
}