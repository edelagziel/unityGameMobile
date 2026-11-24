using System;
using UnityEngine;

namespace Game.ScreenNavigation
{
    public sealed class ScreenNavigator : IScreenNavigator
    {
        private readonly ScreenNavigationHistory history;
        private readonly ScreenRegistry registry;

        public ScreenNavigator(ScreenNavigationHistory history, ScreenRegistry registry)
        {
            this.history = history;
            this.registry = registry;
        }

        public bool Push<TView>() where TView : ScreenView
        {
            var screen = typeof(TView);
            if (!IsRegistered(screen) || !history.Push(screen)) return false;
            ShowOnly(screen);
            return true;
        }

        public bool Pop()
        {
            if (!history.CanPop || !IsRegistered(history.Previous)) return false;
            if (!history.Pop()) return false;
            ShowOnly(history.Current);
            return true;
        }

        public bool Replace<TView>() where TView : ScreenView
        {
            var screen = typeof(TView);
            if (!IsRegistered(screen) || !history.Replace(screen)) return false;
            ShowOnly(screen);
            return true;
        }

        public bool ResetTo<TView>() where TView : ScreenView
        {
            var screen = typeof(TView);
            if (!IsRegistered(screen)) return false;
            history.ResetTo(screen);
            ShowOnly(screen);
            return true;
        }

        private bool IsRegistered(Type screen)
        {
            if (registry.TryGet(screen, out _)) return true;
            Debug.LogWarning($"Cannot navigate to unregistered screen: {screen}");
            return false;
        }

        private void ShowOnly(Type screen)
        {
            foreach (var view in registry.All) view.Hide();
            registry.TryGet(screen, out var current);
            current.Show();
        }
    }
}
