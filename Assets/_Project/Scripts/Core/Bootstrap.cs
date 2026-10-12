using System;
using Stockwell.Input;
using Stockwell.World;
using UnityEngine;

namespace Stockwell.Core
{
    /// <summary>
    /// Scene 0. Registers every service in one readable place, then hands off to the
    /// first gameplay scene. This object survives every scene transition.
    /// </summary>
    public class Bootstrap : MonoBehaviour
    {
        [Header("Services")]
        [SerializeField] private InputService inputService;
        [SerializeField] private SceneService sceneService;

        [Header("Installers")]
        [Tooltip("Components registering further services. Called in array order after the registry is cleared, so startup order stays explicit rather than depending on Unity's component execution order.")]
        [SerializeField] private MonoBehaviour[] installers = Array.Empty<MonoBehaviour>();

        [Header("Config")]
        [SerializeField] private GameSettings settings;
        [SerializeField] private string firstScene = "City";

        [Tooltip("Frame rate cap used for measurement. The 30 fps target is a Phase 6 decision.")]
        [SerializeField] private int targetFrameRate = 60;

        private void Awake()
        {
            // Statics outlive play-mode exit in the editor; start from a clean registry.
            GameServices.Clear();

            Application.targetFrameRate = targetFrameRate;
            QualitySettings.vSyncCount = 0;

            if (settings != null)
            {
                GameServices.Register(settings);
            }
            else
            {
                Debug.LogError("[Bootstrap] No GameSettings assigned.");
            }

            if (inputService != null)
            {
                GameServices.Register<IInputService>(inputService);
            }
            else
            {
                Debug.LogError("[Bootstrap] No InputService assigned.");
            }

            if (sceneService != null)
            {
                GameServices.Register<ISceneService>(sceneService);
            }
            else
            {
                Debug.LogError("[Bootstrap] No SceneService assigned.");
            }

            InstallExtraServices();

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Runs after the core services are in place, so an installer may depend on them.
        /// </summary>
        private void InstallExtraServices()
        {
            if (installers == null)
            {
                return;
            }

            foreach (var m in installers)
            {
                if (m == null)
                {
                    continue;
                }

                if (m is IServiceInstaller installer)
                {
                    installer.Install();
                }
                else
                {
                    Debug.LogError($"[Bootstrap] {m.GetType().Name} is listed as an installer but does not implement IServiceInstaller.");
                }
            }
        }

        private void Start()
        {
            if (sceneService != null && !string.IsNullOrEmpty(firstScene))
            {
                sceneService.SwitchTo(firstScene);
            }
        }
    }
}
