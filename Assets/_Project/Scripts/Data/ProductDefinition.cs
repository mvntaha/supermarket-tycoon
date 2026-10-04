using UnityEngine;

namespace Stockwell.Data
{
    /// <summary>
    /// One sellable product. Everything the game needs to order it, render it, shelve it
    /// and price it lives here, so adding a product never means touching code.
    /// </summary>
    [CreateAssetMenu(menuName = "Stockwell/Product", fileName = "Product")]
    public class ProductDefinition : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Stable id written into the save file. Never change it once shipped; renaming the asset is safe, changing this is not.")]
        [SerializeField] private string id = "product.new";
        [SerializeField] private string displayName = "New Product";
        [SerializeField] private ProductCategory category = ProductCategory.Canned;

        [Header("Economy")]
        [Tooltip("What customers consider a fair price. The pricing curve is centred on this.")]
        [SerializeField] private float marketPrice = 2f;

        [Tooltip("What the player pays per unit when ordering.")]
        [SerializeField] private float wholesaleCost = 1f;

        [Header("Shelving")]
        [Tooltip("How many slot units one unit of this product occupies. A crate of water costs more room than a chocolate bar.")]
        [SerializeField, Min(1)] private int slotSize = 1;

        [Tooltip("Store level at which this becomes orderable. 1 = available from the start.")]
        [SerializeField, Min(1)] private int unlockLevel = 1;

        [Header("Presentation")]
        [SerializeField] private Sprite icon;
        [SerializeField] private GameObject prefab;

        [Header("Pricing curve")]
        [Tooltip("Where purchase chance passes 50%, as a multiple of market price. 1.0 means customers are indifferent at exactly market price.")]
        [SerializeField, Range(0.5f, 2f)] private float midpointRatio = 1.08f;

        [Tooltip("How sharply demand falls off around the midpoint. Higher is more price-sensitive.")]
        [SerializeField, Range(1f, 30f)] private float steepness = 9f;

        public string Id => id;
        public string DisplayName => displayName;
        public ProductCategory Category => category;
        public float MarketPrice => marketPrice;
        public float WholesaleCost => wholesaleCost;
        public int SlotSize => slotSize;
        public int UnlockLevel => unlockLevel;
        public Sprite Icon => icon;
        public GameObject Prefab => prefab;
        public float MidpointRatio => midpointRatio;
        public float Steepness => steepness;

        /// <summary>Editor/test helper. Not used at runtime.</summary>
        public void Configure(string newId, string newName, ProductCategory cat, float market, float wholesale,
                              int slot = 1, int unlock = 1, float midpoint = 1.08f, float steep = 9f)
        {
            id = newId;
            displayName = newName;
            category = cat;
            marketPrice = market;
            wholesaleCost = wholesale;
            slotSize = slot;
            unlockLevel = unlock;
            midpointRatio = midpoint;
            steepness = steep;
        }
    }
}
