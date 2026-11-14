using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.ScreenNavigation
{
    public sealed class SettingsView : ScreenView
    {
        [SerializeField] private Button backButton;
        [SerializeField] private Toggle soundToggle;
        public event Action BackPressed;
        public event Action<bool> SoundChanged;

        private void Awake()
        {
            if (backButton == null)
            {
                Debug.LogError("SettingsView needs a Back button reference.", this);
                return;
            }
            backButton.onClick.AddListener(OnBackClicked);
            if (soundToggle != null) soundToggle.onValueChanged.AddListener(OnSoundChanged);
        }

        private void OnDestroy()
        {
            if (backButton != null) backButton.onClick.RemoveListener(OnBackClicked);
            if (soundToggle != null) soundToggle.onValueChanged.RemoveListener(OnSoundChanged);
        }

        public void ShowSoundEnabled(bool enabled)
        {
            if (soundToggle != null) soundToggle.SetIsOnWithoutNotify(enabled);
        }

        private void OnBackClicked() => BackPressed?.Invoke();
        private void OnSoundChanged(bool enabled) => SoundChanged?.Invoke(enabled);
    }
}
