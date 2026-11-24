using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class InventoryController : IStartable, IDisposable
    {
        private readonly InventoryView view;
        private readonly InventoryModel model;
        private readonly IScreenNavigator navigator;
        private bool started;

        public InventoryController(InventoryView view, InventoryModel model, IScreenNavigator navigator)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.TabPressed += OnInventoryPressed;
            view.Shown += view.SelectTab;
            if (view.isActiveAndEnabled) view.SelectTab();
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.TabPressed -= OnInventoryPressed;
            view.Shown -= view.SelectTab;
        }

        private void OnInventoryPressed() => navigator.ResetTo<InventoryView>();
    }
}
