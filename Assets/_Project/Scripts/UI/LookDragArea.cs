using Stockwell.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Stockwell.UI
{
    /// <summary>
    /// Right-side transparent drag area that feeds look deltas. Uses pointer events so it
    /// works at the same time as <see cref="TouchStick"/>: each finger is a separate
    /// pointer, and this only reacts to the one that began inside its own rect.
    /// </summary>
    public class LookDragArea : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private InputService input;

        private int _activePointerId = int.MinValue;

        public void OnPointerDown(PointerEventData eventData)
        {
            // Claim the first finger only; a second finger landing here is ignored so
            // the view cannot be yanked by a stray palm touch.
            if (_activePointerId == int.MinValue)
            {
                _activePointerId = eventData.pointerId;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId || input == null)
            {
                return;
            }

            input.AddTouchLook(eventData.delta);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId == _activePointerId)
            {
                _activePointerId = int.MinValue;
            }
        }
    }
}
