using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class ScreenNavigationLifetimeScope : LifetimeScope
    {
        [SerializeField] private ScreenView[] screens;
        [SerializeField] private TabBarView tabBar; // NEW

        protected override void Configure(IContainerBuilder builder)
        {
            if (screens == null || screens.Length == 0)
                throw new System.InvalidOperationException("Assign at least one screen view in ScreenNavigationLifetimeScope.");
            if (tabBar == null) // NEW
                throw new System.InvalidOperationException("Assign the TabBarView in ScreenNavigationLifetimeScope.");

            var screenRegistry = new ScreenRegistry(screens);

            foreach (var screen in screens)
                builder.RegisterComponent(screen).As(screen.GetType());

            builder.RegisterInstance(screenRegistry);
            builder.Register<ScreenNavigationHistory>(Lifetime.Scoped);
            builder.Register<IScreenNavigator, ScreenNavigator>(Lifetime.Scoped);
            builder.Register<IScreenFlow, ScreenFlow>(Lifetime.Singleton);
            builder.Register<SettingsModel>(Lifetime.Scoped);
            builder.RegisterEntryPoint<MainMenuController>(Lifetime.Scoped);
            builder.RegisterEntryPoint<SettingsController>(Lifetime.Scoped);
            builder.RegisterEntryPoint<ScreenNavigationInitializer>(Lifetime.Scoped);

            builder.RegisterComponent(tabBar); // NEW
            builder.RegisterEntryPoint<TabBarController>(Lifetime.Scoped); // NEW
        }
    }
}