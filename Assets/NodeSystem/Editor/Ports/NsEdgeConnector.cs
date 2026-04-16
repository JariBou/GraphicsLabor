using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using PointerType = UnityEngine.UIElements.PointerType;

namespace NodeSystem.Editor.Ports
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class NsEdgeConnector<TEdge> : EdgeConnector where TEdge : Edge, new()
    {
        internal const float k_ConnectionDistanceTreshold = 10f;
        protected readonly EdgeDragHelper m_EdgeDragHelper;
        protected bool m_Active;
        protected Edge m_EdgeCandidate;
        protected Vector2 m_MouseDownPosition;
        

        public NsEdgeConnector(IEdgeConnectorListener listener)
        {
            m_EdgeDragHelper = new EdgeDragHelper<TEdge>(listener);
            m_Active = false;
            activators.Add(new ManipulatorActivationFilter
            {
                button = MouseButton.LeftMouse
            });
        }

        public override EdgeDragHelper edgeDragHelper => m_EdgeDragHelper;

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
            if (!CanStartManipulation(e))
                return;
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
            if (m_Active)
            {
                e.StopImmediatePropagation();
            }
            else
            {
                if (target is not Port portTarget)
                    return;
                m_MouseDownPosition = localPosition;
                m_EdgeCandidate = new TEdge();
                m_EdgeDragHelper.draggedPort = portTarget;
                m_EdgeDragHelper.edgeCandidate = m_EdgeCandidate;
                if (!portTarget.ContainsPoint(localPosition))
                {
                    return;
                }
                switch (e)
                {
                    case PointerDownEvent evt1:
                        if (m_EdgeDragHelper.HandlePointerDown(evt1))
                        {
                            m_Active = true;
                            target.CapturePointer(evt1.pointerId);
                            e.StopPropagation();
                            return;
                        }

                        break;
                    case MouseDownEvent evt2:
                        if (m_EdgeDragHelper.HandleMouseDown(evt2))
                        {
                            m_Active = true;
                            target.CaptureMouse();
                            e.StopPropagation();
                            return;
                        }

                        break;
                }

                m_EdgeDragHelper.Reset();
                m_EdgeCandidate = null;
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
            m_Active = false;
            if (m_EdgeCandidate == null)
                return;
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
            if (!m_Active)
                return;
            switch (e)
            {
                case PointerMoveEvent evt1:
                    if (!target.HasPointerCapture(evt1.pointerId))
                        return;
                    m_EdgeDragHelper.HandlePointerMove(evt1);
                    break;
                case MouseMoveEvent evt2:
                    m_EdgeDragHelper.HandleMouseMove(evt2);
                    break;
            }

            m_EdgeCandidate.candidatePosition = position;
            m_EdgeCandidate.UpdateEdgeControl();
            e.StopPropagation();
        }

        protected virtual void OnPointerUp(PointerUpEvent e)
        {
            if (!m_Active || !CanStopManipulation(e))
                return;
            OnPointerOrMouseUp(e, e.localPosition);
            target.ReleasePointer(e.pointerId);
        }

        protected virtual void OnMouseUp(MouseUpEvent e)
        {
            if (!m_Active || !CanStopManipulation(e))
                return;
            OnPointerOrMouseUp(e, e.localMousePosition);
            target.ReleaseMouse();
        }

        protected virtual void OnPointerOrMouseUp(EventBase e, Vector2 localPosition)
        {
            if (CanPerformConnection(localPosition))
                switch (e)
                {
                    case PointerUpEvent evt1:
                        m_EdgeDragHelper.HandlePointerUp(evt1);
                        break;
                    case MouseUpEvent evt2:
                        m_EdgeDragHelper.HandleMouseUp(evt2);
                        break;
                }
            else
                Abort();

            m_Active = false;
            m_EdgeCandidate = null;
            e.StopPropagation();
        }

        protected virtual void OnKeyDown(KeyDownEvent e)
        {
            if (e.keyCode != KeyCode.Escape || !m_Active)
                return;
            Abort();
            m_Active = false;
            target.ReleaseMouse();
            e.StopPropagation();
        }

        protected virtual void Abort()
        {
            target?.GetFirstAncestorOfType<GraphView>()?.RemoveElement(m_EdgeCandidate);
            m_EdgeCandidate.input = null;
            m_EdgeCandidate.output = null;
            m_EdgeCandidate = null;
            m_EdgeDragHelper.Reset();
        }

        protected virtual bool CanPerformConnection(Vector2 mousePosition)
        {
            return Vector2.Distance(m_MouseDownPosition, mousePosition) > 10.0;
        }
    }
}