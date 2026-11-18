using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class SkillTreeController : IStartable, IDisposable
    {
        private readonly SkillTreeView view;
        private readonly SkillTreeModel model;
        private readonly IScreenNavigator navigator;
        private bool started;

        public SkillTreeController(SkillTreeView view, SkillTreeModel model, IScreenNavigator navigator)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.TabPressed += OnSkillTreePressed;
            view.Shown += view.SelectTab;
            if (view.isActiveAndEnabled) view.SelectTab();
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.TabPressed -= OnSkillTreePressed;
            view.Shown -= view.SelectTab;
        }

        private void OnSkillTreePressed() => navigator.ResetTo<SkillTreeView>();
    }
}
