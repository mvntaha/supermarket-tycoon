using NUnit.Framework;
using Stockwell.Data;
using Stockwell.Economy;
using UnityEngine;

namespace Stockwell.Tests
{
    public class EconomyTests
    {
        private ProductDefinition _milk;
        private ProductDefinition _caviar;   // locked, unlock level 5
        private WalletService _wallet;
        private OrderService _orders;
        private InventoryService _inventory;
        private int _storeLevel;

        [SetUp]
        public void SetUp()
        {
            _milk = ScriptableObject.CreateInstance<ProductDefinition>();
            _milk.Configure("dairy.milk", "Milk", ProductCategory.Dairy, 1.30f, 0.65f, 1, 1);

            _caviar = ScriptableObject.CreateInstance<ProductDefinition>();
            _caviar.Configure("lux.caviar", "Caviar", ProductCategory.Canned, 40f, 20f, 1, 5);

            _storeLevel = 1;
            _wallet = new WalletService(100f);
            _orders = new OrderService(_wallet, () => _storeLevel);
            _inventory = new InventoryService();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_milk);
            Object.DestroyImmediate(_caviar);
        }

        // ---- wallet

        [Test]
        public void Wallet_StartsAtTheConfiguredAmount()
        {
            Assert.AreEqual(100f, _wallet.Cash, 1e-4f);
        }

        [Test]
        public void Wallet_TrySpend_DeductsAndReportsSuccess()
        {
            Assert.IsTrue(_wallet.TrySpend(30f));
            Assert.AreEqual(70f, _wallet.Cash, 1e-4f);
        }

        [Test]
        public void Wallet_TrySpend_RefusesAndLeavesCashUntouched_WhenShort()
        {
            Assert.IsFalse(_wallet.TrySpend(150f));
            Assert.AreEqual(100f, _wallet.Cash, 1e-4f);
        }

        [Test]
        public void Wallet_RaisesChangeEvent_OnlyWhenCashMoves()
        {
            var fired = 0;
            _wallet.CashChanged += _ => fired++;
            _wallet.TrySpend(10f);
            _wallet.TrySpend(9999f);   // refused, must not fire
            _wallet.Add(5f);
            _wallet.Add(-5f);          // ignored, must not fire
            Assert.AreEqual(2, fired);
        }

        // ---- order cost

        [Test]
        public void Order_TotalIsWholesaleTimesQuantity()
        {
            _orders.SetQuantity(_milk, 10);
            Assert.AreEqual(6.50f, _orders.CartTotal, 1e-4f);
        }

        [Test]
        public void Order_Confirm_DeductsExactlyTheCartTotal()
        {
            _orders.SetQuantity(_milk, 10);
            Assert.AreEqual(OrderResult.Confirmed, _orders.Confirm());
            Assert.AreEqual(93.50f, _wallet.Cash, 1e-4f);
        }

        [Test]
        public void Order_Confirm_ClearsTheCartAndAnnouncesTheLines()
        {
            System.Collections.Generic.IReadOnlyList<OrderLine> seen = null;
            _orders.OrderConfirmed += lines => seen = lines;

            _orders.SetQuantity(_milk, 4);
            _orders.Confirm();

            Assert.IsTrue(_orders.IsEmpty, "cart should be empty after confirming");
            Assert.IsNotNull(seen, "OrderConfirmed should carry the lines");
            Assert.AreEqual(1, seen.Count);
            Assert.AreEqual(4, seen[0].Quantity);
        }

        [Test]
        public void Order_Confirm_RefusesWhenShort_AndSpendsNothing()
        {
            _orders.SetQuantity(_milk, 1000);   // 650, wallet has 100
            Assert.AreEqual(OrderResult.InsufficientFunds, _orders.Confirm());
            Assert.AreEqual(100f, _wallet.Cash, 1e-4f);
            Assert.IsFalse(_orders.IsEmpty, "a refused order must keep the cart");
        }

        [Test]
        public void Order_Confirm_RefusesLockedProducts()
        {
            _orders.SetQuantity(_caviar, 1);
            Assert.AreEqual(OrderResult.ContainsLockedProduct, _orders.Confirm());
            Assert.AreEqual(100f, _wallet.Cash, 1e-4f);
        }

        [Test]
        public void Order_LockedProductBecomesOrderable_WhenStoreLevelRises()
        {
            _orders.SetQuantity(_caviar, 1);
            Assert.AreEqual(OrderResult.ContainsLockedProduct, _orders.Validate());
            _storeLevel = 5;
            Assert.AreEqual(OrderResult.Confirmed, _orders.Validate());
        }

        [Test]
        public void Order_EmptyCartIsRefused()
        {
            Assert.AreEqual(OrderResult.EmptyCart, _orders.Confirm());
        }

        [Test]
        public void Order_SettingQuantityToZeroRemovesTheLine()
        {
            _orders.SetQuantity(_milk, 5);
            _orders.SetQuantity(_milk, 0);
            Assert.IsTrue(_orders.IsEmpty);
            Assert.AreEqual(0f, _orders.CartTotal, 1e-4f);
        }

        [Test]
        public void Order_AddAccumulatesAndNeverGoesNegative()
        {
            _orders.Add(_milk, 3);
            _orders.Add(_milk, 2);
            Assert.AreEqual(5, _orders.QuantityOf(_milk));
            _orders.Add(_milk, -99);
            Assert.AreEqual(0, _orders.QuantityOf(_milk));
        }

        // ---- stock accounting

        [Test]
        public void Inventory_BackroomAddAndTake()
        {
            _inventory.AddToBackroom(_milk, 10);
            Assert.AreEqual(10, _inventory.BackroomCount(_milk));

            var taken = _inventory.TakeFromBackroom(_milk, 4);
            Assert.AreEqual(4, taken);
            Assert.AreEqual(6, _inventory.BackroomCount(_milk));
        }

        [Test]
        public void Inventory_TakeIsCappedByWhatIsActuallyThere()
        {
            _inventory.AddToBackroom(_milk, 3);
            Assert.AreEqual(3, _inventory.TakeFromBackroom(_milk, 10));
            Assert.AreEqual(0, _inventory.BackroomCount(_milk));
        }

        [Test]
        public void Inventory_TotalIsBackroomPlusShelf()
        {
            _inventory.AddToBackroom(_milk, 6);
            _inventory.ReportShelfDelta(_milk, 4);
            Assert.AreEqual(6, _inventory.BackroomCount(_milk));
            Assert.AreEqual(4, _inventory.ShelfCount(_milk));
            Assert.AreEqual(10, _inventory.TotalCount(_milk));
        }

        [Test]
        public void Inventory_ShelfCountNeverGoesNegative()
        {
            _inventory.ReportShelfDelta(_milk, 2);
            _inventory.ReportShelfDelta(_milk, -10);
            Assert.AreEqual(0, _inventory.ShelfCount(_milk));
        }

        [Test]
        public void Inventory_PriceDefaultsToMarketThenHoldsWhatIsSet()
        {
            Assert.AreEqual(_milk.MarketPrice, _inventory.GetPrice(_milk), 1e-4f);
            _inventory.SetPrice(_milk, 1.75f);
            Assert.AreEqual(1.75f, _inventory.GetPrice(_milk), 1e-4f);
        }

        [Test]
        public void Inventory_NegativePriceIsClampedToZero()
        {
            _inventory.SetPrice(_milk, -5f);
            Assert.AreEqual(0f, _inventory.GetPrice(_milk), 1e-4f);
        }

        [Test]
        public void Inventory_HandlesNullProductWithoutThrowing()
        {
            Assert.DoesNotThrow(() =>
            {
                _inventory.AddToBackroom(null, 5);
                _inventory.TakeFromBackroom(null, 5);
                _inventory.ReportShelfDelta(null, 5);
                _inventory.SetPrice(null, 1f);
            });
            Assert.AreEqual(0, _inventory.TotalCount(null));
            Assert.AreEqual(0f, _inventory.GetPrice(null), 1e-4f);
        }
    }
}
