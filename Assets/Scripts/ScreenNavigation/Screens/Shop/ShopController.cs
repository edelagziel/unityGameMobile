using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class ShopController : IStartable, IDisposable
    {
        private readonly ShopView view;
        private readonly ShopModel model;
        private readonly IScreenNavigator navigator;
        private bool started;

        public ShopController(ShopView view, ShopModel model, IScreenNavigator navigator)
        {
            this.view = view ?? throw new ArgumentNullException(nameof(view));
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.navigator = navigator ?? throw new ArgumentNullException(nameof(navigator));
        }

        public void Start()
        {
            if (started) return;
            started = true;
            view.TabPressed += OnShopPressed;
            view.Shown += view.SelectTab;
            if (view.isActiveAndEnabled) view.SelectTab();
        }

        public void Dispose()
        {
            if (!started) return;
            started = false;
            view.TabPressed -= OnShopPressed;
            view.Shown -= view.SelectTab;
        }

        private void OnShopPressed() => navigator.ResetTo<ShopView>();
    }
}
