using Game.Services.Config;
using NUnit.Framework;

namespace Game.ScreenNavigation.Tests
{
    public sealed class BattleRewardGranterTests
    {
        [TestCase(BattleOutcome.Win, 75)]
        [TestCase(BattleOutcome.Lose, 12)]
        [TestCase((BattleOutcome)99, 0)]
        public void GrantsTheConfiguredReward(BattleOutcome outcome, int expected)
        {
            using var config = new FakeConfig(win: 75, lose: 12);
            var wallet = new FakeWallet();
            Assert.AreEqual(expected, new BattleRewardGranter(config, wallet).Grant(outcome));
            Assert.AreEqual(expected, wallet.Cash);
        }
    }
}
