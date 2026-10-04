using Stockwell.Core;
using Stockwell.Player;
using UnityEngine;

namespace Stockwell.Audio
{
    /// <summary>
    /// Distance-based footsteps: accumulate ground travel and fire a clip every N
    /// metres. Tied to distance rather than a timer so the cadence matches the walk
    /// speed instead of drifting out of step with it.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class FootstepPlayer : MonoBehaviour
    {
        [SerializeField] private PlayerController player;
        [SerializeField] private GameSettings settings;
        [SerializeField] private AudioClip[] clips;

        private AudioSource _source;
        private float _travelled;
        private int _lastIndex = -1;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _source.playOnAwake = false;
        }

        private void Start()
        {
            if (settings == null)
            {
                GameServices.TryGet(out settings);
            }
        }

        private void Update()
        {
            if (player == null || clips == null || clips.Length == 0)
            {
                return;
            }

            // Only while actually moving on the ground, which is what the device test checks.
            if (!player.IsGrounded || player.PlanarSpeed < 0.3f)
            {
                _travelled = 0f;
                return;
            }

            _travelled += player.PlanarSpeed * Time.deltaTime;

            var stride = settings != null ? settings.FootstepDistance : 2.1f;
            if (_travelled < stride)
            {
                return;
            }

            _travelled = 0f;
            PlayStep();
        }

        private void PlayStep()
        {
            var index = Random.Range(0, clips.Length);

            // Avoid the same clip twice in a row; it reads as a stutter rather than a step.
            if (clips.Length > 1 && index == _lastIndex)
            {
                index = (index + 1) % clips.Length;
            }

            _lastIndex = index;

            var volume = settings != null ? settings.FootstepVolume : 0.5f;
            _source.pitch = Random.Range(0.94f, 1.06f);
            _source.PlayOneShot(clips[index], volume);
        }
    }
}
