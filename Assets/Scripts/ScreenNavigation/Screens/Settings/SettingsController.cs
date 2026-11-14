using System;
using VContainer.Unity;

namespace Game.ScreenNavigation
{
    public sealed class SettingsController : IStartable, IDisposable
    {
        private readonly SettingsView view;
        private readonly IScreenNavigator navigator;
        private readonly SettingsModel model;

        public SettingsController(SettingsView view, IScreenNavigator navigator, SettingsModel model)
        {
            this.view = view;
            this.navigator = navigator;
            this.model = model;
        }

        public void Start()
        {
            view.ShowSoundEnabled(model.SoundEnabled);
            view.BackPressed += OnBackPressed;
            view.SoundChanged += OnSoundChanged;
        }

        public void Dispose()
        {
            view.BackPressed -= OnBackPressed;
            view.SoundChanged -= OnSoundChanged;
        }

        private void OnBackPressed() => navigator.Pop();
        private void OnSoundChanged(bool enabled) => model.SetSoundEnabled(enabled);
    }
}
