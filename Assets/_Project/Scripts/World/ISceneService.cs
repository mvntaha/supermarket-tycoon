using System;

namespace Stockwell.World
{
    /// <summary>
    /// Owns additive scene loading. City and Store are separate scenes and only one
    /// is resident at a time, so the NPC cap and draw-call budget are never paid twice.
    /// </summary>
    public interface ISceneService
    {
        /// <summary>Name of the gameplay scene currently resident, or null during a transition.</summary>
        string CurrentScene { get; }

        bool IsTransitioning { get; }

        /// <summary>
        /// Loads <paramref name="sceneName"/> additively and unloads the previous gameplay
        /// scene. Raises <see cref="TransitionComplete"/> with the new scene name when done.
        /// </summary>
        void SwitchTo(string sceneName, string spawnPointId = null);

        /// <summary>Fired after the new scene is loaded and the old one unloaded. Argument is the new scene name.</summary>
        event Action<string> TransitionComplete;

        /// <summary>
        /// The spawn point id requested by the last <see cref="SwitchTo"/>, so the
        /// arriving scene can place the player at the matching door.
        /// </summary>
        string PendingSpawnPointId { get; }
    }
}
