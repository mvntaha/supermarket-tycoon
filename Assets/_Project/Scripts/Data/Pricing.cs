using UnityEngine;

namespace Stockwell.Data
{
    /// <summary>
    /// How the player's price affects whether a customer buys. Pure and static so it can
    /// be unit-tested without a scene, and so Phase 3's customer AI and the Phase 2
    /// pricing UI are guaranteed to agree.
    /// </summary>
    public static class Pricing
    {
        /// <summary>Below this fraction of market price, everyone buys.</summary>
        public const float AlwaysBuyRatio = 0.70f;

        /// <summary>Above this fraction of market price, effectively nobody buys.</summary>
        public const float NeverBuyRatio = 1.50f;

        /// <summary>
        /// Chance in 0..1 that a customer accepts <paramref name="playerPrice"/>.
        /// Sigmoid centred on the product's midpoint ratio, then clamped so the curve
        /// actually reaches 1 and 0 at the ends rather than merely approaching them.
        /// </summary>
        public static float PurchaseChance(ProductDefinition product, float playerPrice)
        {
            if (product == null)
            {
                return 0f;
            }

            return PurchaseChance(playerPrice, product.MarketPrice, product.MidpointRatio, product.Steepness);
        }

        /// <summary>Parameter-level overload, used by the tests and by the pricing UI preview.</summary>
        public static float PurchaseChance(float playerPrice, float marketPrice, float midpointRatio, float steepness)
        {
            if (marketPrice <= 0f)
            {
                return 0f;
            }

            // Free (or nonsensical negative) is always taken.
            if (playerPrice <= 0f)
            {
                return 1f;
            }

            var ratio = playerPrice / marketPrice;

            if (ratio <= AlwaysBuyRatio)
            {
                return 1f;
            }

            if (ratio >= NeverBuyRatio)
            {
                return 0f;
            }

            // Logistic falling from 1 to 0 as ratio rises past the midpoint.
            var raw = 1f / (1f + Mathf.Exp(steepness * (ratio - midpointRatio)));

            // Rescale so the curve hits exactly 1 at AlwaysBuyRatio and exactly 0 at
            // NeverBuyRatio. Without this there is a visible step at both ends.
            var atAlways = 1f / (1f + Mathf.Exp(steepness * (AlwaysBuyRatio - midpointRatio)));
            var atNever = 1f / (1f + Mathf.Exp(steepness * (NeverBuyRatio - midpointRatio)));
            var span = atAlways - atNever;

            if (span <= Mathf.Epsilon)
            {
                return raw >= 0.5f ? 1f : 0f;
            }

            return Mathf.Clamp01((raw - atNever) / span);
        }

        /// <summary>
        /// The price at which purchase chance is 50%. Used by the pricing panel to show
        /// the player where the curve turns without making them hunt for it.
        /// </summary>
        public static float BreakEvenPrice(ProductDefinition product)
        {
            return product == null ? 0f : product.MarketPrice * product.MidpointRatio;
        }
    }
}
