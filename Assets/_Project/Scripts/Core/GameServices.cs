using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stockwell.Core
{
    /// <summary>
    /// The one service registry allowed by the architecture rules. Systems ask for
    /// an interface; nothing else is a singleton. No reflection, no auto-discovery:
    /// <see cref="Bootstrap"/> registers everything explicitly so the startup order
    /// is readable in one place.
    /// </summary>
    public static class GameServices
    {
        private static readonly Dictionary<Type, object> Services = new();

        /// <summary>Registers <paramref name="service"/> as the implementation of <typeparamref name="T"/>.</summary>
        public static void Register<T>(T service) where T : class
        {
            if (service == null)
            {
                Debug.LogError($"[GameServices] Refused to register a null {typeof(T).Name}.");
                return;
            }

            if (Services.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"[GameServices] {typeof(T).Name} was already registered; replacing it.");
            }

            Services[typeof(T)] = service;
        }

        /// <summary>
        /// Returns the registered <typeparamref name="T"/>, or null with an error if the
        /// bootstrap never registered it. Failing loudly here beats a NullReferenceException
        /// three systems away.
        /// </summary>
        public static T Get<T>() where T : class
        {
            if (Services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            Debug.LogError($"[GameServices] {typeof(T).Name} was never registered. Is the Bootstrap scene loaded first?");
            return null;
        }

        public static bool TryGet<T>(out T service) where T : class
        {
            if (Services.TryGetValue(typeof(T), out var found))
            {
                service = (T)found;
                return true;
            }

            service = null;
            return false;
        }

        public static void Unregister<T>() where T : class => Services.Remove(typeof(T));

        /// <summary>
        /// Clears the registry. Statics survive play-mode exit in the editor, so the
        /// bootstrap clears before registering to avoid stale references from a previous run.
        /// </summary>
        public static void Clear() => Services.Clear();
    }
}
