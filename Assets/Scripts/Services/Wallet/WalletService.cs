using System;
using System.Threading;
using System.Threading.Tasks;
using Game.Services.Save;

namespace Game.Services.Wallet
{
    /// <summary>
    /// Keeps the player's Cash. Loads it through <see cref="ISaveService"/> when Bootstrap starts it
    /// and saves every change, so Cash survives closing the app.
    /// </summary>
    public sealed class WalletService : IWalletService, IInitializableService
    {
        private const string CashKey = "wallet.cash";

        private readonly ISaveService save;

        public WalletService(ISaveService save) =>
            this.save = save ?? throw new ArgumentNullException(nameof(save));

        public int Cash { get; private set; }

        public event Action<int> CashChanged;

        public Task InitializeAsync(CancellationToken cancellation)
        {
            Cash = Math.Max(0, save.Load(CashKey, 0));
            return Task.CompletedTask;
        }

        public void Add(int amount)
        {
            RequirePositive(amount);
            // Stop at the maximum instead of overflowing into a negative balance.
            SetCash(amount > int.MaxValue - Cash ? int.MaxValue : Cash + amount);
        }

        public bool TrySpend(int amount)
        {
            RequirePositive(amount);
            if (amount > Cash) return false;

            SetCash(Cash - amount);
            return true;
        }

        private void SetCash(int value)
        {
            Cash = value;
            save.Save(CashKey, Cash);
            CashChanged?.Invoke(Cash);
        }

        private static void RequirePositive(int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Cash amounts must be positive.");
        }
    }
}
