using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Game.Services.Save;

namespace Game.Services.Wallet
{
    public sealed class WalletService : IPurchaseWallet, IInitializableService
    {
        private const string AccountKey = "wallet.account.v1";
        private readonly ISaveService save;
        private Account account = new Account();

        [Serializable]
        private sealed class Account
        {
            public int cash;
            public List<Receipt> receipts = new List<Receipt>();
        }

        [Serializable]
        private struct Receipt
        {
            public string product;
            public int quantity;
        }

        public WalletService(ISaveService save) => this.save = save ?? throw new ArgumentNullException(nameof(save));
        public int Cash => account.cash;
        public event Action<int> CashChanged;

        public Task InitializeAsync(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            account = save.HasKey(AccountKey)
                ? save.Load<Account>(AccountKey) ?? throw new InvalidOperationException("Unreadable wallet account.")
                : new Account { cash = Math.Max(0, save.Load("wallet.cash", 0)) };
            var products = new HashSet<string>();
            if (Cash < 0 || account.receipts == null || account.receipts.Exists(r =>
                string.IsNullOrEmpty(r.product) || r.quantity < 1 || !products.Add(r.product)))
                throw new InvalidOperationException("Invalid wallet balance or purchase history.");
            return Task.CompletedTask;
        }

        public int Purchased(string product) => account.receipts.Find(r => r.product == product).quantity;

        public void Add(int amount)
        {
            RequirePositive(amount);
            Commit((int)Math.Min(int.MaxValue, (long)Cash + amount), account.receipts);
        }

        public bool TrySpend(int amount)
        {
            RequirePositive(amount);
            if (Cash < amount) return false;
            Commit(Cash - amount, account.receipts);
            return true;
        }

        public bool TryPurchase(string product, int targetQuantity, int cashPrice)
        {
            RequirePositive(cashPrice);
            if (string.IsNullOrWhiteSpace(product)) throw new ArgumentException(nameof(product));
            int index = account.receipts.FindIndex(r => r.product == product);
            int previous = index < 0 ? 0 : account.receipts[index].quantity;
            if (targetQuantity <= 0 || targetQuantity - 1 != previous || Cash < cashPrice) return false;
            var receipts = new List<Receipt>(account.receipts);
            var receipt = new Receipt { product = product, quantity = targetQuantity };
            if (index < 0) receipts.Add(receipt);
            else receipts[index] = receipt;
            Commit(Cash - cashPrice, receipts);
            return true;
        }

        private void Commit(int cash, List<Receipt> receipts)
        {
            var next = new Account { cash = cash, receipts = receipts };
            save.Save(AccountKey, next);
            account = next;
            if (CashChanged == null) return;
            foreach (Action<int> observer in CashChanged.GetInvocationList())
            {
                try { observer(Cash); }
                catch (Exception exception) { UnityEngine.Debug.LogException(exception); }
            }
        }

        private static void RequirePositive(int amount)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
        }
    }
}
