using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Services.Config
{
    /// <summary>
    /// Today's config: the EconomyConfig asset that BootstrapLifetimeScope holds in its Inspector.
    /// A service is a plain C# class with no Inspector, so Bootstrap passes the asset to the constructor.
    /// </summary>
    public sealed class ScriptableObjectConfigService : IConfigService, IInitializableService
    {
        public ScriptableObjectConfigService(EconomyConfig economy)
        {
            // Unity's == also catches a destroyed asset, so no ?? here.
            if (economy == null)
                throw new ArgumentNullException(nameof(economy), "Drag the EconomyConfig asset into BootstrapLifetimeScope → Economy.");
            Economy = economy;
        }

        public EconomyConfig Economy { get; }

        /// <summary>
        /// Reports a wrong number in red in the Console, but lets the game run.
        /// A remote config will also fetch the server values here.
        /// </summary>
        public Task InitializeAsync(CancellationToken cancellation)
        {
            var problem = Economy.FindProblem();
            if (problem != null)
                Debug.LogError($"EconomyConfig: {problem} Fix it in Assets/Config/EconomyConfig.", Economy);
            return Task.CompletedTask;
        }
    }
}
