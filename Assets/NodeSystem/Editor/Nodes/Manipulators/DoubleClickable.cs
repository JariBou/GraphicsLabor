using System;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Nodes.Manipulators
{
    public class DoubleClickable : PointerManipulator
    {
        private readonly long _doubleClickDelayInMs;
        private bool _clicked;
        private IVisualElementScheduledItem _timerObj;

        public DoubleClickable(Action<EventBase> callback, long doubleClickDelayInMs)
        {
            ClickedWithEventInfo = callback;
            _doubleClickDelayInMs = doubleClickDelayInMs;
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        public event Action<EventBase> ClickedWithEventInfo;

        private void OnPointerDown(PointerDownEvent evt)
        {
            ProcessDownEvent(evt);
        }

        private void ProcessDownEvent(PointerDownEvent evt)
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
                    _timerObj?.ExecuteLater(_doubleClickDelayInMs);
                else
                    _timerObj = target.schedule.Execute(OnTimer).StartingIn(_doubleClickDelayInMs);
                _clicked = true;
            }
        }

        private void OnTimer(TimerState obj)
        {
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