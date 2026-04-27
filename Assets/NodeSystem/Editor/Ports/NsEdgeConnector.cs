using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PointerType = UnityEngine.UIElements.PointerType;

namespace NodeSystem.Editor.Ports
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class NsEdgeConnector<TEdge> : EdgeConnector where TEdge : Edge, new()
    {
        internal const float ConnectionDistanceTreshold = 10f;
        protected bool active;
        protected Edge edgeCandidate;
        protected Vector2 mouseDownPosition;

        public override EdgeDragHelper edgeDragHelper { get; }


        public NsEdgeConnector(IEdgeConnectorListener listener)
        {
            edgeDragHelper = new EdgeDragHelper<TEdge>(listener);
            active = false;
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse,
            });
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback(new EventCallback<PointerDownEvent>(OnPointerDown));
            target.RegisterCallback(new EventCallback<PointerMoveEvent>(OnPointerMove));
            target.RegisterCallback(new EventCallback<PointerUpEvent>(OnPointerUp));
            target.RegisterCallback(new EventCallback<PointerCaptureOutEvent>(OnPointerCaptureOut));
            target.RegisterCallback(new EventCallback<KeyDownEvent>(OnKeyDown));
            target.RegisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown));
            target.RegisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove));
            target.RegisterCallback(new EventCallback<MouseUpEvent>(OnMouseUp));
            target.RegisterCallback(new EventCallback<MouseCaptureOutEvent>(OnCaptureOut));
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback(new EventCallback<PointerDownEvent>(OnPointerDown));
            target.UnregisterCallback(new EventCallback<PointerMoveEvent>(OnPointerMove));
            target.UnregisterCallback(new EventCallback<PointerUpEvent>(OnPointerUp));
            target.UnregisterCallback(new EventCallback<PointerCaptureOutEvent>(OnPointerCaptureOut));
            target.UnregisterCallback(new EventCallback<KeyDownEvent>(OnKeyDown));
            target.UnregisterCallback(new EventCallback<MouseDownEvent>(OnMouseDown));
            target.UnregisterCallback(new EventCallback<MouseMoveEvent>(OnMouseMove));
            target.UnregisterCallback(new EventCallback<MouseUpEvent>(OnMouseUp));
            target.UnregisterCallback(new EventCallback<MouseCaptureOutEvent>(OnCaptureOut));
        }

        protected virtual void OnMouseDown(MouseDownEvent e)
        {
            if (!CanStartManipulation(e)) return;

            OnPointerOrMouseDown(e, e.localMousePosition);
        }

        protected virtual void OnPointerDown(PointerDownEvent e)
        {
            if ((e.pointerId != PointerId.mousePointerId && e.pointerType != PointerType.touch) ||
                !CanStartManipulation(e))
                return;

            OnPointerOrMouseDown(e, e.localPosition);
        }

        protected virtual void OnPointerOrMouseDown(EventBase e, Vector2 localPosition)
        {
            if (active)
                e.StopImmediatePropagation();
            else
            {
                if (target is not Port portTarget) return;

                mouseDownPosition = localPosition;
                edgeCandidate = new TEdge();
                edgeDragHelper.draggedPort = portTarget;
                edgeDragHelper.edgeCandidate = edgeCandidate;
                if (!portTarget.ContainsPoint(localPosition)) return;

                switch (e)
                {
                    case PointerDownEvent evt1:
                        if (edgeDragHelper.HandlePointerDown(evt1))
                        {
                            active = true;
                            target.CapturePointer(evt1.pointerId);
                            e.StopPropagation();
                            return;
                        }

                        break;
                    case MouseDownEvent evt2:
                        if (edgeDragHelper.HandleMouseDown(evt2))
                        {
                            active = true;
                            target.CaptureMouse();
                            e.StopPropagation();
                            return;
                        }

                        break;
                }

                edgeDragHelper.Reset();
                edgeCandidate = null;
            }
        }

        protected virtual void OnCaptureOut(MouseCaptureOutEvent e)
        {
            OnCaptureOut();
        }

        protected virtual void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            OnCaptureOut();
        }

        protected virtual void OnCaptureOut()
        {
            active = false;
            if (edgeCandidate == null) return;

            Abort();
        }

        protected virtual void OnPointerMove(PointerMoveEvent e)
        {
            OnPointerOrMouseMove(e, e.position);
        }

        protected virtual void OnMouseMove(MouseMoveEvent e)
        {
            OnPointerOrMouseMove(e, e.mousePosition);
        }

        protected virtual void OnPointerOrMouseMove(EventBase e, Vector2 position)
        {
            if (!active) return;

            switch (e)
            {
                case PointerMoveEvent evt1:
                    if (!target.HasPointerCapture(evt1.pointerId)) return;

                    edgeDragHelper.HandlePointerMove(evt1);
                    break;
                case MouseMoveEvent evt2:
                    edgeDragHelper.HandleMouseMove(evt2);
                    break;
            }

            edgeCandidate.candidatePosition = position;
            edgeCandidate.UpdateEdgeControl();
            e.StopPropagation();
        }

        protected virtual void OnPointerUp(PointerUpEvent e)
        {
            if (!active || !CanStopManipulation(e)) return;

            OnPointerOrMouseUp(e, e.localPosition);
            target.ReleasePointer(e.pointerId);
        }

        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (!active || !CanStopManipulation(e)) return;

            OnPointerOrMouseUp(e, e.localMousePosition);
            target.ReleaseMouse();
        }

        protected virtual void OnPointerOrMouseUp(EventBase e, Vector2 localPosition)
        {
            if (CanPerformConnection(localPosition))
            {
                switch (e)
                {
                    case PointerUpEvent evt1:
                        edgeDragHelper.HandlePointerUp(evt1);
                        break;
                    case MouseUpEvent evt2:
                        edgeDragHelper.HandleMouseUp(evt2);
                        break;
                }
            }
            else
                Abort();

            active = false;
            edgeCandidate = null;
            e.StopPropagation();
        }

        protected virtual void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode != KeyCode.Escape || !active) return;

            Abort();
            active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        protected virtual void Abort()
        {
            target?.GetFirstAncestorOfType<GraphView>()?.RemoveElement(edgeCandidate);
            edgeCandidate.input = null;
            edgeCandidate.output = null;
            edgeCandidate = null;
            edgeDragHelper.Reset();
        }

        protected virtual bool CanPerformConnection(Vector2 mousePosition)
        {
            return Vector2.Distance(mouseDownPosition, mousePosition) > 10.0;
        }
    }
}