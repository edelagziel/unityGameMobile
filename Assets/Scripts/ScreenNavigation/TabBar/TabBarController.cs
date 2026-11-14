using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class TabBarController : IStartable, IDisposable
    {
        private readonly TabBarView view;
        private readonly IScreenNavigator navigator;

        public TabBarController(TabBarView view, IScreenNavigator navigator)
        {
            this.view = view;
            this.navigator = navigator;
        }

        public void Start()
        {
            view.TabPressed += OnTabPressed;
            view.ShowSelected(MenuTab.Battle);
        }

        public void Dispose() => view.TabPressed -= OnTabPressed;

        private void OnTabPressed(MenuTab tab)
        {
            switch (tab)
            {
                case MenuTab.Shop: navigator.ResetTo<ShopView>(); break;
                case MenuTab.Inventory: navigator.ResetTo<InventoryView>(); break;
                case MenuTab.Battle: navigator.ResetTo<MainMenuView>(); break;
                case MenuTab.SkillTree: navigator.ResetTo<SkillTreeView>(); break;
                case MenuTab.Recipes: navigator.ResetTo<RecipesView>(); break;
            }
            view.ShowSelected(tab);
        }
    }
}