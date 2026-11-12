using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class MainMenuController : IStartable, IDisposable
    {
        private readonly MainMenuView view;
        private readonly IScreenNavigator navigator;
        private readonly IScreenFlow screenFlow;

        public MainMenuController(MainMenuView view, IScreenNavigator navigator, IScreenFlow screenFlow)
        {
            this.view = view;
            this.navigator = navigator;
            this.screenFlow = screenFlow;
        }

        public void Start()
        {
            view.SettingsPressed += OnSettingsPressed;
            view.PlayPressed += OnPlayPressed;
        }

        public void Dispose()
        {
            view.SettingsPressed -= OnSettingsPressed;
            view.PlayPressed -= OnPlayPressed;
        }

        private void OnSettingsPressed() => navigator.Push<SettingsView>();
        private void OnPlayPressed() => screenFlow.LoadLevel();
    }
}