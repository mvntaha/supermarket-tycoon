using System;
using Stockwell.Core;
using Stockwell.Input;
using UnityEngine;

namespace Stockwell.Player
{
    /// <summary>
    /// The one raycast. Fires from the camera centre every frame, tracks what is
    /// targeted, and routes the Interact action to it. Everything interactable in
    /// the game goes through here.
    /// </summary>
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private GameSettings settings;

        [Tooltip("Layers the interact ray can hit. Exclude the player's own layer.")]
        [SerializeField] private LayerMask interactMask = ~0;

        private IInputService _input;
        private IInteractable _current;

        /// <summary>Fires with the prompt text, or null when nothing is targeted.</summary>
        public event Action<string> TargetChanged;

        public IInteractable Current => _current;

        private void Start()
        {
            _input = GameServices.Get<IInputService>();

            if (settings == null)
            {
                GameServices.TryGet(out settings);
            }

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }

            if (_input != null)
            {
                _input.Interact += OnInteract;
            }
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.Interact -= OnInteract;
            }
        }

        private void Update()
        {
            if (playerCamera == null)
            {
                return;
            }

            var range = settings != null ? settings.InteractRange : 2.5f;
            var ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

            IInteractable found = null;
            if (Physics.Raycast(ray, out var hit, range, interactMask, QueryTriggerInteraction.Collide))
            {
                // GetComponentInParent so a collider on a child mesh still resolves.
                found = hit.collider.GetComponentInParent<IInteractable>();
                if (found != null && !found.CanInteract)
                {
                    found = null;
                }
            }

            if (!ReferenceEquals(found, _current))
            {
                _current = found;
                TargetChanged?.Invoke(_current?.Prompt);
            }
        }

        private void OnInteract()
        {
            if (_current != null && _current.CanInteract)
            {
                _current.Interact(gameObject);
            }
        }
    }
}
