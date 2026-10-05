using Stockwell.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Stockwell.UI
{
    /// <summary>
    /// On-screen action button. Fires on pointer-down rather than through Unity's Button
    /// click (which needs down and up on the same object) so it still responds when the
    /// thumb slides slightly during a tap.
    /// </summary>
    public class TouchButton : MonoBehaviour, IPointerDownHandler
    {
        public enum ActionKind
        {
            Interact,
            PickUpDrop,
            Place
        }

        [SerializeField] private InputService input;
        [SerializeField] private ActionKind action = ActionKind.Interact;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (input == null)
            {
                return;
            }

            switch (action)
            {
                case ActionKind.Interact:
                    input.FireInteract();
                    break;
                case ActionKind.PickUpDrop:
                    input.FirePickUpDrop();
                    break;
                case ActionKind.Place:
                    input.FirePlace();
                    break;
            }
        }
    }
}
