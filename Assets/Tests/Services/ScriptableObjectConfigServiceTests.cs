using System;
using System.Threading;
using Game.Services.Config;
using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Services.Tests
{
    public sealed class ScriptableObjectConfigServiceTests
    {
        private EconomyConfig economy;

        [TearDown]
        public void TearDown()
        {
            // Domain reload is off, so destroy what a test created.
            if (economy != null) Object.DestroyImmediate(economy);
        }

        [Test]
        public void GivesScreensTheAssetFromBootstrap()
        {
            economy = EconomyConfig.Create(new BattleRewards(150, 30), new PriceCurve(100, 1.5f, 10));
            var config = new ScriptableObjectConfigService(economy);

            config.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();

            Assert.AreSame(economy, config.Economy);
            Assert.AreEqual(150, config.Economy.BattleRewards.RewardFor(BattleOutcome.Win));
            Assert.AreEqual(150, config.Economy.UpgradeCost(1));
        }

        [Test]
        public void AMissingAssetIsReportedAtStartup() =>
            Assert.Throws<ArgumentNullException>(() => new ScriptableObjectConfigService(null));

        [Test]
        public void AWrongNumberDoesNotStopTheGame()
        {
            economy = ScriptableObject.CreateInstance<EconomyConfig>(); // a new asset nobody filled in
            var config = new ScriptableObjectConfigService(economy);

            // In the game this logs a red error on purpose; keep it out of the Console while testing.
            Debug.unityLogger.logEnabled = false;
            try
            {
                Assert.DoesNotThrow(() => config.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult());
            }
            finally
            {
                Debug.unityLogger.logEnabled = true;
            }

            Assert.IsNotNull(economy.FindProblem());
            Assert.AreSame(economy, config.Economy);
        }
    }
}
