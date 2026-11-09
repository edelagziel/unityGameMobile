using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class MainMenuController : IStartable, IDisposable
    {
        private readonly MainMenuView view;
        private readonly IScreenNavigator navigator;

        public MainMenuController(MainMenuView view, IScreenNavigator navigator)
        {
            this.view = view;
            this.navigator = navigator;
        }

        public void Start() => view.SettingsPressed += OnSettingsPressed;
        public void Dispose() => view.SettingsPressed -= OnSettingsPressed;
        private void OnSettingsPressed() => navigator.Push<SettingsView>();
    }
}
