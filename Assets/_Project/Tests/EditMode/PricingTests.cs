using NUnit.Framework;
using Stockwell.Data;
using UnityEngine;

namespace Stockwell.Tests
{
    /// <summary>
    /// The purchase-chance curve is the only piece of Phase 2 economy the player feels
    /// directly, and Phase 3's customer AI will depend on it agreeing with the pricing UI.
    /// These lock its shape.
    /// </summary>
    public class PricingTests
    {
        private const float Market = 2.50f;
        private const float Midpoint = 1.08f;
        private const float Steepness = 9f;

        private static float Chance(float price) =>
            Pricing.PurchaseChance(price, Market, Midpoint, Steepness);

        [Test]
        public void Chance_IsOne_AtOrBelowSeventyPercentOfMarket()
        {
            Assert.AreEqual(1f, Chance(Market * Pricing.AlwaysBuyRatio), 1e-4f);
            Assert.AreEqual(1f, Chance(Market * 0.5f), 1e-4f);
            Assert.AreEqual(1f, Chance(0.01f), 1e-4f);
        }

        [Test]
        public void Chance_IsZero_AtOrAboveOneFiftyPercentOfMarket()
        {
            Assert.AreEqual(0f, Chance(Market * Pricing.NeverBuyRatio), 1e-4f);
            Assert.AreEqual(0f, Chance(Market * 2f), 1e-4f);
            Assert.AreEqual(0f, Chance(Market * 10f), 1e-4f);
        }

        [Test]
        public void Chance_IsAlwaysWithinZeroToOne()
        {
            for (var price = 0f; price <= Market * 3f; price += Market * 0.01f)
            {
                var c = Chance(price);
                Assert.GreaterOrEqual(c, 0f, $"below zero at price {price}");
                Assert.LessOrEqual(c, 1f, $"above one at price {price}");
            }
        }

        [Test]
        public void Chance_DecreasesMonotonically_AsPriceRises()
        {
            var previous = Chance(0f);
            for (var price = 0f; price <= Market * 2f; price += Market * 0.005f)
            {
                var c = Chance(price);
                Assert.LessOrEqual(c, previous + 1e-5f,
                    $"chance rose from {previous} to {c} as price reached {price}");
                previous = c;
            }
        }

        [Test]
        public void Chance_IsNearHalf_AtTheMidpointPrice()
        {
            var atMidpoint = Chance(Market * Midpoint);
            Assert.That(atMidpoint, Is.InRange(0.40f, 0.60f),
                $"expected roughly half at the midpoint, got {atMidpoint}");
        }

        [Test]
        public void Chance_IsHigherBelowMidpoint_ThanAbove()
        {
            var below = Chance(Market * (Midpoint - 0.15f));
            var above = Chance(Market * (Midpoint + 0.15f));
            Assert.Greater(below, above);
        }

        [Test]
        public void SteeperCurve_PunishesOverpricingHarder()
        {
            var price = Market * 1.25f;
            var gentle = Pricing.PurchaseChance(price, Market, Midpoint, 4f);
            var harsh = Pricing.PurchaseChance(price, Market, Midpoint, 20f);
            Assert.Greater(gentle, harsh);
        }

        [Test]
        public void Chance_IsZero_ForNullProductOrNonsenseMarketPrice()
        {
            Assert.AreEqual(0f, Pricing.PurchaseChance((ProductDefinition)null, 1f));
            Assert.AreEqual(0f, Pricing.PurchaseChance(1f, 0f, Midpoint, Steepness));
            Assert.AreEqual(0f, Pricing.PurchaseChance(1f, -5f, Midpoint, Steepness));
        }

        [Test]
        public void ProductOverload_MatchesParameterOverload()
        {
            var p = ScriptableObject.CreateInstance<ProductDefinition>();
            p.Configure("test.milk", "Milk", ProductCategory.Dairy, Market, 1.2f, 1, 1, Midpoint, Steepness);

            foreach (var price in new[] { 0.5f, 1.5f, 2.5f, 3.0f, 3.75f, 5f })
            {
                Assert.AreEqual(Chance(price), Pricing.PurchaseChance(p, price), 1e-5f, $"at price {price}");
            }

            Object.DestroyImmediate(p);
        }

        [Test]
        public void BreakEvenPrice_IsMarketTimesMidpoint()
        {
            var p = ScriptableObject.CreateInstance<ProductDefinition>();
            p.Configure("test.beans", "Beans", ProductCategory.Canned, 4f, 2f, 1, 1, 1.25f, Steepness);
            Assert.AreEqual(5f, Pricing.BreakEvenPrice(p), 1e-4f);
            Object.DestroyImmediate(p);
        }
    }
}
