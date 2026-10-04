using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Stockwell.World
{
    /// <summary>
    /// Additive scene swapper. Loads the incoming scene before unloading the outgoing
    /// one so the player is never in an empty world, then forces an unload of unused
    /// assets. Lives on the Bootstrap object, which is never unloaded.
    /// </summary>
    public class SceneService : MonoBehaviour, ISceneService
    {
        public string CurrentScene { get; private set; }
        public bool IsTransitioning { get; private set; }
        public string PendingSpawnPointId { get; private set; }

        public event Action<string> TransitionComplete;

        public void SwitchTo(string sceneName, string spawnPointId = null)
        {
            if (IsTransitioning)
            {
                Debug.LogWarning($"[SceneService] Ignoring SwitchTo({sceneName}) — a transition is already running.");
                return;
            }

            if (sceneName == CurrentScene)
            {
                Debug.LogWarning($"[SceneService] {sceneName} is already the current scene.");
                return;
            }

            PendingSpawnPointId = spawnPointId;
            StartCoroutine(SwitchRoutine(sceneName));
        }

        private IEnumerator SwitchRoutine(string sceneName)
        {
            IsTransitioning = true;
            var outgoing = CurrentScene;

            var load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (load == null)
            {
                Debug.LogError($"[SceneService] Could not start loading '{sceneName}'. Is it in the build settings?");
                IsTransitioning = false;
                yield break;
            }

            while (!load.isDone)
            {
                yield return null;
            }

            CurrentScene = sceneName;

            if (!string.IsNullOrEmpty(outgoing))
            {
                var scene = SceneManager.GetSceneByName(outgoing);
                if (scene.IsValid() && scene.isLoaded)
                {
                    var unload = SceneManager.UnloadSceneAsync(scene);
                    while (unload != null && !unload.isDone)
                    {
                        yield return null;
                    }
                }

                // Without this the unloaded scene's textures and meshes stay resident,
                // which is exactly the leak the Phase 1 device test checks for.
                var cleanup = Resources.UnloadUnusedAssets();
                while (!cleanup.isDone)
                {
                    yield return null;
                }
            }

            // Make the new scene active so instantiated objects and lighting settings land there.
            var loaded = SceneManager.GetSceneByName(sceneName);
            if (loaded.IsValid() && loaded.isLoaded)
            {
                SceneManager.SetActiveScene(loaded);
            }

            IsTransitioning = false;
            TransitionComplete?.Invoke(sceneName);
        }
    }
}
