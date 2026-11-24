using System;
using Game.Services.Config;
using Game.Services.Save;
using Game.Services.Wallet;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Bootstrap
{
    /// <summary>
    /// The app's root scope, on the only GameObject of the Bootstrap scene (scene 0 in the Scene List).
    /// Registers every service once, keeps them alive across scene loads (DontDestroyOnLoad),
    /// and starts <see cref="BootstrapFlow"/>.
    /// </summary>
    public sealed class BootstrapLifetimeScope : LifetimeScope
    {
        [SerializeField] private string firstScene = "Home";
        [Tooltip("Every economy number of the game: Assets/Config/EconomyConfig.asset.")]
        [SerializeField] private EconomyConfig economy;

        private static BootstrapLifetimeScope instance;

        protected override void Awake()
        {
            // Bootstrap runs once per launch. A second copy would mean a second set of services.
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void OnDestroy()
        {
            if (instance == this) instance = null;
            base.OnDestroy();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            if (economy == null)
                throw new InvalidOperationException("Drag Assets/Config/EconomyConfig into BootstrapLifetimeScope → Economy.");

            // Services, in init order: BootstrapFlow starts them from top to bottom.
            builder.Register<PlayerPrefsSaveService>(Lifetime.Singleton).AsImplementedInterfaces(); // 1 · Save
            builder.Register<ScriptableObjectConfigService>(Lifetime.Singleton)                     // 2 · Config: the asset above
                .WithParameter(economy)
                .AsImplementedInterfaces();
            builder.Register<WalletService>(Lifetime.Singleton).AsImplementedInterfaces();          // 3 · Wallet: loads Cash from Save

            builder.RegisterEntryPoint<BootstrapFlow>().WithParameter("firstScene", firstScene);
        }
    }
}
