using Stockwell.Core;
using Stockwell.Player;
using UnityEngine;

namespace Stockwell.World
{
    /// <summary>
    /// Lives on the persistent Bootstrap object alongside the player. When a scene
    /// transition finishes it finds the matching <see cref="SpawnPoint"/> in the new
    /// scene and moves the player there.
    /// </summary>
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private PlayerController player;

        private ISceneService _scenes;

        private void Start()
        {
            _scenes = GameServices.Get<ISceneService>();
            if (_scenes != null)
            {
                _scenes.TransitionComplete += OnTransitionComplete;
            }
        }

        private void OnDestroy()
        {
            if (_scenes != null)
            {
                _scenes.TransitionComplete -= OnTransitionComplete;
            }
        }

        private void OnTransitionComplete(string sceneName)
        {
            if (player == null)
            {
                Debug.LogError("[PlayerSpawner] No PlayerController assigned.");
                return;
            }

            var wanted = _scenes.PendingSpawnPointId;
            var points = FindObjectsByType<SpawnPoint>(FindObjectsSortMode.None);

            if (points.Length == 0)
            {
                Debug.LogWarning($"[PlayerSpawner] '{sceneName}' has no SpawnPoint; leaving the player where it is.");
                return;
            }

            SpawnPoint target = null;

            if (!string.IsNullOrEmpty(wanted))
            {
                foreach (var p in points)
                {
                    if (p.Id == wanted)
                    {
                        target = p;
                        break;
                    }
                }

                if (target == null)
                {
                    Debug.LogWarning($"[PlayerSpawner] No SpawnPoint '{wanted}' in '{sceneName}'; using the first one found.");
                }
            }

            target ??= points[0];
            player.Teleport(target.transform.position, target.transform.rotation);
        }
    }
}
