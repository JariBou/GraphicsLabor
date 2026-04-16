using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace NodeSystem.Editor.Graph.Manipulators
{
    public class NsClickSelector : ClickSelector
    {
        public static bool WasSelectableDescendantHitByMouse(
            GraphElement currentTarget,
            MouseDownEvent evt)
        {
            if (!(evt.target is VisualElement target) || currentTarget == target)
                return false;
            for (VisualElement dest = target; dest != null && currentTarget != dest; dest = dest.parent)
            {
                if (dest is GraphElement graphElement && graphElement.enabledInHierarchy && graphElement.pickingMode != PickingMode.Ignore && graphElement.IsSelectable())
                {
                    Vector2 localPoint = currentTarget.ChangeCoordinatesTo(dest, evt.localMousePosition);
                    if (graphElement.HitTest(localPoint))
                        return true;
                }
            }
            return false;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown_Impl);
        }

        /// <summary>
        ///   <para>Called to unregister event callbacks from the target element.</para>
        /// </summary>
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown_Impl);
        }
        
        protected virtual void OnMouseDown_Impl(MouseDownEvent e)
        {
            if (e.currentTarget is not GraphElement currentTarget || !CanStartManipulation(e) || !currentTarget.IsSelectable() || !currentTarget.HitTest(e.localMousePosition) || WasSelectableDescendantHitByMouse(currentTarget, e))
                return;
            ISelection firstAncestorOfType = currentTarget.GetFirstAncestorOfType<ISelection>();

            if (currentTarget is Edge edge)
            {
                if (e.altKey && firstAncestorOfType is NodeSystemView graphView)
                {
                    edge.output.Disconnect(edge);
                    edge.input.Disconnect(edge);
                    graphView.RemoveConnection(edge);
                    graphView.RemoveElement(edge);
                    firstAncestorOfType.ClearSelection();
                    return;
                }
            }
            
            if (currentTarget.IsSelected((VisualElement) firstAncestorOfType))
            {
                if (e.actionKey)
                    currentTarget.Unselect((VisualElement) firstAncestorOfType);
            }
            else
                currentTarget.Select((VisualElement) firstAncestorOfType, e.actionKey);
            
            e.StopImmediatePropagation();
        }
    }
}