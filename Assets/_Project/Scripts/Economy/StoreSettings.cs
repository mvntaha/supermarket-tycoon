using UnityEngine;

namespace Stockwell.Economy
{
    /// <summary>
    /// Economy tuning that would otherwise be magic numbers. Separate from GameSettings
    /// (movement and input) so the two can be balanced independently.
    /// </summary>
    [CreateAssetMenu(menuName = "Stockwell/Store Settings", fileName = "StoreSettings")]
    public class StoreSettings : ScriptableObject
    {
        [Header("Cash")]
        [SerializeField] private float startingCash = 250f;

        [Header("Delivery")]
        [Tooltip("Seconds between confirming an order and the boxes appearing.")]
        [SerializeField] private float deliveryDelaySeconds = 6f;

        [Tooltip("How many slot-units one delivery box holds. Larger orders split across several boxes.")]
        [SerializeField, Min(1)] private int boxCapacity = 24;

        [Header("Progression")]
        [Tooltip("Store level, which gates which products are orderable. Phase 4 raises this.")]
        [SerializeField, Min(1)] private int storeLevel = 1;

        public float StartingCash => startingCash;
        public float DeliveryDelaySeconds => deliveryDelaySeconds;
        public int BoxCapacity => boxCapacity;
        public int StoreLevel => storeLevel;
    }
}
