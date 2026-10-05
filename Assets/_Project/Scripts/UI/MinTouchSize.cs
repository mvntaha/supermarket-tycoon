using Stockwell.Core;
using UnityEngine;

namespace Stockwell.UI
{
    /// <summary>
    /// Enforces a physical minimum size on a touch target. A 48 dp button is the Android
    /// accessibility floor, and "48 pixels" is not the same thing on a 440 dpi phone, so
    /// the size is computed from <see cref="Screen.dpi"/> and the canvas scale factor.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class MinTouchSize : MonoBehaviour
    {
        [SerializeField] private GameSettings settings;

        [Tooltip("Override in dp. 0 means use GameSettings.MinTouchTargetDp.")]
        [SerializeField] private float overrideDp;

        [Tooltip("Apply to width, height, or both.")]
        [SerializeField] private bool applyWidth = true;
        [SerializeField] private bool applyHeight = true;

        private const float ReferenceDpi = 160f; // 1 dp == 1 px at 160 dpi, by definition.

        private RectTransform _rect;
        private Canvas _canvas;
        private Vector2Int _lastScreen;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _canvas = GetComponentInParent<Canvas>();
            Apply();
        }

        private void Update()
        {
            if (Screen.width != _lastScreen.x || Screen.height != _lastScreen.y)
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

            _lastScreen = new Vector2Int(Screen.width, Screen.height);

            var dp = overrideDp > 0f
                ? overrideDp
                : (settings != null ? settings.MinTouchTargetDp : 48f);

            // Screen.dpi is 0 on some devices and in the editor; fall back to 1 dp = 1 px.
            var dpi = Screen.dpi > 1f ? Screen.dpi : ReferenceDpi;
            var pixels = dp * (dpi / ReferenceDpi);

            // The canvas scaler maps canvas units to pixels, so divide it back out.
            var scale = _canvas != null && _canvas.scaleFactor > 0.0001f ? _canvas.scaleFactor : 1f;
            var units = pixels / scale;

            var size = _rect.sizeDelta;
            if (applyWidth) size.x = Mathf.Max(size.x, units);
            if (applyHeight) size.y = Mathf.Max(size.y, units);
            _rect.sizeDelta = size;
        }
    }
}
