using Stockwell.Core;
using Stockwell.Input;
using UnityEngine;

namespace Stockwell.Player
{
    /// <summary>
    /// First-person walk and look on a CharacterController. Deliberately plain:
    /// no head bob, no sprint, no crouch — Phase 1 only needs to prove the controls
    /// work on a phone.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private GameSettings settings;

        private CharacterController _controller;
        private IInputService _input;
        private float _pitch;
        private float _verticalVelocity;

        /// <summary>Horizontal speed in m/s, read by the footstep player.</summary>
        public float PlanarSpeed { get; private set; }
        public bool IsGrounded => _controller != null && _controller.isGrounded;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            if (cameraPivot == null)
            {
                Debug.LogError("[PlayerController] No camera pivot assigned.");
            }
        }

        private void Start()
        {
            _input = GameServices.Get<IInputService>();

            if (settings == null)
            {
                GameServices.TryGet(out settings);
            }
        }

        private void Update()
        {
            if (_input == null)
            {
                return;
            }

            ApplyLook(_input.Look);
            ApplyMove(_input.Move);
        }

        private void ApplyLook(Vector2 look)
        {
            if (Mathf.Approximately(look.x, 0f) && Mathf.Approximately(look.y, 0f))
            {
                return;
            }

            // Yaw turns the body, pitch only tilts the camera, so movement stays horizontal.
            transform.Rotate(Vector3.up, look.x, Space.Self);

            var clamp = settings != null ? settings.PitchClamp : 85f;
            _pitch = Mathf.Clamp(_pitch - look.y, -clamp, clamp);

            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
            }
        }

        private void ApplyMove(Vector2 move)
        {
            var speed = settings != null ? settings.WalkSpeed : 3.2f;
            var gravity = settings != null ? settings.Gravity : -19.62f;

            var planar = transform.right * move.x + transform.forward * move.y;
            if (planar.sqrMagnitude > 1f)
            {
                planar.Normalize();
            }

            planar *= speed;
            PlanarSpeed = new Vector2(planar.x, planar.z).magnitude;

            if (_controller.isGrounded)
            {
                // A small downward bias keeps isGrounded stable on slopes and seams.
                _verticalVelocity = -1f;
            }
            else
            {
                _verticalVelocity += gravity * Time.deltaTime;
            }

            var motion = planar + Vector3.up * _verticalVelocity;
            _controller.Move(motion * Time.deltaTime);
        }

        /// <summary>
        /// Repositions the player, used by the door after a scene swap. The controller
        /// has to be disabled around the write or it overrides the new position.
        /// </summary>
        public void Teleport(Vector3 position, Quaternion rotation)
        {
            var wasEnabled = _controller.enabled;
            _controller.enabled = false;

            transform.SetPositionAndRotation(position, rotation);
            _pitch = 0f;
            if (cameraPivot != null)
            {
                cameraPivot.localRotation = Quaternion.identity;
            }

            _verticalVelocity = 0f;
            _controller.enabled = wasEnabled;
        }
    }
}
