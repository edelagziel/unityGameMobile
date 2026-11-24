using System;
using Game.Services.Config;
using Game.Services.Wallet;

namespace Game.ScreenNavigation
{
    /// <summary>
    /// The one place a battle pays Cash: the reward for the outcome comes from the config and goes to the wallet.
    /// The stand-in Win/Lose buttons call it now; the real battle will call the same method.
    /// </summary>
    public sealed class BattleRewardGranter
    {
        private readonly IConfigService config;
        private readonly IWalletService wallet;

        public BattleRewardGranter(IConfigService config, IWalletService wallet)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
            this.wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
        }

        /// <summary>Pays the reward and returns how much was paid.</summary>
        public int Grant(BattleOutcome outcome)
        {
            var rewards = config.Economy.BattleRewards;
            var reward = outcome switch
            {
                BattleOutcome.Win => rewards.Win,
                BattleOutcome.Lose => rewards.Lose,
                _ => 0
            };
            if (reward > 0) wallet.Add(reward); // the wallet takes only positive amounts; a loss may pay 0
            return reward;
        }
    }
}
