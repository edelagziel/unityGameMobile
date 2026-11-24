using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class RecipesController : IStartable, IDisposable
    {
        private readonly RecipesView view;
        private readonly RecipesModel model;
        private readonly IScreenNavigator navigator;
        private bool started;

        public RecipesController(RecipesView view, RecipesModel model, IScreenNavigator navigator)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.TabPressed += OnRecipesPressed;
            view.Shown += view.SelectTab;
            if (view.isActiveAndEnabled) view.SelectTab();
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.TabPressed -= OnRecipesPressed;
            view.Shown -= view.SelectTab;
        }

        private void OnRecipesPressed() => navigator.ResetTo<RecipesView>();
    }
}
