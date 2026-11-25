using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class ScreenNavigationLifetimeScope : LifetimeScope
    {
        [SerializeField] private ScreenView[] screens;

        protected override void Configure(IContainerBuilder builder)
        {
            if (screens == null || screens.Length == 0)
                throw new System.InvalidOperationException("Assign at least one screen view in ScreenNavigationLifetimeScope.");

            var screenRegistry = new ScreenRegistry(screens);

            foreach (var screen in screens)
            {
                var registration = builder.RegisterComponent(screen).As(screen.GetType());
                if (screen is SkillTreeView) registration.As<ISkillTreeView>();
            }

            builder.RegisterInstance(screenRegistry);
            builder.Register<ScreenNavigationHistory>(Lifetime.Scoped);
            builder.Register<IScreenNavigator, ScreenNavigator>(Lifetime.Scoped);
            builder.Register<MainMenuModel>(Lifetime.Scoped);
            builder.Register<SettingsModel>(Lifetime.Scoped);
            builder.Register<ShopModel>(Lifetime.Scoped);
            builder.Register<InventoryModel>(Lifetime.Scoped);
            builder.Register<SkillTreeModel>(Lifetime.Scoped).As<ISkillTreeModel>().AsSelf();
            builder.Register<RecipesModel>(Lifetime.Scoped);
            builder.Register<LevelModel>(Lifetime.Scoped);
            builder.Register<BattleRewardGranter>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MainMenuController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<SettingsController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<ShopController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<InventoryController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<SkillTreeController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<RecipesController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<LevelController>(Lifetime.Scoped).AsSelf();
            builder.RegisterEntryPoint<ScreenNavigationInitializer>(Lifetime.Scoped);

            builder.Register<TabHighlightSelection>(Lifetime.Scoped);
        }
    }
}
