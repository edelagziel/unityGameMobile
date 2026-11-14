using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class LevelController : IStartable, IDisposable
    {
        private readonly LevelView view;
        private readonly IScreenFlow screenFlow;

        public LevelController(LevelView view, IScreenFlow screenFlow)
        {
            this.view = view;
            this.screenFlow = screenFlow;
        }

        public void Start() => view.BackPressed += OnBackPressed;
        public void Dispose() => view.BackPressed -= OnBackPressed;

        private void OnBackPressed() => screenFlow.LoadHome();
    }
}