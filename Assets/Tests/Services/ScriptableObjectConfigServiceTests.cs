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
            Assert.AreEqual(150, config.Economy.BattleRewards.RewardFor(BattleOutcome.Win));
            Assert.AreEqual(150, config.Economy.UpgradeCost(1));
        }

        [Test]
        public void AMissingAssetIsReportedAtStartup() =>
            Assert.Throws<ArgumentNullException>(() => new ScriptableObjectConfigService(null));

        [Test]
        public void MissingEconomyIsReportedAtStartup() =>
            Assert.Throws<ArgumentException>(() => new ScriptableObjectConfigService(root));

        [Test]
        public void AWrongNumberDoesNotStopTheGame()
        {
            economy = ScriptableObject.CreateInstance<EconomyConfig>(); // a new asset nobody filled in
            AssignEconomy();
            var config = new ScriptableObjectConfigService(root);

            LogAssert.Expect(LogType.Error, new Regex("^EconomyConfig: Battle rewards"));
            config.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();

            Assert.IsNotNull(economy.FindProblem());
            Assert.AreSame(economy, config.Economy);
        }
    }
}
