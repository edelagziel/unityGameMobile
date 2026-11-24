using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Services.Config
{
    public sealed class ScriptableObjectConfigService : IConfigService, IInitializableService
    {
        private readonly GameConfig defaults;

        public ScriptableObjectConfigService(GameConfig defaults)
        {
            if (defaults == null)
                throw new ArgumentNullException(nameof(defaults), "Assign GameConfig to BootstrapLifetimeScope.");
            if (defaults.Economy == null)
                throw new ArgumentException("Assign EconomyConfig to the Economy field of GameConfig.", nameof(defaults));
            this.defaults = defaults;
        }

        public IEconomyConfig Economy => defaults.Economy;

        private static string FindProblem(IEconomyConfig economy)
        {
            var rewards = economy.BattleRewards;
            if (rewards.Win < 1 || rewards.Lose < 0 || rewards.Lose > rewards.Win)
                return $"Battle rewards: a win must pay at least 1 and a loss from 0 up to a win (win {rewards.Win}, loss {rewards.Lose}).";

            var upgrade = economy.Upgrade;
            if (!(upgrade.BaseCost >= 1 && upgrade.Growth >= 1f && upgrade.MaxLevel >= 1))
                return $"Upgrade: base cost, growth and max level must all be at least 1 (base {upgrade.BaseCost}, growth {upgrade.Growth}, max {upgrade.MaxLevel}).";

            return null;
        }

        public Task InitializeAsync(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var problem = FindProblem(Economy);
            if (problem != null)
                Debug.LogError($"EconomyConfig: {problem}", defaults.Economy);
            return Task.CompletedTask;
        }
    }
}
