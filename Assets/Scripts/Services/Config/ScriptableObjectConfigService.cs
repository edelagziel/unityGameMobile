using System;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>Checks the numbers once at startup, so a wrong asset stops Bootstrap with a clear message.</summary>
        public Task InitializeAsync(CancellationToken cancellation)
        {
            Economy.Validate();
            return Task.CompletedTask;
        }
    }
}
