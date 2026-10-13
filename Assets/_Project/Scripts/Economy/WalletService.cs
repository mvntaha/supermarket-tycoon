using System;

namespace Stockwell.Economy
{
    /// <summary>
    /// Plain C# so it can be unit-tested without a scene and serialised directly in
    /// Phase 4. Rounds to whole pence on every write, because repeated float arithmetic
    /// on prices otherwise drifts into values the HUD renders as "3.9999999".
    /// </summary>
    [Serializable]
    public class WalletService : IWalletService
    {
        private float _cash;

        public WalletService(float startingCash)
        {
            _cash = Round(Math.Max(0f, startingCash));
        }

        public float Cash => _cash;

        public event Action<float> CashChanged;

        public bool CanAfford(float amount) => amount <= _cash + 0.0001f;

        public bool TrySpend(float amount)
        {
            if (amount < 0f || !CanAfford(amount))
            {
                return false;
            }

            _cash = Round(_cash - amount);
            CashChanged?.Invoke(_cash);
            return true;
        }

        public void Add(float amount)
        {
            if (amount <= 0f)
            {
                return;
            }

            _cash = Round(_cash + amount);
            CashChanged?.Invoke(_cash);
        }

        private static float Round(float v) => (float)Math.Round(v, 2, MidpointRounding.AwayFromZero);
    }
}
