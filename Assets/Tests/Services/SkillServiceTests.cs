using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.Services.Config;
using Game.Services.Save;
using Game.Services.Skills;
using Game.Services.Wallet;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Game.Services.Tests
{
    public sealed class SkillServiceTests
    {
        private readonly SkillContext sword = new SkillContext("swordsman", "card.1");
        private SnapshotSave save;
        private WalletService wallet;
        private SkillService service;
        private IConfigService config;

        [SetUp]
        public void SetUp()
        {
            save = new SnapshotSave();
            save.Save("wallet.cash", 500);
            var root = AssetDatabase.LoadAssetAtPath<GameConfig>("Assets/Config/GameConfig.asset");
            config = new ScriptableObjectConfigService(root);
            Restart();
        }

        private void Restart()
        {
            wallet = new WalletService(save);
            wallet.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            service = new SkillService(config, wallet);
            service.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        [Test]
        public void BuyingAQuotedLevelOnceSurvivesSerializationAndIsolatesOwnersAndTrees()
        {
            Assert.IsNull(service.Purchase(sword, "damage", 1));
            Assert.IsNotNull(service.Purchase(sword, "damage", 1));
            Restart();
            Assert.AreEqual(450, wallet.Cash);
            Assert.AreEqual(1, service.Inspect(sword, "damage").Current);
            Assert.AreEqual(0, service.Inspect(new SkillContext("archer", "card.1"), "damage").Current);
            Assert.AreEqual(0, service.Inspect(new SkillContext("swordsman", "card.2"), "damage").Current);
            Assert.AreEqual(80, service.Inspect(sword, "damage").Price);
            Assert.IsNotNull(service.Purchase(sword, "damage", 2));
            Assert.AreEqual(450, wallet.Cash);
        }

        [Test]
        public void EveryPrerequisiteAndSequentialLevelIsRequiredAndEffectsAccumulate()
        {
            Assert.IsNotNull(service.Purchase(sword, "health", 2));
            Assert.IsNull(service.Purchase(sword, "health", 1));
            Assert.IsNull(service.Purchase(sword, "health", 2));
            Assert.IsNotNull(service.Purchase(sword, "health", 3));
            Assert.IsNotNull(service.Purchase(sword, "veteran", 1));
            Assert.IsNull(service.Purchase(sword, "damage", 1));
            Assert.IsNull(service.Purchase(sword, "veteran", 1));
            var effects = service.GrantedEffects(sword).ToList();
            Assert.AreEqual(25, effects.Select(e => e.effect).OfType<StatModifierEffect>()
                .Where(e => e.Stat == SkillStat.UnitHealth).Sum(e => e.Amount));
            Assert.AreEqual(2, effects.Count(e => e.skillId == "veteran" && e.level == 1));
        }

        [Test]
        public void FailedWriteCanBeRetriedWithoutLosingCashOrGrantingTwice()
        {
            save.Fail = true;
            Assert.IsNotNull(service.Purchase(sword, "health", 1));
            Assert.AreEqual(500, wallet.Cash);
            Assert.AreEqual(0, service.Inspect(sword, "health").Current);
            save.Fail = false;
            Restart();
            Assert.IsNull(service.Purchase(sword, "health", 1));
            Assert.IsNotNull(service.Purchase(sword, "health", 1));
            Assert.AreEqual(460, wallet.Cash);
        }

        [Test]
        public void InsufficientFundsAndInvalidIdentitiesLeaveBalanceAlone()
        {
            wallet.TrySpend(499);
            Assert.AreEqual("Insufficient Cash", service.Purchase(sword, "damage", 1));
            Assert.IsNotNull(service.Purchase(new SkillContext("swordsman", "a/b"), "health", 1));
            Assert.IsNotNull(service.Purchase(default, null, 1));
            Assert.IsNotNull(service.Purchase(sword, "removed", 1));
            Assert.AreEqual(1, wallet.Cash);
        }

        [Test]
        public void GlobalDrillsUseSharedCurveAndShrunkenDefinitionsCapGrantedLevels()
        {
            var player = new SkillContext("global", "player");
            Assert.AreEqual(100, service.Inspect(player, "drill").Price);
            Assert.IsNull(service.Purchase(player, "drill", 1));
            Assert.AreEqual(150, service.Inspect(player, "drill").Price);
            // Simulate receipts from an older version with a longer tree.
            wallet.TryPurchase("skill/global/player/drill", 2, 1);
            wallet.TryPurchase("skill/global/player/drill", 3, 1);
            Assert.AreEqual(2, service.Inspect(player, "drill").Current);
            Assert.AreEqual(2, service.GrantedEffects(player).Count());
        }

        [TestCase("skills.Array.data[0].id", "damage")]
        [TestCase("skills.Array.data[0].id", "bad/id")]
        [TestCase("skills.Array.data[2].requirements.Array.data[0].skillId", "missing")]
        [TestCase("skills.Array.data[2].requirements.Array.data[0].skillId", "veteran")]
        [TestCase("skills.Array.data[2].requirements.Array.data[0].requiredLevel", "8")]
        [TestCase("skills.Array.data[1].levels.Array.data[1].fixedCash", "-10")]
        public void RejectsInvalidDefinitions(string path, string value)
        {
            var copy = UnityEngine.Object.Instantiate(config.SkillTrees.Single(t => t.Id == "swordsman"));
            try
            {
                var data = new SerializedObject(copy);
                var field = data.FindProperty(path);
                if (field.propertyType == SerializedPropertyType.String) field.stringValue = value;
                else field.intValue = int.Parse(value);
                data.ApplyModifiedPropertiesWithoutUndo();
                Assert.Throws<InvalidOperationException>(() => new SkillCatalog(new[] { copy }, config.Economy.Upgrade));
            }
            finally { UnityEngine.Object.DestroyImmediate(copy); }
        }

        [Test]
        public void DistinguishesReachableCrossLevelDependenciesFromCycles()
        {
            var copy = UnityEngine.Object.Instantiate(config.SkillTrees.Single(t => t.Id == "swordsman"));
            try
            {
                var data = new SerializedObject(copy);
                var requirements = data.FindProperty("skills.Array.data[0].levels.Array.data[1].requirements");
                requirements.arraySize = 1;
                var req = requirements.GetArrayElementAtIndex(0);
                req.FindPropertyRelative("skillId").stringValue = "damage";
                req.FindPropertyRelative("requiredLevel").intValue = 2;
                data.ApplyModifiedPropertiesWithoutUndo();
                Assert.DoesNotThrow(() => new SkillCatalog(new[] { copy }, config.Economy.Upgrade));
                data.FindProperty("skills.Array.data[1].levels.Array.data[1].requirements.Array.data[0].requiredLevel").intValue = 2;
                data.ApplyModifiedPropertiesWithoutUndo();
                StringAssert.Contains("cycle", Assert.Throws<InvalidOperationException>(() =>
                    new SkillCatalog(new[] { copy }, config.Economy.Upgrade)).Message);
            }
            finally { UnityEngine.Object.DestroyImmediate(copy); }
        }

        [Test]
        public void LocalSaveReloadsLegacyCashAndPersistsReceiptAndRemainingBalance()
        {
            var local = new IsolatedPlayerPrefs();
            try
            {
                local.Save("wallet.cash", 100);
                var firstWallet = new WalletService(local);
                firstWallet.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
                var first = new SkillService(config, firstWallet);
                first.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
                Assert.IsNull(first.Purchase(sword, "health", 1));

                var nextWallet = new WalletService(local);
                nextWallet.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
                var next = new SkillService(config, nextWallet);
                next.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
                Assert.AreEqual(60, nextWallet.Cash);
                Assert.AreEqual(1, next.Inspect(sword, "health").Current);
            }
            finally { local.Dispose(); }
        }

        private sealed class IsolatedPlayerPrefs : ISaveService, IDisposable
        {
            private readonly PlayerPrefsSaveService local = new();
            private readonly string prefix = "tests.skills." + Guid.NewGuid().ToString("N") + ".";
            private readonly HashSet<string> written = new();
            public bool HasKey(string key) => local.HasKey(prefix + key);
            public void Delete(string key) => local.Delete(prefix + key);
            public T Load<T>(string key, T fallback = default) => local.Load(prefix + key, fallback);
            public void Save<T>(string key, T value)
            {
                written.Add(key);
                local.Save(prefix + key, value);
            }

            public void Dispose()
            {
                foreach (string key in written) Delete(key);
            }
        }

        private sealed class SnapshotSave : ISaveService
        {
            [Serializable] private sealed class Box<T> { public T value; }
            private readonly Dictionary<string, string> values = new();
            public bool Fail;
            public bool HasKey(string key) => values.ContainsKey(key);
            public void Delete(string key) => values.Remove(key);
            public T Load<T>(string key, T fallback = default) =>
                values.TryGetValue(key, out var json) ? JsonUtility.FromJson<Box<T>>(json).value : fallback;
            public void Save<T>(string key, T value)
            {
                if (Fail) throw new InvalidOperationException("Simulated disk failure");
                values[key] = JsonUtility.ToJson(new Box<T> { value = value });
            }
        }
    }
}
