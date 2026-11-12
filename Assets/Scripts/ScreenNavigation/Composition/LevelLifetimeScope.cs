using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class LevelLifetimeScope : LifetimeScope
    {
        [SerializeField] private LevelView levelView;

        protected override void Configure(IContainerBuilder builder)
        {
            if (levelView == null)
                throw new System.InvalidOperationException("Assign LevelView in LevelLifetimeScope.");

            builder.RegisterComponent(levelView);
            builder.Register<IScreenFlow, ScreenFlow>(Lifetime.Singleton);
            builder.RegisterEntryPoint<LevelController>(Lifetime.Scoped);
        }
    }
}