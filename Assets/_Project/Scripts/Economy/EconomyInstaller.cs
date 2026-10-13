using System;
using Stockwell.Core;
using UnityEngine;

namespace Stockwell.Economy
{
    /// <summary>
    /// Registers the Phase 2 store services. Kept out of Bootstrap so the foundation
    /// layer does not have to know what a wallet or an order is.
    /// </summary>
    public class EconomyInstaller : MonoBehaviour, IServiceInstaller
    {
        [SerializeField] private StoreSettings settings;

        private WalletService _wallet;
        private InventoryService _inventory;
        private OrderService _orders;

        public void Install()
        {
            if (settings == null)
            {
                Debug.LogError("[EconomyInstaller] No StoreSettings assigned; using fallback values.");
            }

            var startingCash = settings != null ? settings.StartingCash : 250f;

            _wallet = new WalletService(startingCash);
            _inventory = new InventoryService();
            _orders = new OrderService(_wallet, () => settings != null ? settings.StoreLevel : 1);

            GameServices.Register<IWalletService>(_wallet);
            GameServices.Register<IInventoryService>(_inventory);
            GameServices.Register<IOrderService>(_orders);

            if (settings != null)
            {
                GameServices.Register(settings);
            }
        }
    }
}
