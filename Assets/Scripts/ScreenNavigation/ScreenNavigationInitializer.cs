using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class ScreenNavigationInitializer : IStartable
    {
        private readonly IScreenNavigator navigator;

        public ScreenNavigationInitializer(IScreenNavigator navigator) => this.navigator = navigator;

        public void Start() => navigator.ResetTo<MainMenuView>();
    }
}
