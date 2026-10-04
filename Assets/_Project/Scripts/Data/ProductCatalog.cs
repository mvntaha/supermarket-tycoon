using System.Collections.Generic;
using UnityEngine;

namespace Stockwell.Data
{
    /// <summary>
    /// Every product in the game, in one asset. Lookup by id is built once on first use
    /// rather than scanning the list, because the order UI and the save loader both do it
    /// per entry.
    /// </summary>
    [CreateAssetMenu(menuName = "Stockwell/Product Catalog", fileName = "ProductCatalog")]
    public class ProductCatalog : ScriptableObject
    {
        [SerializeField] private List<ProductDefinition> products = new();

        private Dictionary<string, ProductDefinition> _byId;

        public IReadOnlyList<ProductDefinition> All => products;

        public ProductDefinition ById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            BuildIndex();
            return _byId.TryGetValue(id, out var p) ? p : null;
        }

        /// <summary>Products the player can actually order at this store level.</summary>
        public IEnumerable<ProductDefinition> Unlocked(int storeLevel)
        {
            foreach (var p in products)
            {
                if (p != null && p.UnlockLevel <= storeLevel)
                {
                    yield return p;
                }
            }
        }

        public IEnumerable<ProductDefinition> InCategory(ProductCategory category)
        {
            foreach (var p in products)
            {
                if (p != null && p.Category == category)
                {
                    yield return p;
                }
            }
        }

        private void BuildIndex()
        {
            if (_byId != null)
            {
                return;
            }

            _byId = new Dictionary<string, ProductDefinition>(products.Count);
            foreach (var p in products)
            {
                if (p == null || string.IsNullOrEmpty(p.Id))
                {
                    continue;
                }

                if (_byId.ContainsKey(p.Id))
                {
                    Debug.LogError($"[ProductCatalog] Duplicate product id '{p.Id}'. Ids must be unique; they are written into save files.");
                    continue;
                }

                _byId[p.Id] = p;
            }
        }

        private void OnDisable() => _byId = null;

#if UNITY_EDITOR
        public void EditorSetProducts(List<ProductDefinition> list)
        {
            products = list;
            _byId = null;
        }
#endif
    }
}
