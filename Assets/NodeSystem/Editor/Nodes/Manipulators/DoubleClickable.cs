using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Nodes.Manipulators
{
    public class DoubleClickable : PointerManipulator
    {
        public event Action<EventBase> ClickedWithEventInfo;

        private readonly long _doubleClickDelayInMs;
        private bool _clicked;
        private IVisualElementScheduledItem _timerObj;

        public DoubleClickable(Action<EventBase> callback, long doubleClickDelayInMs)
        {
            ClickedWithEventInfo = callback;
            _doubleClickDelayInMs = doubleClickDelayInMs;
            activators.Add(new ManipulatorActivationFilter()
            {
                button = MouseButton.LeftMouse,
            });
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            ProcessDownEvent(evt, evt.localPosition);
        }

        private void ProcessDownEvent(PointerDownEvent evt, Vector3 mouseLocalPosition)
        {
            if (_clicked)
            {
                ClickedWithEventInfo?.Invoke(evt);
                _clicked = false;
                evt.StopImmediatePropagation();
            }
            else
            {
                if (_timerObj != null)
                {
                    _timerObj?.ExecuteLater(_doubleClickDelayInMs);
                }
                else
                {
                    _timerObj = target.schedule.Execute(OnTimer).StartingIn(_doubleClickDelayInMs);
                }
                _clicked = true;
            }
        }

        private void OnTimer(TimerState obj)
        {
            Debug.Log("On Timer");
            _clicked = false;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }
        
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
        }
    }
}