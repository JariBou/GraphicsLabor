using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NodeSystem.Editor.Editors.Fields
{
    public class NsObjectFieldDisplay : VisualElement
    {
        private const string USSClassName = "unity-object-field-display";
        private const string IconUssClassName = USSClassName + "__icon";
        private const string LabelUssClassName = USSClassName + "__label";
        private const string AcceptDropVariantUssClassName = USSClassName + "--accept-drop";

        private readonly NsObjectField _objectField;
        private readonly Image _objectIcon;
        private readonly Label _objectLabel;

        public NsObjectFieldDisplay(NsObjectField objectField)
        {
            AddToClassList(USSClassName);
            Image image = new()
            {
                scaleMode = ScaleMode.ScaleAndCrop, pickingMode = PickingMode.Ignore,
            };
            _objectIcon = image;
            _objectIcon.AddToClassList(IconUssClassName);
            Label label = new()
            {
                pickingMode = PickingMode.Ignore,
            };
            _objectLabel = label;
            _objectLabel.AddToClassList(LabelUssClassName);
            _objectField = objectField;
            Update();
            Add(_objectIcon);
            Add(_objectLabel);
        }

        public void Update()
        {
            GUIContent guiContent = EditorGUIUtility.ObjectContent(_objectField.Value, _objectField.ObjectType);
            _objectIcon.image = guiContent.image;
            _objectLabel.text = guiContent.text;
        }

        [EventInterest(typeof(MouseDownEvent), typeof(KeyDownEvent), typeof(DragUpdatedEvent), typeof(DragPerformEvent),
                       typeof(DragLeaveEvent))]
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);
            switch (evt)
            {
                case null:
                    return;
                case MouseDownEvent { button: 0 } downEvent:
                    OnMouseDown(downEvent);
                    break;
                default:
                {
                    if (evt.eventTypeId == EventBase<KeyDownEvent>.TypeId())
                    {
                        KeyDownEvent keyDownEvent1 = evt as KeyDownEvent;
                        if ((evt is KeyDownEvent keyDownEvent2 ? keyDownEvent2.keyCode == KeyCode.Space ? 1 : 0 : 0) !=
                            0 ||
                            (evt is KeyDownEvent keyDownEvent3
                                ? keyDownEvent3.keyCode == KeyCode.KeypadEnter ? 1 : 0
                                : 0) !=
                            0 || (evt is KeyDownEvent keyDownEvent4 && keyDownEvent4.keyCode == KeyCode.Return))
                            OnKeyboardEnter();
                        else
                        {
                            if (keyDownEvent1 is not { keyCode: KeyCode.Delete } && keyDownEvent1 is not
                                    { keyCode: KeyCode.Backspace })
                                return;

                            OnKeyboardDelete();
                        }
                    }
                    else if (evt.eventTypeId == EventBase<DragUpdatedEvent>.TypeId())
                        OnDragUpdated(evt);
                    else if (evt.eventTypeId == EventBase<DragPerformEvent>.TypeId())
                        OnDragPerform(evt);
                    else
                    {
                        if (evt.eventTypeId != EventBase<DragLeaveEvent>.TypeId()) return;

                        OnDragLeave();
                    }

                    break;
                }
            }
        }

        private void OnDragLeave()
        {
            RemoveFromClassList(AcceptDropVariantUssClassName);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            Object gameObject = _objectField.Value;
            Component component = gameObject as Component;
            if ((bool)(Object)component) gameObject = component?.gameObject;
            if (gameObject == null) return;

            if (evt.clickCount == 1)
            {
                if (!evt.shiftKey && !evt.ctrlKey && (bool)gameObject) EditorGUIUtility.PingObject(gameObject);
            }
            else
            {
                if (evt.clickCount != 2) return;

                if ((bool)gameObject)
                {
                    AssetDatabase.OpenAsset(gameObject);
                    GUIUtility.ExitGUI();
                }
            }

            evt.StopPropagation();
        }

        private void OnKeyboardEnter()
        {
            _objectField.ShowObjectSelector();
            // throw new NotImplementedException();
        }

        private void OnKeyboardDelete()
        {
            _objectField.Value = null;
        }

        private Object DndValidateObject()
        {
            Type objFieldType = _objectField.ObjectType;
            Object[] draggedObjs = DragAndDrop.objectReferences;

            if (objFieldType == null) return draggedObjs[0];

            if (draggedObjs[0] is GameObject reference1 && (objFieldType.IsInterface || typeof(Component).IsAssignableFrom(objFieldType)))
                // ReSharper disable once CoVariantArrayConversion  Justification: Wtf?
                draggedObjs = reference1.GetComponents(typeof(Component));
            return draggedObjs.FirstOrDefault(go => go != null && objFieldType.IsAssignableFrom(go.GetType()));
        }

        private void OnDragUpdated(EventBase evt)
        {
            if (!(DndValidateObject() != null)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            AddToClassList(AcceptDropVariantUssClassName);
            evt.StopPropagation();
        }

        private void OnDragPerform(EventBase evt)
        {
            Object @object = DndValidateObject();
            if (!(@object != null)) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            _objectField.Value = @object;
            DragAndDrop.AcceptDrag();
            RemoveFromClassList(AcceptDropVariantUssClassName);
            evt.StopPropagation();
        }
    }

    public class ObjectFieldSelector : VisualElement
    {
        private readonly NsObjectField _objectField;

        public ObjectFieldSelector(NsObjectField objectField)
        {
            _objectField = objectField;
        }

        [EventInterest(typeof(MouseDownEvent))]
        protected override void HandleEventBubbleUp(EventBase evt)
        {
            base.HandleEventBubbleUp(evt);
            if (evt is not MouseDownEvent { button: 0 }) return;

            _objectField.ShowObjectSelector();
        }
    }
}