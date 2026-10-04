using System;
using Stockwell.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Stockwell.Input
{
    /// <summary>
    /// Merges the Input System action map (PC) with the on-screen touch HUD into one
    /// <see cref="IInputService"/>. Touch widgets push their values in each frame via
    /// the Set/Add methods; the action map is polled. Whichever is larger wins, so a
    /// device with both never fights itself.
    /// </summary>
    public class InputService : MonoBehaviour, IInputService
    {
        [SerializeField] private InputActionAsset actions;
        [SerializeField] private GameSettings settings;

        private InputAction _move;
        private InputAction _look;
        private InputAction _interact;
        private InputAction _pickUpDrop;
        private InputAction _place;

        // Pushed by the touch HUD, consumed and cleared each frame.
        private Vector2 _touchMove;
        private Vector2 _touchLookDelta;

        public Vector2 Move { get; private set; }
        public Vector2 Look { get; private set; }

        public event Action Interact;
        public event Action PickUpDrop;
        public event Action Place;

        /// <summary>
        /// Touch HUD is shown when no mouse is connected. A phone with an OTG mouse
        /// attached therefore gets PC-style controls, which is the behaviour we want.
        /// </summary>
        public bool UsingTouch => Mouse.current == null;

        private void Awake()
        {
            if (actions == null)
            {
                Debug.LogError("[InputService] No InputActionAsset assigned.");
                return;
            }

            var map = actions.FindActionMap("Gameplay", throwIfNotFound: false);
            if (map == null)
            {
                Debug.LogError("[InputService] Action map 'Gameplay' not found in the asset.");
                return;
            }

            _move = map.FindAction("Move");
            _look = map.FindAction("Look");
            _interact = map.FindAction("Interact");
            _pickUpDrop = map.FindAction("PickUpDrop");
            _place = map.FindAction("Place");

            map.Enable();
        }

        private void OnEnable()
        {
            if (_interact != null) _interact.performed += OnInteractPerformed;
            if (_pickUpDrop != null) _pickUpDrop.performed += OnPickUpDropPerformed;
            if (_place != null) _place.performed += OnPlacePerformed;
        }

        private void OnDisable()
        {
            if (_interact != null) _interact.performed -= OnInteractPerformed;
            if (_pickUpDrop != null) _pickUpDrop.performed -= OnPickUpDropPerformed;
            if (_place != null) _place.performed -= OnPlacePerformed;
        }

        private void OnInteractPerformed(InputAction.CallbackContext _) => Interact?.Invoke();
        private void OnPickUpDropPerformed(InputAction.CallbackContext _) => PickUpDrop?.Invoke();
        private void OnPlacePerformed(InputAction.CallbackContext _) => Place?.Invoke();

        private void Update()
        {
            var actionMove = _move?.ReadValue<Vector2>() ?? Vector2.zero;
            Move = actionMove.sqrMagnitude > _touchMove.sqrMagnitude ? actionMove : _touchMove;

            // Mouse delta is per-frame already; touch delta is accumulated by the drag area.
            var mouseDelta = _look?.ReadValue<Vector2>() ?? Vector2.zero;
            var look = mouseDelta * (settings != null ? settings.MouseLookSensitivity : 0.12f)
                       + _touchLookDelta * (settings != null ? settings.TouchLookSensitivity : 0.08f);

            if (settings != null && settings.InvertLookY)
            {
                look.y = -look.y;
            }

            Look = look;
            _touchLookDelta = Vector2.zero;
        }

        // --- called by the touch HUD ---

        public void SetTouchMove(Vector2 value) => _touchMove = Vector2.ClampMagnitude(value, 1f);
        public void ClearTouchMove() => _touchMove = Vector2.zero;
        public void AddTouchLook(Vector2 delta) => _touchLookDelta += delta;

        public void FireInteract() => Interact?.Invoke();
        public void FirePickUpDrop() => PickUpDrop?.Invoke();
        public void FirePlace() => Place?.Invoke();
    }
}
