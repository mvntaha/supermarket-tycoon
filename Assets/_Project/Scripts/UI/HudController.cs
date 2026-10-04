using Stockwell.Core;
using Stockwell.Input;
using Stockwell.Player;
using TMPro;
using UnityEngine;

namespace Stockwell.UI
{
    /// <summary>
    /// Owns the HUD: shows the interact prompt, and shows the touch controls only when
    /// there is no mouse. Lives on the persistent HUD canvas.
    /// </summary>
    public class HudController : MonoBehaviour
    {
        [Header("Touch")]
        [SerializeField] private GameObject touchControlsRoot;

        [Header("Prompt")]
        [SerializeField] private GameObject promptRoot;
        [SerializeField] private TMP_Text promptLabel;

        [Header("Reticle")]
        [SerializeField] private GameObject reticle;

        [SerializeField] private PlayerInteractor interactor;

        private IInputService _input;

        private void Start()
        {
            _input = GameServices.Get<IInputService>();

            if (touchControlsRoot != null && _input != null)
            {
                touchControlsRoot.SetActive(_input.UsingTouch);
            }

            if (interactor != null)
            {
                interactor.TargetChanged += OnTargetChanged;
            }

            OnTargetChanged(null);
        }

        private void OnDestroy()
        {
            if (interactor != null)
            {
                interactor.TargetChanged -= OnTargetChanged;
            }
        }

        private void OnTargetChanged(string prompt)
        {
            var hasTarget = !string.IsNullOrEmpty(prompt);

            if (promptRoot != null)
            {
                promptRoot.SetActive(hasTarget);
            }

            if (hasTarget && promptLabel != null)
            {
                promptLabel.text = prompt;
            }

            if (reticle != null)
            {
                reticle.SetActive(true);
            }
        }
    }
}
