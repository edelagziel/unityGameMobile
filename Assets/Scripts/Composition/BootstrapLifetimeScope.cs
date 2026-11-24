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
        [Tooltip("Local configuration defaults: Assets/Config/GameConfig.asset.")]
        [SerializeField] private GameConfig gameConfig;

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
            // Services, in init order: BootstrapFlow starts them from top to bottom.
            builder.Register<PlayerPrefsSaveService>(Lifetime.Singleton).AsImplementedInterfaces(); // 1 · Save
            builder.Register<ScriptableObjectConfigService>(Lifetime.Singleton)                     // 2 · Config: the asset above
                .WithParameter(gameConfig)
                .AsImplementedInterfaces();
            builder.Register<WalletService>(Lifetime.Singleton).AsImplementedInterfaces();          // 3 · Wallet: loads Cash from Save

            builder.RegisterEntryPoint<BootstrapFlow>().WithParameter("firstScene", firstScene);
        }
    }
}
