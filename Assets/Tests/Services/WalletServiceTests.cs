using System;
using System.Collections.Generic;
using System.Threading;
using Game.Services.Save;
using Game.Services.Wallet;
using NUnit.Framework;

namespace Game.Services.Tests
{
    public sealed class WalletServiceTests
    {
        private InMemorySaveService save;
        private WalletService wallet;

        [SetUp]
        public void SetUp()
        {
            save = new InMemorySaveService();
            wallet = StartWallet(save);
        }

        [Test]
        public void FirstLaunchStartsWithNoCash() => Assert.AreEqual(0, wallet.Cash);

        [Test]
        public void AddIncreasesCash()
        {
            wallet.Add(10);
            wallet.Add(5);
            Assert.AreEqual(15, wallet.Cash);
        }

        [Test]
        public void CashSurvivesARestart()
        {
            wallet.Add(25);

            var afterRestart = StartWallet(save);

            Assert.AreEqual(25, afterRestart.Cash);
        }

        [Test]
        public void TrySpendRemovesCashWhenThereIsEnough()
        {
            wallet.Add(30);
            Assert.IsTrue(wallet.TrySpend(20));
            Assert.AreEqual(10, wallet.Cash);
        }

        [Test]
        public void TrySpendChangesNothingWhenThereIsNotEnough()
        {
            wallet.Add(5);
            Assert.IsFalse(wallet.TrySpend(10));
            Assert.AreEqual(5, wallet.Cash);
        }

        [Test]
        public void CashChangedReportsTheNewBalance()
        {
            var reported = new List<int>();
            wallet.CashChanged += reported.Add;

            wallet.Add(10);
            wallet.TrySpend(4);

            CollectionAssert.AreEqual(new[] { 10, 6 }, reported);
        }

        [Test]
        public void AmountsMustBePositive()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.Add(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => wallet.TrySpend(-1));
        }

        [Test]
        public void AddStopsAtTheMaximumInsteadOfOverflowing()
        {
            wallet.Add(int.MaxValue);
            wallet.Add(1);
            Assert.AreEqual(int.MaxValue, wallet.Cash);
        }

        private static WalletService StartWallet(ISaveService save)
        {
            var started = new WalletService(save);
            started.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            return started;
        }

        // Keeps values in memory, so these tests never touch the real PlayerPrefs.
        private sealed class InMemorySaveService : ISaveService
        {
            private readonly Dictionary<string, object> values = new Dictionary<string, object>();

            public bool HasKey(string key) => values.ContainsKey(key);
            public T Load<T>(string key, T fallback = default) => values.TryGetValue(key, out var value) ? (T)value : fallback;
            public void Save<T>(string key, T value) => values[key] = value;
            public void Delete(string key) => values.Remove(key);
        }
    }
}
