using System;
using Game.Services.Config;
using NUnit.Framework;

namespace Game.Services.Tests
{
    public sealed class BattleRewardsTests
    {
        [Test]
        public void EachOutcomePaysItsOwnReward()
        {
            var rewards = new BattleRewards(win: 150, lose: 30);
            Assert.AreEqual(150, rewards.RewardFor(BattleOutcome.Win));
            Assert.AreEqual(30, rewards.RewardFor(BattleOutcome.Lose));
        }

        [Test]
        public void ALossMayPayNothing() =>
            Assert.AreEqual(0, new BattleRewards(win: 150, lose: 0).RewardFor(BattleOutcome.Lose));

        [TestCase(0, 0)]
        [TestCase(150, -1)]
        [TestCase(30, 150)]
        public void WrongNumbersAreRejected(int win, int lose) =>
            Assert.Throws<InvalidOperationException>(() => new BattleRewards(win, lose));
    }
}
