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

        public Task InitializeAsync(CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();
            var problem = defaults.Economy.FindProblem();
            if (problem != null)
                Debug.LogError($"EconomyConfig: {problem}", defaults.Economy);
            return Task.CompletedTask;
        }
    }
}
