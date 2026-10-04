using System;
using System.Text;
using Stockwell.World;
using Stockwell.Core;
using TMPro;
using UnityEngine;
using Unity.Profiling;
using UnityEngine.Rendering;

namespace Stockwell.Diagnostics
{
    /// <summary>
    /// Development-build-only frame-time overlay. Reports median and 1% low rather
    /// than an average, because the decision recorded in CLAUDE.md is that "stable"
    /// means 1% low >= 30 fps — an average hides exactly the hitching that matters.
    /// </summary>
    public class PerfOverlay : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject panel;

        [Tooltip("Frames kept for the median and 1% low. 1000 frames is ~17 s at 60 fps.")]
        [SerializeField] private int sampleCount = 1000;

        [SerializeField] private float refreshInterval = 0.5f;
        [SerializeField] private bool startVisible = true;

        private float[] _samples;
        private float[] _sorted;
        private int _writeIndex;
        private int _filled;
        private float _nextRefresh;
        private readonly StringBuilder _sb = new(256);

        private ProfilerRecorder _drawCalls;
        private ProfilerRecorder _triangles;
        private ProfilerRecorder _setPass;
        private bool _recordersValid;

        private void Awake()
        {
            // The overlay must not exist in a release build at all.
            if (!Debug.isDebugBuild && !Application.isEditor)
            {
                Destroy(gameObject);
                return;
            }

            _samples = new float[Mathf.Max(60, sampleCount)];
            _sorted = new float[_samples.Length];
            SetVisible(startVisible);
        }

        private void OnEnable()
        {
            if (_samples == null)
            {
                return;
            }

            // These counters exist only when the Profiler module is available; on a
            // release player they come back invalid, hence the Valid checks below.
            _drawCalls = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
            _triangles = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");
            _setPass = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");
            _recordersValid = true;
        }

        private void OnDisable()
        {
            if (!_recordersValid)
            {
                return;
            }

            if (_drawCalls.Valid) _drawCalls.Dispose();
            if (_triangles.Valid) _triangles.Dispose();
            if (_setPass.Valid) _setPass.Dispose();
            _recordersValid = false;
        }

        private void Update()
        {
            if (_samples == null)
            {
                return;
            }

            var ms = Time.unscaledDeltaTime * 1000f;
            _samples[_writeIndex] = ms;
            _writeIndex = (_writeIndex + 1) % _samples.Length;
            _filled = Mathf.Min(_filled + 1, _samples.Length);

            if (Time.unscaledTime < _nextRefresh || label == null)
            {
                return;
            }

            _nextRefresh = Time.unscaledTime + refreshInterval;
            Refresh(ms);
        }

        private void Refresh(float currentMs)
        {
            Array.Copy(_samples, _sorted, _filled);
            Array.Sort(_sorted, 0, _filled);

            var median = _sorted[_filled / 2];

            // 1% low frame rate = the slowest 1% of frames, so the 99th percentile frame time.
            var idx = Mathf.Clamp(Mathf.CeilToInt(_filled * 0.99f) - 1, 0, _filled - 1);
            var onePercentLowMs = _sorted[idx];

            _sb.Clear();
            _sb.Append("cur ").Append(currentMs.ToString("F1")).Append(" ms\n");
            _sb.Append("median ").Append(median.ToString("F1")).Append(" ms (")
               .Append((1000f / Mathf.Max(median, 0.0001f)).ToString("F0")).Append(" fps)\n");
            _sb.Append("1% low ").Append((1000f / Mathf.Max(onePercentLowMs, 0.0001f)).ToString("F0"))
               .Append(" fps (").Append(onePercentLowMs.ToString("F1")).Append(" ms)\n");

            if (_recordersValid && _drawCalls.Valid)
            {
                _sb.Append("draw ").Append(_drawCalls.LastValue);
                if (_setPass.Valid) _sb.Append("  setpass ").Append(_setPass.LastValue);
                _sb.Append('\n');
            }
            else
            {
                _sb.Append("draw n/a\n");
            }

            if (_recordersValid && _triangles.Valid)
            {
                _sb.Append("tris ").Append(_triangles.LastValue).Append('\n');
            }
            else
            {
                _sb.Append("tris n/a\n");
            }

            if (GameServices.TryGet<ISceneService>(out var scenes) && !string.IsNullOrEmpty(scenes.CurrentScene))
            {
                _sb.Append("scene ").Append(scenes.CurrentScene).Append('\n');
            }

            _sb.Append("samples ").Append(_filled);

            label.text = _sb.ToString();
        }

        /// <summary>Hooked to the debug button in the HUD.</summary>
        public void Toggle() => SetVisible(panel == null || !panel.activeSelf);

        private void SetVisible(bool visible)
        {
            if (panel != null)
            {
                panel.SetActive(visible);
            }
        }

        /// <summary>Resets the rolling window, so a PERF_LOG reading is per-scene.</summary>
        public void ResetSamples()
        {
            _filled = 0;
            _writeIndex = 0;
        }
    }
}
