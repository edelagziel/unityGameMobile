using System;
using System.Collections.Generic;

namespace Game.ScreenNavigation
{
    // Explicit scene references work even when a screen GameObject is inactive.
    public sealed class ScreenRegistry
    {
        private readonly Dictionary<Type, ScreenView> screens = new Dictionary<Type, ScreenView>();

        public ScreenRegistry(params ScreenView[] views)
        {
            if (views == null) throw new ArgumentNullException(nameof(views));
            foreach (var view in views)
            {
                if (view == null) throw new ArgumentException("A screen reference is missing.", nameof(views));
                var screenType = view.GetType();
                if (screens.ContainsKey(screenType))
                    throw new ArgumentException($"Duplicate screen type: {screenType}", nameof(views));
                screens.Add(screenType, view);
            }
        }

        public bool TryGet(Type screenType, out ScreenView view) => screens.TryGetValue(screenType, out view) && view != null;

        public IEnumerable<ScreenView> All => screens.Values;
    }
}
