using System;
using Stockwell.Data;

namespace Stockwell.Economy
{
    /// <summary>
    /// Where stock is and what it sells for. Backroom stock is what has been delivered
    /// but not yet shelved; shelf stock is tracked by the slots themselves and reported
    /// here so the HUD and the order UI can see totals without walking the scene.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>Units sitting in delivered boxes, not yet on a shelf.</summary>
        int BackroomCount(ProductDefinition product);

        /// <summary>Units currently on shelves.</summary>
        int ShelfCount(ProductDefinition product);

        int TotalCount(ProductDefinition product);

        void AddToBackroom(ProductDefinition product, int amount);

        /// <summary>Removes up to <paramref name="amount"/> from the backroom; returns how many were actually taken.</summary>
        int TakeFromBackroom(ProductDefinition product, int amount);

        /// <summary>Called by shelf slots when their contents change.</summary>
        void ReportShelfDelta(ProductDefinition product, int delta);

        /// <summary>The player's selling price. Defaults to the product's market price.</summary>
        float GetPrice(ProductDefinition product);

        void SetPrice(ProductDefinition product, float price);

        event Action<ProductDefinition> StockChanged;
        event Action<ProductDefinition> PriceChanged;
    }
}
