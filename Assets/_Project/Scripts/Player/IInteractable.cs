using UnityEngine;

namespace Stockwell.Player
{
    /// <summary>
    /// Anything the centre-screen raycast can act on. Shelves, boxes, the ordering
    /// terminal and the checkout all arrive later through this one interface — the
    /// architecture rule is one interaction system, not one per feature.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>Short verb phrase shown in the HUD prompt, e.g. "Enter store".</summary>
        string Prompt { get; }

        /// <summary>False when the object is present but cannot be used right now.</summary>
        bool CanInteract { get; }

        void Interact(GameObject interactor);
    }
}
