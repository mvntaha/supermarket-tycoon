using System;
using System.Collections.Generic;
using Stockwell.Data;
using UnityEngine;

namespace Stockwell.Economy
{
    /// <summary>
    /// Plain C# and keyed by product id rather than by object reference, so Phase 4 can
    /// serialise it directly and reload it against a catalogue lookup.
    /// </summary>
    [Serializable]
    public class InventoryService : IInventoryService
    {
        private readonly Dictionary<string, int> _backroom = new();
        private readonly Dictionary<string, int> _shelf = new();
        private readonly Dictionary<string, float> _price = new();

        public event Action<ProductDefinition> StockChanged;
        public event Action<ProductDefinition> PriceChanged;

        public int BackroomCount(ProductDefinition product) => Get(_backroom, product);

        public int ShelfCount(ProductDefinition product) => Get(_shelf, product);

        public int TotalCount(ProductDefinition product) => BackroomCount(product) + ShelfCount(product);

        public void AddToBackroom(ProductDefinition product, int amount)
        {
            if (product == null || amount <= 0)
            {
                return;
            }

            _backroom[product.Id] = Get(_backroom, product) + amount;
            StockChanged?.Invoke(product);
        }

        public int TakeFromBackroom(ProductDefinition product, int amount)
        {
            if (product == null || amount <= 0)
            {
                return 0;
            }

            var have = Get(_backroom, product);
            var taken = Mathf.Min(have, amount);
            if (taken <= 0)
            {
                return 0;
            }

            _backroom[product.Id] = have - taken;
            StockChanged?.Invoke(product);
            return taken;
        }

        public void ReportShelfDelta(ProductDefinition product, int delta)
        {
            if (product == null || delta == 0)
            {
                return;
            }

            var next = Get(_shelf, product) + delta;
            _shelf[product.Id] = Mathf.Max(0, next);
            StockChanged?.Invoke(product);
        }

        public float GetPrice(ProductDefinition product)
        {
            if (product == null)
            {
                return 0f;
            }

            return _price.TryGetValue(product.Id, out var p) ? p : product.MarketPrice;
        }

        public void SetPrice(ProductDefinition product, float price)
        {
            if (product == null)
            {
                return;
            }

            // Negative prices would invert the purchase curve; clamp rather than trust the caller.
            _price[product.Id] = Mathf.Max(0f, (float)Math.Round(price, 2, MidpointRounding.AwayFromZero));
            PriceChanged?.Invoke(product);
        }

        private static int Get(Dictionary<string, int> map, ProductDefinition p)
        {
            if (p == null)
            {
                return 0;
            }

            return map.TryGetValue(p.Id, out var v) ? v : 0;
        }
    }
}
