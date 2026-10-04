using UnityEngine;

namespace Stockwell.UI
{
    /// <summary>
    /// Insets a RectTransform to <see cref="Screen.safeArea"/> so touch controls never
    /// land under a notch, a camera cutout or a rounded corner. Anchoring the HUD from
    /// the start is the cheap order; retrofitting safe areas after the HUD exists is not.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        [Tooltip("Extra inset in pixels applied on every edge, on top of the OS safe area.")]
        [SerializeField] private float extraPadding = 8f;

        private RectTransform _rect;
        private Rect _lastSafeArea;
        private Vector2Int _lastScreen;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            Apply();
        }

        private void Update()
        {
            // Rotation, multi-window and foldables all change this at runtime, and the
            // first frame on Android often reports the pre-inset values.
            if (Screen.safeArea != _lastSafeArea ||
                Screen.width != _lastScreen.x ||
                Screen.height != _lastScreen.y)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (_rect == null)
            {
                return;
            }

            _lastSafeArea = Screen.safeArea;
            _lastScreen = new Vector2Int(Screen.width, Screen.height);

            if (Screen.width <= 0 || Screen.height <= 0)
            {
                return;
            }

            var area = _lastSafeArea;

            // Pad inwards, then clamp so a large padding can never invert the rect.
            var xMin = area.xMin + extraPadding;
            var yMin = area.yMin + extraPadding;
            var xMax = area.xMax - extraPadding;
            var yMax = area.yMax - extraPadding;

            if (xMax <= xMin) { xMin = area.xMin; xMax = area.xMax; }
            if (yMax <= yMin) { yMin = area.yMin; yMax = area.yMax; }

            var min = new Vector2(xMin / Screen.width, yMin / Screen.height);
            var max = new Vector2(xMax / Screen.width, yMax / Screen.height);

            _rect.anchorMin = min;
            _rect.anchorMax = max;
            _rect.offsetMin = Vector2.zero;
            _rect.offsetMax = Vector2.zero;
        }
    }
}
