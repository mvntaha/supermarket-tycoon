using UnityEngine;

namespace Stockwell.Core
{
    /// <summary>
    /// Tunable values that would otherwise be magic numbers scattered through the
    /// player and input code. One asset, edited without recompiling.
    /// </summary>
    [CreateAssetMenu(menuName = "Stockwell/Game Settings", fileName = "GameSettings")]
    public class GameSettings : ScriptableObject
    {
        [Header("Look")]
        [Tooltip("Degrees of rotation per unit of mouse delta.")]
        [SerializeField] private float mouseLookSensitivity = 0.12f;

        [Tooltip("Degrees of rotation per unit of touch drag delta. Touch deltas are larger than mouse deltas, so this is lower.")]
        [SerializeField] private float touchLookSensitivity = 0.08f;

        [Tooltip("How far the camera may look up and down, in degrees.")]
        [SerializeField] private float pitchClamp = 85f;

        [SerializeField] private bool invertLookY;

        [Header("Movement")]
        [SerializeField] private float walkSpeed = 3.2f;
        [SerializeField] private float gravity = -19.62f;

        [Header("Interaction")]
        [Tooltip("Raycast range from the camera centre, in metres.")]
        [SerializeField] private float interactRange = 2.5f;

        [Header("Footsteps")]
        [Tooltip("Metres of ground travel between footstep sounds.")]
        [SerializeField] private float footstepDistance = 2.1f;
        [SerializeField, Range(0f, 1f)] private float footstepVolume = 0.5f;

        [Header("Touch HUD")]
        [Tooltip("Minimum physical size of a touch target, in dp. 48 is the Android accessibility floor.")]
        [SerializeField] private float minTouchTargetDp = 48f;

        public float MouseLookSensitivity => mouseLookSensitivity;
        public float TouchLookSensitivity => touchLookSensitivity;
        public float PitchClamp => pitchClamp;
        public bool InvertLookY => invertLookY;
        public float WalkSpeed => walkSpeed;
        public float Gravity => gravity;
        public float InteractRange => interactRange;
        public float FootstepDistance => footstepDistance;
        public float FootstepVolume => footstepVolume;
        public float MinTouchTargetDp => minTouchTargetDp;
    }
}
