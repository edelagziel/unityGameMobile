using System;
using Game.Services.Config;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class LevelController : IStartable, IDisposable
    {
        private readonly LevelView view;
        private readonly LevelModel model;
        private readonly IScreenNavigator navigator;
        private readonly BattleRewardGranter rewards;
        private bool started;

        public LevelController(LevelView view, LevelModel model, IScreenNavigator navigator, BattleRewardGranter rewards)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
            this.rewards = rewards ?? throw new ArgumentNullException(nameof(rewards));
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.BackPressed += OnBackPressed;
            view.Finished += OnFinished;
            view.BackToMenuPressed += OnBackToMenuPressed;
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.BackPressed -= OnBackPressed;
            view.Finished -= OnFinished;
            view.BackToMenuPressed -= OnBackToMenuPressed;
        }

        private void OnBackPressed() => navigator.Pop();

        private void OnFinished(BattleOutcome outcome)
        {
            if (view.IsResultShown) return; // a battle pays once, even if it reports its end twice
            var reward = rewards.Grant(outcome);
            view.ShowResult(outcome, reward);
        }

        private void OnBackToMenuPressed() => navigator.ResetTo<MainMenuView>();
    }
}
