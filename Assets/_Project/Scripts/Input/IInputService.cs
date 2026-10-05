using System;
using UnityEngine;

namespace Stockwell.Input
{
    /// <summary>
    /// The single input surface the rest of the game reads. Touch and keyboard/mouse
    /// both feed this, so no gameplay code ever asks which device is in use.
    /// </summary>
    public interface IInputService
    {
        /// <summary>Normalised move intent. x = strafe, y = forward.</summary>
        Vector2 Move { get; }

        /// <summary>Look delta for this frame, already scaled by sensitivity. Degrees.</summary>
        Vector2 Look { get; }

        event Action Interact;
        event Action PickUpDrop;
        event Action Place;

        /// <summary>True when the touch HUD should be shown (no mouse present).</summary>
        bool UsingTouch { get; }
    }
}
