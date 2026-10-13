using System;

namespace Stockwell.Economy
{
    /// <summary>The player's money. Nothing else is allowed to mutate cash directly.</summary>
    public interface IWalletService
    {
        float Cash { get; }

        /// <summary>Fired whenever cash changes, with the new balance.</summary>
        event Action<float> CashChanged;

        /// <summary>True if the player can afford <paramref name="amount"/> right now.</summary>
        bool CanAfford(float amount);

        /// <summary>Spends if affordable and returns true; leaves cash untouched and returns false otherwise.</summary>
        bool TrySpend(float amount);

        void Add(float amount);
    }
}
