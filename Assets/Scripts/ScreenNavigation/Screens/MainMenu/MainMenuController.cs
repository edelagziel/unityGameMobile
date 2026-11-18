using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class MainMenuController : IStartable, IDisposable
    {
        private readonly MainMenuView view;
        private readonly MainMenuModel model;
        private readonly IScreenNavigator navigator;
        private bool started;

        public MainMenuController(MainMenuView view, MainMenuModel model, IScreenNavigator navigator)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.SettingsPressed += OnSettingsPressed;
            view.PlayPressed += OnPlayPressed;
            view.TabPressed += OnBattlePressed;
            view.Shown += view.SelectTab;
            if (view.isActiveAndEnabled) view.SelectTab();
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.SettingsPressed -= OnSettingsPressed;
            view.PlayPressed -= OnPlayPressed;
            view.TabPressed -= OnBattlePressed;
            view.Shown -= view.SelectTab;
        }

        private void OnSettingsPressed() => navigator.Push<SettingsView>();
        private void OnPlayPressed() => navigator.Push<LevelView>();
        private void OnBattlePressed() => navigator.ResetTo<MainMenuView>();
    }
}
