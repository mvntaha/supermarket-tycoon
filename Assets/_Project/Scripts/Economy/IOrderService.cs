using System;
using System.Collections.Generic;
using Stockwell.Data;

namespace Stockwell.Economy
{
    /// <summary>One line of the order cart.</summary>
    [Serializable]
    public struct OrderLine
    {
        public ProductDefinition Product;
        public int Quantity;

        public float Cost => Product != null ? Product.WholesaleCost * Quantity : 0f;
    }

    /// <summary>Why a Confirm() was refused, so the UI can say something useful.</summary>
    public enum OrderResult
    {
        Confirmed,
        EmptyCart,
        InsufficientFunds,
        ContainsLockedProduct
    }

    /// <summary>
    /// The ordering cart. Deliberately does not know how deliveries arrive - it raises
    /// OrderConfirmed and something in the scene turns that into boxes, so the cart stays
    /// testable without a scene.
    /// </summary>
    public interface IOrderService
    {
        IReadOnlyList<OrderLine> Lines { get; }
        float CartTotal { get; }
        bool IsEmpty { get; }

        void SetQuantity(ProductDefinition product, int quantity);
        void Add(ProductDefinition product, int delta);
        void Clear();

        int QuantityOf(ProductDefinition product);

        /// <summary>Checks affordability and unlocks without mutating anything.</summary>
        OrderResult Validate();

        /// <summary>Spends the cash and raises OrderConfirmed with the lines; clears the cart on success.</summary>
        OrderResult Confirm();

        event Action<IReadOnlyList<OrderLine>> OrderConfirmed;
        event Action CartChanged;
    }
}
