using System;
using System.Threading;
using Game.Services.Config;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;

namespace Game.Services.Tests
{
    public sealed class ScriptableObjectConfigServiceTests
    {
        private EconomyConfig economy;
        private GameConfig root;

        [SetUp]
        public void SetUp() => root = ScriptableObject.CreateInstance<GameConfig>();

        private void AssignEconomy()
        {
            var serialized = new SerializedObject(root);
            serialized.FindProperty("economy").objectReferenceValue = economy;
            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        [TearDown]
        public void TearDown()
        {
            // Domain reload is off, so destroy what a test created.
            Object.DestroyImmediate(root);
            if (economy != null) Object.DestroyImmediate(economy);
        }

        [Test]
        public void GivesScreensTheAssetFromBootstrap()
        {
            economy = EconomyConfig.Create(new BattleRewards(150, 30), new PriceCurve(100, 1.5f, 10));
            AssignEconomy();
            var config = new ScriptableObjectConfigService(root);

            config.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreSame(economy, config.Economy);
            Assert.AreEqual(150, config.Economy.BattleRewards.Win);
            Assert.AreEqual(1.5f, config.Economy.Upgrade.Growth);
        }

        [Test]
        public void AMissingAssetIsReportedAtStartup() =>
            Assert.Throws<ArgumentNullException>(() => new ScriptableObjectConfigService(null));

        [Test]
        public void MissingEconomyIsReportedAtStartup() =>
            Assert.Throws<ArgumentException>(() => new ScriptableObjectConfigService(root));

        [TestCase(0, 0, 0, 0f, 0, "Battle rewards")]
        [TestCase(0, 0, 100, 1.5f, 10, "Battle rewards")]
        [TestCase(150, -1, 100, 1.5f, 10, "Battle rewards")]
        [TestCase(30, 150, 100, 1.5f, 10, "Battle rewards")]
        [TestCase(150, 30, 0, 1.5f, 10, "Upgrade")]
        [TestCase(150, 30, 100, 0.9f, 10, "Upgrade")]
        [TestCase(150, 30, 100, float.NaN, 10, "Upgrade")]
        [TestCase(150, 30, 100, 1.5f, 0, "Upgrade")]
        public void InvalidNumbersAreLoggedAndStartupContinues(
            int win, int lose, int baseCost, float growth, int maxLevel, string problem)
        {
            economy = EconomyConfig.Create(new BattleRewards(win, lose), new PriceCurve(baseCost, growth, maxLevel));
            AssignEconomy();
            var config = new ScriptableObjectConfigService(root);

            LogAssert.Expect(LogType.Error, new Regex($"^EconomyConfig: {problem}"));
            config.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreSame(economy, config.Economy);
        }
    }
}
