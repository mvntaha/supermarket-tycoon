using System;
using System.Collections.Generic;
using Stockwell.Data;

namespace Stockwell.Economy
{
    /// <summary>
    /// Plain C#. Knows about the wallet and the store level, but nothing about scenes,
    /// boxes or timing - it just announces a confirmed order.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly List<OrderLine> _lines = new();
        private readonly IWalletService _wallet;
        private readonly Func<int> _storeLevel;

        public OrderService(IWalletService wallet, Func<int> storeLevel)
        {
            _wallet = wallet;
            _storeLevel = storeLevel ?? (() => 1);
        }

        public IReadOnlyList<OrderLine> Lines => _lines;

        public float CartTotal
        {
            get
            {
                var total = 0f;
                foreach (var l in _lines)
                {
                    total += l.Cost;
                }

                return (float)Math.Round(total, 2, MidpointRounding.AwayFromZero);
            }
        }

        public bool IsEmpty => _lines.Count == 0;

        public event Action<IReadOnlyList<OrderLine>> OrderConfirmed;
        public event Action CartChanged;

        public int QuantityOf(ProductDefinition product)
        {
            if (product == null)
            {
                return 0;
            }

            for (var i = 0; i < _lines.Count; i++)
            {
                if (_lines[i].Product == product)
                {
                    return _lines[i].Quantity;
                }
            }

            return 0;
        }

        public void SetQuantity(ProductDefinition product, int quantity)
        {
            if (product == null)
            {
                return;
            }

            if (quantity < 0)
            {
                quantity = 0;
            }

            for (var i = 0; i < _lines.Count; i++)
            {
                if (_lines[i].Product != product)
                {
                    continue;
                }

                if (quantity == 0)
                {
                    _lines.RemoveAt(i);
                }
                else
                {
                    _lines[i] = new OrderLine { Product = product, Quantity = quantity };
                }

                CartChanged?.Invoke();
                return;
            }

            if (quantity > 0)
            {
                _lines.Add(new OrderLine { Product = product, Quantity = quantity });
                CartChanged?.Invoke();
            }
        }

        public void Add(ProductDefinition product, int delta) =>
            SetQuantity(product, QuantityOf(product) + delta);

        public void Clear()
        {
            if (_lines.Count == 0)
            {
                return;
            }

            _lines.Clear();
            CartChanged?.Invoke();
        }

        public OrderResult Validate()
        {
            if (_lines.Count == 0)
            {
                return OrderResult.EmptyCart;
            }

            var level = _storeLevel();
            foreach (var l in _lines)
            {
                if (l.Product == null || l.Product.UnlockLevel > level)
                {
                    return OrderResult.ContainsLockedProduct;
                }
            }

            if (_wallet == null || !_wallet.CanAfford(CartTotal))
            {
                return OrderResult.InsufficientFunds;
            }

            return OrderResult.Confirmed;
        }

        public OrderResult Confirm()
        {
            var check = Validate();
            if (check != OrderResult.Confirmed)
            {
                return check;
            }

            // Snapshot before clearing, so listeners get the lines they were promised.
            var snapshot = new List<OrderLine>(_lines);

            if (!_wallet.TrySpend(CartTotal))
            {
                return OrderResult.InsufficientFunds;
            }

            _lines.Clear();
            CartChanged?.Invoke();
            OrderConfirmed?.Invoke(snapshot);
            return OrderResult.Confirmed;
        }
    }
}
