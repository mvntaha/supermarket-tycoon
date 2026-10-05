using Stockwell.Core;
using Stockwell.Player;
using UnityEngine;

namespace Stockwell.World
{
    /// <summary>
    /// The only real interactable in Phase 1. Swaps between City and Store and places
    /// the player at the named spawn point in the arriving scene.
    /// </summary>
    public class DoorInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private string targetScene = "Store";

        [Tooltip("Id of the SpawnPoint in the target scene to arrive at.")]
        [SerializeField] private string targetSpawnPointId = "StoreEntrance";

        [SerializeField] private string prompt = "Enter store";

        private ISceneService _scenes;

        public string Prompt => prompt;

        // Refuse while a transition is running, so a double tap cannot queue two loads.
        public bool CanInteract => _scenes == null || !_scenes.IsTransitioning;

        private void Start()
        {
            _scenes = GameServices.Get<ISceneService>();
        }

        public void Interact(GameObject interactor)
        {
            if (_scenes == null)
            {
                Debug.LogError("[DoorInteractable] No ISceneService registered.");
                return;
            }

            if (_scenes.IsTransitioning)
            {
                return;
            }

            _scenes.SwitchTo(targetScene, targetSpawnPointId);
        }
    }
}
