using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class LevelController : IStartable, IDisposable
    {
        private readonly LevelView view;
        private readonly LevelModel model;
        private readonly IScreenNavigator navigator;
        private bool started;

        public LevelController(LevelView view, LevelModel model, IScreenNavigator navigator)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.BackPressed += OnBackPressed;
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.BackPressed -= OnBackPressed;
        }

        private void OnBackPressed() => navigator.Pop();
    }
}
