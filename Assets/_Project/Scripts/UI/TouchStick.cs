using Stockwell.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Stockwell.UI
{
    /// <summary>
    /// Left-hand virtual stick. Written against Unity UI pointer events rather than the
    /// Input System's OnScreenStick so that it and <see cref="LookDragArea"/> can be
    /// driven by two fingers at once — the EventSystem tracks a pointer per finger.
    /// </summary>
    public class TouchStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [SerializeField] private InputService input;
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;

        [Tooltip("Travel of the handle from centre, in pixels, for a full-magnitude input.")]
        [SerializeField] private float radius = 90f;

        private Vector2 _value;

        private void Awake()
        {
            if (background == null)
            {
                background = transform as RectTransform;
            }
        }

        public void OnPointerDown(PointerEventData eventData) => UpdateStick(eventData);

        public void OnDrag(PointerEventData eventData) => UpdateStick(eventData);

        public void OnPointerUp(PointerEventData eventData)
        {
            _value = Vector2.zero;

            if (handle != null)
            {
                handle.anchoredPosition = Vector2.zero;
            }

            if (input != null)
            {
                input.ClearTouchMove();
            }
        }

        private void UpdateStick(PointerEventData eventData)
        {
            if (background == null)
            {
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out var local);

            _value = Vector2.ClampMagnitude(local / radius, 1f);

            if (handle != null)
            {
                handle.anchoredPosition = _value * radius;
            }

            if (input != null)
            {
                input.SetTouchMove(_value);
            }
        }
    }
}
