using System;
using System.Collections.Generic;

namespace Game.ScreenNavigation
{
    // Owns the history and its no-duplicate invariant. It has no Unity dependency.
    public sealed class ScreenNavigationHistory
    {
        private readonly List<Type> history = new List<Type>();

        public int Count => history.Count;
        public Type Current => history[history.Count - 1];
        public bool HasCurrent => history.Count > 0;
        public Type Previous => history[history.Count - 2];
        public bool CanPop => history.Count > 1;

        public bool Push(Type screen)
        {
            if (history.Contains(screen)) return false;
            history.Add(screen);
            return true;
        }

        public bool Pop()
        {
            if (history.Count <= 1) return false;
            history.RemoveAt(history.Count - 1);
            return true;
        }

        public bool Replace(Type screen)
        {
            if (history.Count == 0 || history.Contains(screen)) return false;
            history[history.Count - 1] = screen;
            return true;
        }

        public void ResetTo(Type screen)
        {
            history.Clear();
            history.Add(screen);
        }
    }
}
